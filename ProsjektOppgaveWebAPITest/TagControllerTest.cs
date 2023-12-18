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
        const int tagId = 1;
        var expectedTag = new Tag();
        _serviceMock.Setup(service => service.GetTag(tagId)).Returns(expectedTag);

        // Act
        var result = _controller.GetTag(tagId);

        // Assert
        Assert.Equal(expectedTag, result);
        _serviceMock.Verify(x => x.GetTag(tagId), Times.Once);
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
