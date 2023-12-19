using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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
        const string name = "test-name";
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
        var t = new TagViewModel { Name = "test" };

        // Act
        var result = await _controller.Create(t);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        _serviceMock.Verify(x => x.Save(It.IsAny<Tag>()), Times.Once);
    }
    
    [Fact]
    public async Task Create_ReturnsJsonResult_WhenTagExists()
    {
        // Arrange
        var t = new TagViewModel { Name = "Test" };
        var tag = new Tag { Name = t.Name };
        _serviceMock.Setup(x => x.GetTag(t.Name)).Returns(tag);

        // Act
        var result = await _controller.Create(t);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(tag, jsonResult.Value);
    }
    
    [Fact]
    public async Task CreateRelation_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await _controller.CreateRelation(new BlogTagRelationsViewModel());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateRelation_ReturnsOk_WhenSuccessful()
    {

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext 
            { 
                User = new ClaimsPrincipal(new ClaimsIdentity(new [] 
                {
                    new Claim(ClaimTypes.NameIdentifier, "testUserId")
                })) 
            }
        };
        var relation = new BlogTagRelationsViewModel();

        // Act
        var result = await _controller.CreateRelation(relation);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Relation successful!", okResult.Value);
    }

    
}
