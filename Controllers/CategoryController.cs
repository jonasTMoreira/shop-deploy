using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices]DataContext context
        )
        {
            try
            {
                IList<Category> categories = await context.Categories.AsNoTracking().ToListAsync();
                return Ok(categories);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível encontrar as categorias. Erro: " + e.Message});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices]DataContext context
        )
        {
            try
            {
                Category model = await context.Categories.AsNoTracking().FirstOrDefaultAsync(cat => cat.Id == id);
                if (model == null)
                    return NotFound(new { Message = "Categoria não encontrada"});

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível encontrar a categoria. Erro: " + e.Message });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);    
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível criar a categoria. Erro: " + e.Message });
            }            
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody]Category model,
            [FromServices]DataContext context    
        )
        {
            try
            {
                if (model.Id != id)
                    return NotFound(new { message = "Categoria não encontrada"});

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { Message = "Não foi possível atualizar a categoria. Outro usuário está modificando este registro"});
            }            
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não possível atualizar a categoria. Erro: " + e.Message});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices]DataContext context
        )
        {
            try
            {
                Category model = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);
                if (model == null)
                    return NotFound(new { message = "Categoria não encontrada"});
                
                context.Categories.Remove(model);
                await context.SaveChangesAsync();
                return Ok(new { Message = $"Categoria {model.Title} removida com sucesso"});                                
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível remover a categoria. Erro: " + e.Message});
            }
        }
    }
}