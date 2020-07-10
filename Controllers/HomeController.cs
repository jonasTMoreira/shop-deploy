using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        public async Task<ActionResult<dynamic>> Get(
            [FromServices]DataContext context
        )
        {
            User employee = new User { Id = 1, UserName = "Robin", Password = "Robin", Role = "employee" };
            User manager = new User { Id = 2, UserName = "Batman", Password = "Batman", Role = "manager" };
            Category category = new Category { Id = 1, Title = "Informática" };
            Product product = new Product { Id = 1, Category = category, Title = "Mouse", Price = 299, Description = "Mouse Gamer"};

            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Dados iniciais configurados. Ainda não subimos o BD para o azure" });
        }
    }
}