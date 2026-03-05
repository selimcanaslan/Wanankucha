using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Entities;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> CreateUserAsync(
        string nameSurname, string email, string userName, string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        if (await _userRepository.ExistsWithEmailAsync(normalizedEmail, cancellationToken))
            return Result<Guid>.Failure(Error.Conflict("User.DuplicateEmail", "A user with this email already exists."));

        var normalizedUserName = userName.ToUpperInvariant();
        if (await _userRepository.ExistsWithUsernameAsync(normalizedUserName, cancellationToken))
            return Result<Guid>.Failure(Error.Conflict("User.DuplicateUsername", "A user with this username already exists."));

        var user = User.Create(nameSurname, email, userName, _passwordHasher.HashPassword(password));

        await _userRepository.AddAsync(user, cancellationToken);

        // Assign default "User" role
        var defaultRole = await _roleRepository.GetByNameAsync("USER", cancellationToken);
        if (defaultRole == null)
        {
            defaultRole = Role.Create("User");
            await _roleRepository.AddAsync(defaultRole, cancellationToken);
        }

        await _roleRepository.AddUserRoleAsync(
            UserRole.Create(user.Id, defaultRole.Id),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }

    public async Task<UserDto?> FindByEmailOrUsernameAsync(
        string emailOrUsername, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailOrUsernameAsync(emailOrUsername.ToUpperInvariant(), cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(
        Guid userId, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;
        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<IEnumerable<UserListItemDto>> GetAllUsersAsync(
        int page, int size, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(page, size, cancellationToken);
        return users.Select(u => new UserListItemDto
        {
            Id = u.Id,
            Email = u.Email,
            UserName = u.UserName,
            NameSurname = u.NameSurname
        });
    }

    public async Task UpdateRefreshTokenAsync(
        Guid userId, string refreshToken, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return;

        user.SetRefreshToken(refreshToken, endDate);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByRefreshTokenAsync(refreshToken, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    // ─── Private helpers ─────────────────────────────────────────────────────

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        UserName = user.UserName,
        NameSurname = user.NameSurname,
        RefreshToken = user.RefreshToken,
        RefreshTokenEndDate = user.RefreshTokenEndDate
    };
}
