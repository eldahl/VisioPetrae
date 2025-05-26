using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using inference_registry.Controllers;
using inference_registry.Models;
using inference_registry.Services;
using System.IO;
using System.Threading.Tasks;

namespace inference_registry.Tests.Controllers;

public class RequestControllerTests
{
    private readonly Mock<ILogger<RequestController>> _loggerMock;
    private readonly InferenceServerRegistry _registry;
    private readonly RequestController _controller;

    public RequestControllerTests()
    {
        _loggerMock = new Mock<ILogger<RequestController>>();
        _registry = new InferenceServerRegistry();
        _controller = new RequestController(_registry, _loggerMock.Object);
    }

    [Fact]
    public async Task RequestInference_WhenNoServersAvailable_ReturnsServiceUnavailable()
    {
        // Arrange
        var request = new InferenceRequest
        {
            prompt = "test prompt",
            image = CreateMockFormFile()
        };

        // Act
        var result = await _controller.RequestInference(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(503, statusCodeResult.StatusCode);
        Assert.Equal("No available servers", statusCodeResult.Value);
    }

    [Fact]
    public async Task RequestInference_WhenServerAvailable_ReturnsInferenceResponse()
    {
        // Arrange

        var dto = new InferenceServerDTO 
        { 
            Hostname = "host1", 
            Port = 8000,
            MaxTasks = 10,
            IsAvailable = true,
            Status = "Online"
        };
        var mockServer = new Mock<InferenceServer>(MockBehavior.Loose, dto);
        mockServer.Setup(s => s.RequestInference(It.IsAny<InferenceRequest>()))
            .ReturnsAsync(new InferenceResponse 
            { 
                server_uuid = "test-uuid",
                dialog_uuid = "test-dialog-uuid",
                response = "test response"
            });
        _registry.AddServer(mockServer.Object);

        var request = new InferenceRequest
        {
            prompt = "test prompt",
            image = CreateMockFormFile()
        };

        // Act
        var result = await _controller.RequestInference(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<InferenceResponse>(okResult.Value);
        Assert.Equal("test-uuid", response.server_uuid);
        Assert.Equal("test-dialog-uuid", response.dialog_uuid);
        Assert.Equal("test response", response.response);
    }

    private IFormFile CreateMockFormFile()
    {
        var content = "Hello World from a Fake File";
        var fileName = "test.jpg";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        return new FormFile(stream, 0, stream.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
} 