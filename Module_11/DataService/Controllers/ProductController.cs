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
        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork uow, ILogger<Product> logger)
        {
            _unitOfWork = uow;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetProducts")]
        public async Task<List<Product>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            var result = await _unitOfWork.ProductRepository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _unitOfWork.ProductRepository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPut("", Name = "PutProduct")]
        public async Task<IActionResult> Put([FromBody] Product item)
        {
            var dbEnt = await _unitOfWork.ProductRepository.GetAsync(item.ID);
            if (dbEnt == null) return NotFound();
            await _unitOfWork.ProductRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
             return Ok(item);
        }
        [HttpPost("", Name = "PostProduct")]
        public async Task<IActionResult> Post([FromBody] Product item)
        {
            await _unitOfWork.ProductRepository.InsertAsync(item);
            await _unitOfWork.SaveAsync();
            item = await _unitOfWork.ProductRepository.GetAsync(item.ID);
            return Created(Url.Action(nameof(Get)), new { id = item.ID });
           
        }
        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dbEnt = await _unitOfWork.ProductRepository.GetAsync(id);
            if (dbEnt == null) return NotFound();
            await _unitOfWork.ProductRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
           return Accepted();
        }
    }
}