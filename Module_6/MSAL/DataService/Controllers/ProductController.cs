using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController: ControllerBase
    {
        private ILogger<Product> _logger;
        private IProductRepository _repository;
        public ProductController(IProductRepository repository, ILogger<Product> logger)
        {
            _repository = repository;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetProducts")]
        public async Task<List<Product>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _repository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _repository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPut("", Name = "PutProduct")]
        public async Task<IActionResult> Put([FromBody] Product item)
        {
            var result = await _repository.InsertAsync(item);
            if (result) return Created(Url.Action(nameof(Get)), new { id = item.ID});
            return NoContent();

        }
        [HttpPost("", Name = "PostProduct")]
        public async Task<IActionResult> Post([FromBody] Product item)
        {
            var result = await _repository.UpdateAsync(item);
            if (result) return Ok(item);
            return NotFound();
        }
        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result) return Accepted();
            return NotFound();
        }
    }
}