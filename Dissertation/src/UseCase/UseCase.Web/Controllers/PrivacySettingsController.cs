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
    public class PrivacySettingsController : ControllerBase
    {
        private readonly IPrivacySettingsQueryService _privacySettingsQueryService;
        private readonly IPrivacySettingsCommandService _privacySettingsCommandService;
        private readonly IMapper _mapper;

        public PrivacySettingsController(IPrivacySettingsQueryService privacySettingsQueryService, IPrivacySettingsCommandService privacySettingsCommandService, IMapper mapper)
        {
            _privacySettingsQueryService = privacySettingsQueryService ?? throw new ArgumentNullException(nameof(privacySettingsQueryService));
            _privacySettingsCommandService = privacySettingsCommandService ?? throw new ArgumentNullException(nameof(privacySettingsCommandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Route("get/{userId}")]
        public IActionResult GetPost(int userId)
        {
            try
            {
                var privacySettings = _privacySettingsQueryService.GetPrivacySettings(userId);
                return Ok(_mapper.Map<PrivacySettingsViewModel>(privacySettings));
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut]
        [Route("update")]
        public IActionResult UpdatePost(PrivacySettingsViewModel privacySettings)
        {
            try
            {
                _privacySettingsCommandService.UpdatePrivacySettings(_mapper.Map<PrivacySettings>(privacySettings));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}