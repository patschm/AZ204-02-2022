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
        private IUnitOfWork  _unitOfWork;
        public ProductGroupController(IUnitOfWork uow, ILogger<ProductGroup> logger)
        {
            _unitOfWork = uow;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetProductGroups")]
        public async Task<List<ProductGroup>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _unitOfWork.ProductGroupRepository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetProductGroup")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _unitOfWork.ProductGroupRepository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("products/{id}", Name = "GetRelatedProducts")]
        public async Task<List<Product>> GetProducts([FromRoute]int id, [FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _unitOfWork.ProductGroupRepository.GetProductsAsync(id);
            result = result.Skip(start).Take(count);
            return await result.ToListAsync();
        }
        [HttpPut("", Name = "PutProductGroup")]
        public async Task<IActionResult> Put([FromBody] ProductGroup item)
        {
            var dbEnt = await _unitOfWork.ProductGroupRepository.GetAsync(item.ID);
            if (dbEnt == null) return NotFound();
            
            await _unitOfWork.ProductGroupRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
            return Accepted();
        }
        [HttpPost("", Name = "PostProductGroup")]
        public async Task<IActionResult> Post([FromBody] ProductGroup item)
        {
            await _unitOfWork.ProductGroupRepository.InsertAsync(item);
            await _unitOfWork.SaveAsync();
            item = await _unitOfWork.ProductGroupRepository.GetAsync(item.ID);
            return Created(Url.Action(nameof(Get)), new { id = item.ID });
        }
        [HttpDelete("{id}", Name = "DeleteProductGroup")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dbEnt = await _unitOfWork.ProductGroupRepository.GetAsync(id);
            if (dbEnt == null) return NotFound();
            await _unitOfWork.ProductGroupRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return Accepted();
        }
    }
}