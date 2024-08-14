using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy ="CanRead")] //This CanRead Policy defined in Program.cs with "read" scope.
    public class ApiController : ControllerBase
    {
    }
}
