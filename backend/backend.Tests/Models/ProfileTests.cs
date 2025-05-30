using Xunit;
using backend.Models;

namespace backend.Tests.Models;

public class ProfileTests
{
    [Fact]
    public void Profile_Constructor_WithDTO_SetsPropertiesCorrectly()
    {
        // Arrange
        var dto = new ProfileDTO
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Bio = "Test bio",
            AvatarUrl = "https://example.com/avatar.jpg"
        };

        // Act
        var profile = new Profile(dto);

        // Assert
        Assert.NotEmpty(profile.Uuid.ToString());
        Assert.Equal(dto.Username, profile.Username);
        Assert.Equal(dto.Email, profile.Email);
        Assert.Equal(dto.FirstName, profile.FirstName);
        Assert.Equal(dto.LastName, profile.LastName);
        Assert.Equal(dto.Bio, profile.Bio);
        Assert.Equal(dto.AvatarUrl, profile.AvatarUrl);
        Assert.Equal(DateTime.UtcNow.Date, profile.CreatedAt.Date);
        Assert.Equal(DateTime.UtcNow.Date, profile.UpdatedAt.Date);
    }

    [Fact]
    public void Profile_UpdatedFromDTO_UpdatesPropertiesCorrectly()
    {
        // Arrange
        var initialDto = new ProfileDTO
        {
            Username = "initialuser",
            Email = "initial@example.com",
            FirstName = "Initial",
            LastName = "User",
            Bio = "Initial bio",
            AvatarUrl = "https://example.com/initial.jpg"
        };
        var profile = new Profile(initialDto);
        var originalUuid = profile.Uuid;
        var originalCreatedAt = profile.CreatedAt;

        var updateDto = new ProfileDTO
        {
            Username = "updateduser",
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            Bio = "Updated bio",
            AvatarUrl = "https://example.com/updated.jpg"
        };

        // Act
        profile.UpdatedFromDTO(updateDto);

        // Assert
        Assert.Equal(originalUuid, profile.Uuid);
        Assert.Equal(originalCreatedAt, profile.CreatedAt);
        Assert.Equal(updateDto.Username, profile.Username);
        Assert.Equal(updateDto.Email, profile.Email);
        Assert.Equal(updateDto.FirstName, profile.FirstName);
        Assert.Equal(updateDto.LastName, profile.LastName);
        Assert.Equal(updateDto.Bio, profile.Bio);
        Assert.Equal(updateDto.AvatarUrl, profile.AvatarUrl);
        Assert.True(profile.UpdatedAt > originalCreatedAt);
    }
} 