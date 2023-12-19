using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProsjektOppgaveWebAPI.Controllers;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services;

namespace ProsjektOppgaveWebAPITest;

public class BlogControllerTest
{
    private readonly Mock<IBlogService> _mockService;
    private readonly BlogController _controller;

    public BlogControllerTest()
    {
        _mockService = new Mock<IBlogService>();
        _controller = new BlogController(_mockService.Object);
    }
    
    private static IEnumerable<AllBlogViewModel> GetTestBlogs()
    {
        return new List<AllBlogViewModel>
        {
            new AllBlogViewModel(),
            new AllBlogViewModel(),
            new AllBlogViewModel()
        };
    }
    
    
    
    // GET
    [Fact]
    public async Task GetAll_ReturnsCorrectType()
    {
        // Arrange
        _mockService.Setup(service => service.GetAllBlogs())
            .Returns(Task.FromResult(GetTestBlogs()));

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsType<List<AllBlogViewModel>>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectNumberOfBlogs()
    {
        // Arrange
        _mockService.Setup(service => service.GetAllBlogs())
            .Returns(Task.FromResult(GetTestBlogs()));

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.Equal(3, result.Count());
    }
    
    [Fact]
    public void Get_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = _controller.Get(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void Get_ReturnsNotFound_WhenIdIsInvalid()
    {
        // Arrange
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns((Blog)null);

        // Act
        var result = _controller.Get(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public void Get_ReturnsOk_WhenIdIsValid()
    {
        // Arrange
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns(new Blog());

        // Act
        var result = _controller.Get(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    
    
    // POST
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await _controller.Create(new BlogViewModel());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var blogViewModel = new BlogViewModel { BlogId = 0 };
        var blog = new Blog { BlogId = blogViewModel.BlogId };

        // Mock User
        const string userId = "user1";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _mockService.Setup(service => service.Save(blog, userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(blogViewModel);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("Get", createdAtActionResult.ActionName);
        //Assert.Equal(blog, createdAtActionResult.Value);
    }
    
    
    
    // PUT
    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdDoesNotMatchBlogId()
    {
        // Arrange
        var blog = new BlogViewModel { BlogId = 1 };

        // Act
        var result = await _controller.Update(2, blog);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenBlogDoesNotExist()
    {
        // Arrange
        var blogViewModel = new BlogViewModel { BlogId = 1 };
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns((Blog)null);

        // Act
        var result = await _controller.Update(1, blogViewModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        const string userId = "otherUserId";
        var blogViewModel = new BlogViewModel { BlogId = 1 };
        var blog = new Blog { BlogId = blogViewModel.BlogId, Owner = new IdentityUser { Id = userId, UserName = "otherUser" } };
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns(blog);
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

        // Act
        var result = await _controller.Update(1, blogViewModel);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var blogViewModel = new BlogViewModel { BlogId = 1 };
        const string userId = "testUserId";
        var blog = new Blog { BlogId = blogViewModel.BlogId, OwnerId = userId };

        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns(blog);

        // Mock User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _mockService.Setup(service => service.Save(It.IsAny<Blog>(), userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(1, blogViewModel);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(x => x.Save(It.IsAny<Blog>(), userId), Times.Once);
    }
    
    
    
    // DELETE
    [Fact]
    public void Delete_ReturnsNotFound_WhenBlogDoesNotExist()
    {
        // Arrange
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns((Blog)null);

        // Act
        var result = _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ReturnsUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        var blog = new Blog { BlogId = 1, OwnerId = "otherUserId" };
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns(blog);
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

        // Act
        var result = _controller.Delete(1);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var blog = new Blog { BlogId = 1, OwnerId = "testUserId" };
        _mockService.Setup(service => service.GetBlog(It.IsAny<int>()))
            .Returns(blog);

        // Mock User
        const string userId = "testUserId";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        _mockService.Setup(service => service.Delete(It.IsAny<int>(), userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(x => x.Delete(1, userId), Times.Once);
    }

}