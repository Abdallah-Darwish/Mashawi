using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mashawi.Services;
using Mashawi.Db;

namespace Mashawi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly InitializationManager _initializationManager;
        public TestController(InitializationManager initializationManager)
        {
            _initializationManager = initializationManager;
        }

        [HttpGet("RecreateAndSeedDb")]
        public async Task<IActionResult> RecreateAndSeedDb()
        {
            await _initializationManager.RecreateAndSeedDb().ConfigureAwait(false);
            return Ok("Recreated and seeded successfully.");
        }

        [HttpGet("RecreateDb")]
        public async Task<IActionResult> RecreateDb()
        {
            await _initializationManager.RecreateDb().ConfigureAwait(false);
            return Ok("Recreated successfully.");
        }
    }
}