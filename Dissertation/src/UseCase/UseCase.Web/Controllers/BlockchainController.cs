using Microsoft.AspNetCore.Mvc;
using System;
using UseCase.NeoConnector;

namespace UseCase.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockchainController : ControllerBase
    {
        public BlockchainController()
        {
        }

        [HttpGet]
        [Route("get/transaction/{hash}")]
        public IActionResult GetUser(string hash)
        {
            try
            {
                var content = Connector.CallContractFunction("L3BWaAvXEiyFwfAbjU5otSKANPYfbwpX8eUS8W946y5xSgEY3Lwi", "getTransaction", hash, Connector.DerivePublicKey("L3BWaAvXEiyFwfAbjU5otSKANPYfbwpX8eUS8W946y5xSgEY3Lwi"));
                return Ok(content);
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}