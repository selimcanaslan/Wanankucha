using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Entities;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        if (await _userRepository.ExistsWithEmailAsync(normalizedEmail, cancellationToken))
        {
            return new ServiceResponse<Guid>("A user with this email already exists.");
        }

        var normalizedUserName = userName.ToUpperInvariant();
        if (await _userRepository.ExistsWithUsernameAsync(normalizedUserName, cancellationToken))
        {
            return new ServiceResponse<Guid>("A user with this username already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            NormalizedUserName = normalizedUserName,
            Email = email,
            NormalizedEmail = normalizedEmail,
            NameSurname = nameSurname,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ServiceResponse<Guid>(user.Id, "User created successfully.");
    }

    public async Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        var normalized = emailOrUsername.ToUpperInvariant();
        
        var user = await _userRepository.FindByEmailOrUsernameAsync(normalized, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(page, size, cancellationToken);

        return users.Select(MapToDto);
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return;

        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = endDate;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByRefreshTokenAsync(refreshToken, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            NameSurname = user.NameSurname,
            RefreshToken = user.RefreshToken,
            RefreshTokenEndDate = user.RefreshTokenEndDate
        };
    }
}
