using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;
using Wanankucha.Domain.Entities;
using Wanankucha.Domain.Repositories;

namespace Wanankucha.Application.Services;

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

    public async Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password)
    {
        var normalizedEmail = email.ToUpperInvariant();
        if (await _userRepository.ExistsWithEmailAsync(normalizedEmail))
        {
            return new ServiceResponse<Guid>("A user with this email already exists.");
        }

        var normalizedUserName = userName.ToUpperInvariant();
        if (await _userRepository.ExistsWithUsernameAsync(normalizedUserName))
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

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<Guid>(user.Id, "User created successfully.");
    }

    public async Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        var normalized = emailOrUsername.ToUpperInvariant();
        
        var user = await _userRepository.FindByEmailOrUsernameAsync(normalized);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size)
    {
        var users = await _userRepository.GetAllAsync(page, size);

        return users.Select(MapToDto);
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = endDate;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.FindByRefreshTokenAsync(refreshToken);

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
