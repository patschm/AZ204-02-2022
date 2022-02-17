using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Controllers
{
    [ApiController]
    [Route("brands")]
    public class BrandController: ControllerBase
    {
        private ILogger<BrandController> _logger;
        private IBrandRepository _repository;
        public BrandController(IBrandRepository repository, ILogger<BrandController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetBrands")]
        public async Task<List<Brand>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _repository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetBrand")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _repository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPut("", Name = "PutBrand")]
        public async Task<IActionResult> Put([FromBody] Brand item)
        {
            var result = await _repository.InsertAsync(item);
            if (result) return Created(Url.Action(nameof(Get)), new { id = item.ID});
            return NoContent();

        }
        [HttpPost("", Name = "PostBrand")]
        public async Task<IActionResult> Post([FromBody] Brand item)
        {
            var result = await _repository.UpdateAsync(item);
            if (result) return Ok(item);
            return NotFound();
        }
        [HttpDelete("{id}", Name = "DeleteBrand")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result) return Accepted();
            return NotFound();
        }
    }
}