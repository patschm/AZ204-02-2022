using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork uow, ILogger<BrandController> logger)
        {
            _unitOfWork = uow;
            _logger = logger;
        }
    
        [HttpGet("", Name = "GetBrands")]
        public async Task<List<Brand>> GetAll([FromQuery] int start = 0, [FromQuery] int count = 10)
        {
            _logger.LogError("Aha! U vroeg om all merken!!!");
            var result = await _unitOfWork.BrandRepository.GetAllAsync(start, count);
            return await result.ToListAsync();
        }
        [HttpGet("{id}", Name = "GetBrand")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await _unitOfWork.BrandRepository.GetAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPut("", Name = "PutBrand")]
        public async Task<IActionResult> Put([FromBody] Brand item)
        {
            var dbEnt = await _unitOfWork.BrandRepository.GetAsync(item.ID);
            if (dbEnt == null) return NotFound();
            await _unitOfWork.BrandRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
            return Accepted();
        }
        [HttpPost("", Name = "PostBrand")]
        public async Task<IActionResult> Post([FromBody] Brand item)
        {
            await _unitOfWork.BrandRepository.InsertAsync(item);
            await _unitOfWork.SaveAsync();
            item = await _unitOfWork.BrandRepository.GetAsync(item.ID);
            return Created(Url.Action(nameof(Get)), new { id = item.ID});
        }
        [HttpDelete("{id}", Name = "DeleteBrand")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dbEnt = await _unitOfWork.BrandRepository.GetAsync(id);
            if (dbEnt == null) return NotFound();
            await _unitOfWork.BrandRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return Accepted();
        }
    }
}