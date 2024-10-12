﻿using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using SocialNetwork.API.DTO.Post;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.Post
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("posts")]
    [ApiExplorerSettings(GroupName = "User")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        // GET: posts
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] Guid? postId, [FromQuery] Guid? authorId, [FromQuery] Guid? groupId, [FromQuery] string? content)
        {
            var (posts, error) = await _postService.GetPostsAsync(postId, authorId, groupId, content);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = posts.Select(post => new PostResponse
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                GroupId = post.GroupId,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: posts
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (post, error) = await _postService.CreatePostAsync(userId.Value, request.Content, request.GroupId);
            if (!string.IsNullOrEmpty(error) || post == null) return BadRequest(new { Error = error });

            var response = new PostResponse
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                GroupId = post.GroupId,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPosts), new { postId = post.Id }, response);
        }

        // PUT: posts/{postId}
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedPost, error) = await _postService.UpdatePostAsync(postId, userId.Value, request.Content);
            if (!string.IsNullOrEmpty(error) || updatedPost == null) return BadRequest(new { Error = error });

            var response = new PostResponse
            {
                Id = updatedPost.Id,
                AuthorId = updatedPost.AuthorId,
                GroupId = updatedPost.GroupId,
                Content = updatedPost.Content,
                CreatedAt = updatedPost.CreatedAt,
                UpdatedAt = updatedPost.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: posts/{postId}
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _postService.DeletePostAsync(postId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}
