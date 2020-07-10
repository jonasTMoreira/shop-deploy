using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices]DataContext context
        )
        {   
            try
            {
                IList<User> users = 
                    await context.Users.AsNoTracking().ToListAsync();
            
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível buscar os usuários. Erro: " + e.Message });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody]User model,
            [FromServices]DataContext context
        )
        {
            try
            {
                if (model.Id != id)
                    return NotFound(new { Message = "Usuário não encontrado"});

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                context.Entry(User).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível criar o usuário. Erro: " + e.Message });
            }
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromBody]User model,
            [FromServices]DataContext context
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";

                return Ok(model);
            }
            catch (Exception e)
            {                
                return BadRequest(new { Message = "Não foi possível criar um usuário. Erro: " + e.Message });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody]User model,
            [FromServices]DataContext context
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                User user =
                    await context.Users
                            .AsNoTracking()
                            .Where(x => x.UserName == model.UserName && x.Password == model.Password)
                            .FirstOrDefaultAsync();
                
                if (user == null)
                    return NotFound(new { Message = "Usuário não encontrado ou usuário e/ou senha inválidos"});

                var token = TokenService.GenerateToken(user);
                user.Password = "";

                return new {
                    user = user,
                    token = token
                };
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Não foi possível realizar o login. Erro: " + e.Message });
            }
        }
    }
}