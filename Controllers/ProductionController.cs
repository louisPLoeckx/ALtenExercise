using ALtenExercise.Models;
using ALtenExercise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ALtenExercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : Controller
    {
        private readonly ILogger<ProductionController> _logger;

        private Repo _repo;
        decimal efficiency = 0.0m;
        List<Response> responses = new List<Response>();
        List<PowerPlant> powerPlants = new List<PowerPlant>();

        public ProductionController(Repo repo, ILogger<ProductionController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("ProductionPlan")]
        public IActionResult ProductionPlan([Microsoft.AspNetCore.Mvc.FromBody] LoadInput loadInput)
        {
            powerPlants = loadInput.PowerPlants;
            
            responses = _repo.LoadDistribution(loadInput, efficiency, powerPlants.Count);

            return Ok(responses);
        }

    }
}
