using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.NeoConnector;
using UseCase.Web.Helpers;
using UseCase.Web.Models;

namespace UseCase.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsQueryService _postsQueryService;
        private readonly IPostsCommandService _postsCommandService;
        private readonly IMapper _mapper;

        public PostsController(IPostsQueryService postsQueryService, IPostsCommandService postsCommandService, IMapper mapper)
        {
            _postsQueryService = postsQueryService ?? throw new ArgumentNullException(nameof(postsQueryService));
            _postsCommandService = postsCommandService ?? throw new ArgumentNullException(nameof(postsCommandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetPost(int id)
        {
            try
            {
                var post = _postsQueryService.GetPost(id);
                return Ok(_mapper.Map<PostViewModel>(post));
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetPosts(int offset)
        {
            try
            {
                var posts = _postsQueryService.GetPosts(offset);
                return Ok(_mapper.Map<IEnumerable<Post>>(posts));
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("submit")]
        public async Task<IActionResult> SubmitPost(PostViewModel post)
        {
            try
            {
                post.Hash = BlockchainTools.HashObject(post).ToHexString();
                _postsCommandService.InsertPost(_mapper.Map<Post>(post));

                string transaction = $"User with ID {post.UserId} added a new post";
                await Connector.BroadcastTransaction("L3BWaAvXEiyFwfAbjU5otSKANPYfbwpX8eUS8W946y5xSgEY3Lwi", "addTransaction", BlockchainTools.HashObject(transaction), transaction, Connector.DerivePublicKey("L3BWaAvXEiyFwfAbjU5otSKANPYfbwpX8eUS8W946y5xSgEY3Lwi"));
                
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut]
        [Route("update")]
        public IActionResult UpdatePost(PostViewModel post)
        {
            try
            {
                post.Hash = BlockchainTools.HashObject(post).ToHexString();
                _postsCommandService.UpdatePost(_mapper.Map<Post>(post));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut]
        [Route("like")]
        public IActionResult LikePost(int id)
        {
            try
            {
                _postsCommandService.LikePost(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult DeletePost(int id)
        {
            try
            {
                _postsCommandService.DeletePost(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}