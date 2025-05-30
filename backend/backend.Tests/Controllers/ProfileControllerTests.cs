using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using backend.Controllers;
using backend.Models;
using backend.Services;
using backend.Persistence;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
namespace backend.Tests.Controllers;

public class ProfileControllerTests
{
    private readonly Mock<ProfileService> _serviceMock;
    private readonly Mock<ILogger<ProfileController>> _loggerMock;
    private readonly ProfileController _controller;
    private readonly Mock<JwtService> _jwtServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public ProfileControllerTests()
    {
        var dbMock = new Mock<IMongoDBContext>();
        _serviceMock = new Mock<ProfileService>(dbMock.Object);
        _loggerMock = new Mock<ILogger<ProfileController>>();
        
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["JWT:JWTSigningKey"]).Returns("test-secret-key");
        _configurationMock.Setup(c => c["JWT:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["JWT:Audience"]).Returns("test-audience");

        _jwtServiceMock = new Mock<JwtService>(_configurationMock.Object);
        _controller = new ProfileController(_serviceMock.Object, _jwtServiceMock.Object, _loggerMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.ValidateToken(It.IsAny<string>()))
            .Returns(new System.Security.Claims.ClaimsPrincipal());
    }

    [Fact]
    public async Task GetAllProfiles_ReturnsOkWithProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new Profile(new ProfileDTO { Username = "user1" }),
            new Profile(new ProfileDTO { Username = "user2" })
        };
        _serviceMock.Setup(s => s.GetAllProfiles())
            .ReturnsAsync(profiles);

        // Act
        var result = await _controller.GetAllProfiles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfiles = Assert.IsAssignableFrom<IEnumerable<Profile>>(okResult.Value);
        Assert.Equal(2, returnedProfiles.Count());
    }

    [Fact]
    public async Task GetProfile_WhenProfileExists_ReturnsOkWithProfile()
    {
        // Arrange
        var profile = new Profile(new ProfileDTO { Username = "testuser" });
        _serviceMock.Setup(s => s.GetProfileByUuid(It.IsAny<string>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _controller.GetProfile(profile.Uuid);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<Profile>(okResult.Value);
        Assert.Equal(profile.Uuid, returnedProfile.Uuid);
    }

    [Fact]
    public async Task GetProfile_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetProfileByUuid(It.IsAny<string>()))
            .ReturnsAsync((Profile?)null);

        // Act
        var result = await _controller.GetProfile("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetProfileByUsername_WhenProfileExists_ReturnsOkWithProfile()
    {
        // Arrange
        var profile = new Profile(new ProfileDTO { Username = "testuser" });
        _serviceMock.Setup(s => s.GetProfileByUsername(It.IsAny<string>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _controller.GetProfileByUsername(profile.Username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<Profile>(okResult.Value);
        Assert.Equal(profile.Username, returnedProfile.Username);
    }

    [Fact]
    public async Task GetProfileByUsername_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetProfileByUsername(It.IsAny<string>()))
            .ReturnsAsync((Profile?)null);

        // Act
        var result = await _controller.GetProfileByUsername("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProfile_WithValidDTO_ReturnsOkWithCreatedProfile()
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
        var profile = new Profile(dto);
        _serviceMock.Setup(s => s.CreateProfile(It.IsAny<ProfileDTO>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _controller.CreateProfile(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<Profile>(okResult.Value);
        Assert.Equal(dto.Username, returnedProfile.Username);
    }

    [Fact]
    public async Task UpdateProfile_WhenProfileExists_ReturnsOk()
    {
        // Arrange
        var dto = new ProfileDTO
        {
            Username = "updateduser",
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            Bio = "Updated bio",
            AvatarUrl = "https://example.com/updated.jpg"
        };
        _serviceMock.Setup(s => s.ProfileExists(It.IsAny<string>()))
            .ReturnsAsync(true);
        _serviceMock.Setup(s => s.UpdateProfile(It.IsAny<string>(), It.IsAny<ProfileDTO>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateProfile("test-uuid", dto);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProfile_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var dto = new ProfileDTO { Username = "updateduser" };
        _serviceMock.Setup(s => s.ProfileExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateProfile("nonexistent", dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProfile_WhenProfileExists_ReturnsOk()
    {
        // Arrange
        var userId = "test-uuid";
        var uuid = "test-uuid";
        
        var _contextMock = new Mock<HttpContext>();
        _contextMock.Setup(c => c.User.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, uuid));

        var _controllerContext = new ControllerContext(new ActionContext(_contextMock.Object, new RouteData(), new ControllerActionDescriptor()));
        _controller.ControllerContext = _controllerContext;

        _serviceMock.Setup(s => s.ProfileExists(It.IsAny<string>()))
            .ReturnsAsync(true);
        _serviceMock.Setup(s => s.DeleteProfile(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProfile(userId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteProfile_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "test-uuid";
        var uuid = "test-uuid";

        var _contextMock = new Mock<HttpContext>();
        _contextMock.Setup(c => c.User.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, uuid));

        var _controllerContext = new ControllerContext(new ActionContext(_contextMock.Object, new RouteData(), new ControllerActionDescriptor()));
        _controller.ControllerContext = _controllerContext;


        _serviceMock.Setup(s => s.ProfileExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProfile(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
} 