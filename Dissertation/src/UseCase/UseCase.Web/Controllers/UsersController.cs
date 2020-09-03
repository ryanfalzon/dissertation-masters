using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.Web.Models;

namespace UseCase.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersQueryService _usersQueryService;
        private readonly IUsersCommandService _usersCommandService;
        private readonly IMapper _mapper;

        public UsersController(IUsersQueryService usersQueryService, IUsersCommandService usersCommandService, IMapper mapper)
        {
            _usersQueryService = usersQueryService ?? throw new ArgumentNullException(nameof(usersQueryService));
            _usersCommandService = usersCommandService ?? throw new ArgumentNullException(nameof(usersCommandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Route("get/{publicKey}")]
        public IActionResult GetUser(string publicKey)
        {
            try
            {
                var user = _usersQueryService.GetUser(publicKey);
                return Ok(_mapper.Map<UserViewModel>(user));
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("post")]
        public IActionResult PostUser(UserViewModel user)
        {
            try
            {
                _usersCommandService.InsertUser(_mapper.Map<User>(user));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut]
        [Route("update")]
        public IActionResult UpdateUser(UserViewModel user)
        {
            try
            {
                _usersCommandService.UpdateUser(_mapper.Map<User>(user));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}