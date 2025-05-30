using Xunit;
using Moq;
using backend.Models;
using backend.Services;
using MongoDB.Driver;
using System.Threading.Tasks;
using backend.Persistence;

namespace backend.Tests.Services;

public class ProfileServiceTests
{
    private readonly Mock<IMongoDBContext> _dbMock;
    private readonly Mock<IMongoCollection<Profile>> _collectionMock;
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _dbMock = new Mock<IMongoDBContext>();
        _collectionMock = new Mock<IMongoCollection<Profile>>();
        _dbMock.Setup(db => db.Collection<Profile>()).Returns(_collectionMock.Object);
        _service = new ProfileService(_dbMock.Object);
    }

    [Fact]
    public async Task GetAllProfiles_ReturnsAllProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new Profile(new ProfileDTO { Username = "user1" }),
            new Profile(new ProfileDTO { Username = "user2" })
        };
        var cursor = new Mock<IAsyncCursor<Profile>>();
        cursor.Setup(c => c.Current).Returns(profiles);
        cursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<FindOptions<Profile>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _service.GetAllProfiles();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProfileByUuid_WhenProfileExists_ReturnsProfile()
    {
        // Arrange
        var profile = new Profile(new ProfileDTO { Username = "testuser" });
        var cursor = new Mock<IAsyncCursor<Profile>>();
        cursor.Setup(c => c.Current).Returns(new List<Profile> { profile });
        cursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<FindOptions<Profile>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _service.GetProfileByUuid(profile.Uuid.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.Uuid, result!.Uuid);
    }

    [Fact]
    public async Task GetProfileByUsername_WhenProfileExists_ReturnsProfile()
    {
        // Arrange
        var profile = new Profile(new ProfileDTO { Username = "testuser" });
        var cursor = new Mock<IAsyncCursor<Profile>>();
        cursor.Setup(c => c.Current).Returns(new List<Profile> { profile });
        cursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<FindOptions<Profile>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        // Act
        var result = await _service.GetProfileByUsername(profile.Username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.Username, result!.Username);
    }

    [Fact]
    public async Task CreateProfile_WithValidDTO_CreatesAndReturnsProfile()
    {
        // Arrange
        var dto = new ProfileDTO
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Bio = "New bio",
            AvatarUrl = "https://example.com/new.jpg"
        };

        _collectionMock.Setup(c => c.InsertOneAsync(
            It.IsAny<Profile>(),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateProfile(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Uuid.ToString());
        Assert.Equal(dto.Username, result.Username);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.FirstName, result.FirstName);
        Assert.Equal(dto.LastName, result.LastName);
        Assert.Equal(dto.Bio, result.Bio);
        Assert.Equal(dto.AvatarUrl, result.AvatarUrl);
    }

    [Fact]
    public async Task UpdateProfile_WhenProfileExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var profile = new Profile(new ProfileDTO { Username = "testuser" });
        var cursor = new Mock<IAsyncCursor<Profile>>();
        cursor.Setup(c => c.Current).Returns(new List<Profile> { profile });
        cursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _collectionMock.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<FindOptions<Profile>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        _collectionMock.Setup(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<Profile>(),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

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
        var result = await _service.UpdateProfile(profile.Uuid.ToString(), updateDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteProfile_WhenProfileExists_DeletesAndReturnsTrue()
    {
        // Arrange
        _collectionMock.Setup(c => c.CountDocumentsAsync(
            It.IsAny<FilterDefinition<Profile>>(), 
            It.IsAny<CountOptions>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _collectionMock.Setup(c => c.DeleteOneAsync(
            It.IsAny<FilterDefinition<Profile>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        // Act
        var result = await _service.DeleteProfile("test-uuid");

        // Assert
        Assert.True(result);
    }
} 