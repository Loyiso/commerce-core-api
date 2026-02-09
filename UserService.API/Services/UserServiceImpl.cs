using UserService.API.Contracts;
using UserService.API.Models;
using UserService.API.Repositories;

namespace UserService.API.Services;

public class UserServiceImpl : IUserService
{
    private readonly IUserRepository _repo;

    public UserServiceImpl(IUserRepository repo) => _repo = repo;

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _repo.GetAllAsync(ct);
        return users.Select(ToResponse).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(UserCreateRequest request, CancellationToken ct = default)
    {
        Validate(request.FirstName, request.LastName);

        var user = new UserProfile
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email?.Trim() ?? string.Empty,
            Phone = request.Phone?.Trim() ?? string.Empty,
            Address = new Address
            {
                Line1 = request.Address.Line1,
                Line2 = request.Address.Line2,
                City = request.Address.City,
                Province = request.Address.Province,
                PostalCode = request.Address.PostalCode,
                Country = request.Address.Country
            }
        };

        var created = await _repo.AddAsync(user, ct);
        return ToResponse(created);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UserUpdateRequest request, CancellationToken ct = default)
    {
        Validate(request.FirstName, request.LastName);

        var user = new UserProfile
        {
            Id = id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email?.Trim() ?? string.Empty,
            Phone = request.Phone?.Trim() ?? string.Empty,
            Address = new Address
            {
                Line1 = request.Address.Line1,
                Line2 = request.Address.Line2,
                City = request.Address.City,
                Province = request.Address.Province,
                PostalCode = request.Address.PostalCode,
                Country = request.Address.Country
            }
        };

        var updated = await _repo.UpdateAsync(user, ct);
        return updated is null ? null : ToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) =>
        _repo.DeleteAsync(id, ct);

    private static void Validate(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required.");
    }

    private static UserResponse ToResponse(UserProfile user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Address = new AddressDto
        {
            Line1 = user.Address.Line1,
            Line2 = user.Address.Line2,
            City = user.Address.City,
            Province = user.Address.Province,
            PostalCode = user.Address.PostalCode,
            Country = user.Address.Country
        },
        CreatedAtUtc = user.CreatedAtUtc,
        UpdatedAtUtc = user.UpdatedAtUtc
    };
}
