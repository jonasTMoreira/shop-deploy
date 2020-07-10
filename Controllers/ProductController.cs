using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductControllers : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices]DataContext context
        )
        {
            try
            {
                IList<Product> products = 
                    await context.Products.Include(prod => prod.Category).AsNoTracking().ToListAsync();
                
                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível carregar produtos. Erro: " + e.Message });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices]DataContext context
        )        
        {
            try
            {
                Product product = 
                    await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                
                if (product == null)
                    return NotFound(new { Message = "Produto não encontrado" });

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível encontrar o produto. Erro: " + e.Message });
            }
        }

        [HttpGet]
        [Route("categories/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int categoryId,
            [FromServices]DataContext context
        )
        {
            try
            {
                IList<Product> products = 
                    await context.Products.Include(x => x.Category).AsNoTracking()
                            .Where(x => x.Category.Id == categoryId).ToListAsync();
                
                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível buscar os produtos pela categoria. Erro: " + e.Message });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromBody]Product model,
            [FromServices]DataContext context
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                context.Products.Add(model);
                await context.SaveChangesAsync();
                
                return Ok(model);                    
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não possível salvar o produto. Erro: " + e.Message });
            }            
        }
    }
}