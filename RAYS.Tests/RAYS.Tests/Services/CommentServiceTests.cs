using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAYS.Models;
using RAYS.Repositories;
using RAYS.Services;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepo;
    private readonly Mock<IPostRepository> _mockPostRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ILogger<CommentService>> _mockLogger;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _mockCommentRepo = new Mock<ICommentRepository>();
        _mockPostRepo = new Mock<IPostRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<CommentService>>();
        _commentService = new CommentService(_mockCommentRepo.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mockLogger.Object);
    }

    // Positive Test Case Example

    [Fact]
    public async Task AddComment_Success()
    {
        // Arrange
        var comment = new Comment
        {
            PostId = 1,
            UserId = 1,
            Text = "Test Comment",
            CreatedAt = DateTime.UtcNow
        };

        _mockPostRepo.Setup(repo => repo.GetByIdAsync(comment.PostId)).ReturnsAsync(new Post()); // Simulate post found
        _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(comment.UserId)).ReturnsAsync(new User
        {
            Username = "TestUser",
            Email = "testuser@example.com",  // Required property
            PasswordHash = "hashedpassword"  // Required property
        }); // Simulate user found
        _mockCommentRepo.Setup(repo => repo.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask); // Simulate successful add

        // Act
        var result = await _commentService.AddComment(comment);

        // Assert
        Assert.Equal(comment.Text, result.Text);
        _mockCommentRepo.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once); // Ensure AddAsync was called
    }

    [Fact]
    public async Task UpdateComment_Success()
    {
        // Arrange
        var comment = new Comment { Id = 1, PostId = 1, UserId = 1, Text = "Old Comment", CreatedAt = DateTime.UtcNow };
        _mockCommentRepo.Setup(repo => repo.GetByIdAsync(comment.Id)).ReturnsAsync(comment); // Simulate existing comment
        _mockCommentRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask); // Simulate successful update

        // Act
        var updatedComment = await _commentService.UpdateComment(comment.Id, "Updated Comment", comment.UserId);

        // Assert
        Assert.Equal("Updated Comment", updatedComment.Text);
        _mockCommentRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Comment>()), Times.Once); // Ensure UpdateAsync was called
    }

    [Fact]
    public async Task GetCommentsForPost_Success()
    {
        // Arrange
        var post = new Post { Id = 1, Content = "Test Post Content" }; // Using the 'Content' property instead of 'Title'
        _mockPostRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(post); // Simulate the post found

        var comments = new List<Comment>
        {
            new Comment { Id = 1, PostId = 1, UserId = 1, Text = "Comment 1", CreatedAt = DateTime.UtcNow },
            new Comment { Id = 2, PostId = 1, UserId = 1, Text = "Comment 2", CreatedAt = DateTime.UtcNow }
        };
        _mockCommentRepo.Setup(repo => repo.GetAllAsync(1)).ReturnsAsync(comments); // Simulate retrieving comments for the post

        // Simulate a user for each comment
        _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new User 
        {
            Username = "TestUser",
            Email = "testuser@example.com",
            PasswordHash = "hashedpassword"
        });

        // Act
        var result = await _commentService.GetCommentsForPost(1);

        // Assert
        Assert.Equal(2, result.Count()); // Assert that we got 2 comments
        _mockPostRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once); // Ensure GetByIdAsync was called
        _mockCommentRepo.Verify(repo => repo.GetAllAsync(1), Times.Once); // Ensure GetAllAsync was called
    }

    [Fact]
    public async Task DeleteComment_Success()
    {
        // Arrange
        var comment = new Comment { Id = 1, PostId = 1, UserId = 1, Text = "Test Comment", CreatedAt = DateTime.UtcNow };
        _mockCommentRepo.Setup(repo => repo.GetByIdAsync(comment.Id)).ReturnsAsync(comment); // Simulate existing comment
        _mockCommentRepo.Setup(repo => repo.DeleteAsync(comment.Id)).Returns(Task.CompletedTask); // Simulate successful deletion

        // Act
        await _commentService.DeleteComment(comment.Id, comment.UserId);

        // Assert
        _mockCommentRepo.Verify(repo => repo.DeleteAsync(comment.Id), Times.Once); // Ensure DeleteAsync was called
    }

    // Negative Test Cases
    [Fact]
    public async Task AddComment_PostNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var comment = new Comment
        {
            PostId = 1,
            UserId = 1,
            Text = "Test Comment",
            CreatedAt = DateTime.UtcNow
        };

        // Fixing the nullable warning by using a nullable Post return type
        _mockPostRepo.Setup(repo => repo.GetByIdAsync(comment.PostId)).ReturnsAsync((Post?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _commentService.AddComment(comment));
    }


    [Fact]
    public async Task UpdateComment_CommentNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var comment = new Comment { Id = 1, PostId = 1, UserId = 1, Text = "Old Comment", CreatedAt = DateTime.UtcNow };
        _mockCommentRepo.Setup(repo => repo.GetByIdAsync(comment.Id)).ReturnsAsync((Comment?)null); // Simulate comment not found

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _commentService.UpdateComment(comment.Id, "Updated Comment", comment.UserId));
    }

    [Fact]
    public async Task GetCommentsForPost_PostNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockPostRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Post?)null); // Simulate post not found

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _commentService.GetCommentsForPost(1));
    }

    [Fact]
    public async Task DeleteComment_UserNotAuthorized_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var comment = new Comment { Id = 1, PostId = 1, UserId = 1, Text = "Test Comment", CreatedAt = DateTime.UtcNow };
        var unauthorizedUserId = 2; // Simulate a different user trying to delete
        _mockCommentRepo.Setup(repo => repo.GetByIdAsync(comment.Id)).ReturnsAsync(comment); // Simulate existing comment

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _commentService.DeleteComment(comment.Id, unauthorizedUserId));
    }
}
