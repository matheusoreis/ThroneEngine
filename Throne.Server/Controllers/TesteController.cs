using Microsoft.AspNetCore.Mvc;

namespace Throne.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public bool Get()
        {
            return true;
        }
    }
}
