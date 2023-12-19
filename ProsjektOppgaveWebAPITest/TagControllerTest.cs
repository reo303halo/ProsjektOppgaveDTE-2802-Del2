using Microsoft.AspNetCore.Mvc;
using Moq;
using ProsjektOppgaveWebAPI.Controllers;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services.TagServices;

namespace ProsjektOppgaveWebAPITest;

public class TagControllerTest
{
    private readonly Mock<ITagService> _serviceMock;
    private readonly TagController _controller;

    public TagControllerTest()
    {
        _serviceMock = new Mock<ITagService>();
        _controller = new TagController(_serviceMock.Object);
    }
    
    // GET
    [Fact]
    public void GetComment_ReturnsExpectedComment()
    {
        // Arrange
        var name = "TestName";
        var expectedTag = new Tag { Name = name};
        _serviceMock.Setup(service => service.GetTag(name)).Returns(expectedTag);

        // Act
        var result = _controller.GetTag(name);

        // Assert
        Assert.Equal(expectedTag, result);
        _serviceMock.Verify(x => x.GetTag(name), Times.Once);
    }

    
    // POST
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await _controller.Create(new TagViewModel());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenModelStateIsValid()
    {
        // Arrange
        var t = new TagViewModel { Name = "Test" };
        var tag = new Tag { Name = t.Name };

        // Act
        var result = await _controller.Create(t);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        _serviceMock.Verify(x => x.Save(It.IsAny<Tag>()), Times.Once);
    }
}
