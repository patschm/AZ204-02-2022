using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Controllers
{
    [ApiController]
    [Route("productgroups")]
    public class ProductGroupController: ControllerBase
    {
        private ILogger<ProductGroup> _logger;
        private IProductGroupRepository _repository;
        public ProductGroupController(IProductGroupRepository repository, ILogger<ProductGroup> logger)
        {
            _repository = repository;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetProductGroups")]
        public async Task<List<ProductGroup>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _repository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetProductGroup")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _repository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("products/{id}", Name = "GetRelatedProducts")]
        public async Task<List<Product>> GetProducts([FromRoute]int id, [FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _repository.GetProductsAsync(id);
            result = result.Skip(start).Take(count);
            return await result.ToListAsync();
        }
        [HttpPut("", Name = "PutProductGroup")]
        public async Task<IActionResult> Put([FromBody] ProductGroup item)
        {
            var result = await _repository.InsertAsync(item);
            if (result) return Created(Url.Action(nameof(Get)), new { id = item.ID});
            return NoContent();

        }
        [HttpPost("", Name = "PostProductGroup")]
        public async Task<IActionResult> Post([FromBody] ProductGroup item)
        {
            var result = await _repository.UpdateAsync(item);
            if (result) return Ok(item);
            return NotFound();
        }
        [HttpDelete("{id}", Name = "DeleteProductGroup")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result) return Accepted();
            return NotFound();
        }
    }
}