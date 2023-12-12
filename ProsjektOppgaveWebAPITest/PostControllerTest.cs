using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProsjektOppgaveWebAPI.Controllers;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services;

namespace ProsjektOppgaveWebAPITest;

public class PostControllerTest
{
    private readonly Mock<IBlogService> _serviceMock;
    private readonly PostController _controller;

    public PostControllerTest()
    {
        _serviceMock = new Mock<IBlogService>();
        _controller = new PostController(_serviceMock.Object);
    }

    
    
    // GET
    [Fact]
    public async Task GetPosts_ReturnsExpectedPosts()
    {
        // Arrange
        const int blogId = 1;
        var expectedPosts = new List<Post> { new Post(), new Post() };
        _serviceMock.Setup(service => service.GetPostsForBlog(blogId)).ReturnsAsync(expectedPosts);

        // Act
        var result = await _controller.GetPosts(blogId);

        // Assert
        Assert.Equal(expectedPosts, result);
        _serviceMock.Verify(x => x.GetPostsForBlog(blogId), Times.Once);
    }
    
    [Fact]
    public void GetPost_ReturnsExpectedPost()
    {
        // Arrange
        const int postId = 1;
        var expectedPost = new Post();
        _serviceMock.Setup(service => service.GetPost(postId)).Returns(expectedPost);

        // Act
        var result = _controller.GetPost(postId);

        // Assert
        Assert.Equal(expectedPost, result);
        _serviceMock.Verify(x => x.GetPost(postId), Times.Once);
    }

    
    
    // POST
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await _controller.Create(new PostViewModel());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenBlogIsClosed()
    {
        // Arrange
        var post = new PostViewModel { BlogId = 1 };
        var blog = new Blog { Status = 1 };
        _serviceMock.Setup(service => service.GetBlog(post.BlogId)).Returns(blog);

        // Act
        var result = await _controller.Create(post);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenModelStateIsValidAndBlogIsOpen()
    {
        // Arrange
        var post = new PostViewModel { BlogId = 1 };
        var blog = new Blog { Status = 0 };
        _serviceMock.Setup(service => service.GetBlog(post.BlogId)).Returns(blog);

        var newPost = new Post { BlogId = post.BlogId };

        // Mock User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId"),
        }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = await _controller.Create(post);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetPosts", createdAtActionResult.ActionName);
        Assert.Equal(post, createdAtActionResult.Value);
        _serviceMock.Verify(x => x.SavePost(It.IsAny<Post>(), "userId"), Times.Once);
    }

    
    
    // PUT
    [Fact]
    public void Update_ReturnsBadRequest_WhenIdDoesNotMatchPostId()
    {
        // Arrange
        var post = new PostViewModel { PostId = 1 };

        // Act
        var result = _controller.Update(2, post);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void Update_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var post = new PostViewModel { PostId = 1 };
        _serviceMock.Setup(service => service.GetPost(post.PostId)).Returns((Post)null);

        // Act
        var result = _controller.Update(1, post);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Update_ReturnsUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        var postViewModel = new PostViewModel { PostId = 1 };
        var post = new Post { PostId = postViewModel.PostId, OwnerId = "user1" };
        var existingPost = new Post { PostId = 1, OwnerId = "user2" };
        _serviceMock.Setup(service => service.GetPost(post.PostId)).Returns(existingPost);

        // Mock User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user1"),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = _controller.Update(1, postViewModel);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Update_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var postViewModel = new PostViewModel { PostId = 1 };
        var post = new Post { PostId = postViewModel.PostId, OwnerId = "user1" };
        var existingPost = new Post { PostId = 1, OwnerId = "user1" };
        _serviceMock.Setup(service => service.GetPost(post.PostId)).Returns(existingPost);

        // Mock User
        const string userId = "user1";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = _controller.Update(1, postViewModel);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(x => x.SavePost(It.IsAny<Post>(), userId), Times.Once);
    }
    
    
    
    // DELETE
    [Fact]
    public void Delete_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        const int postId = 1;
        _serviceMock.Setup(service => service.GetPost(postId)).Returns((Post)null);

        // Act
        var result = _controller.Delete(postId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ReturnsUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        const int postId = 1;
        var post = new Post { PostId = postId, OwnerId = "user1" };
        _serviceMock.Setup(service => service.GetPost(postId)).Returns(post);

        // Mock User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user2"),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = _controller.Delete(postId);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
    {
        // Arrange
        const int postId = 1;
        var post = new Post { PostId = postId, OwnerId = "user1" };
        _serviceMock.Setup(service => service.GetPost(postId)).Returns(post);

        // Mock User
        const string userId = "user1";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            // other claims as needed
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = _controller.Delete(postId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(x => x.DeletePost(postId, userId), Times.Once);
    }
}