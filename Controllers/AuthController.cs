using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;
using kzy_entities.Entities;
using UnitOfWorkDemo1.Interfaces;
using UnitOfWorkDemo.Repositories;

namespace UnitOfWorkDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IAuthService authService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Use UnitOfWork to handle database operations
            var username = user.Username;
            var existingUser = await _unitOfWork.GetRepository<User>().Query(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var newUser = await _authService.Register(user.Username, user.Password);
            await _unitOfWork.GGWPChangesAsync();
            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var token = await _authService.Authenticate(user.Username, user.Password);
            if (token == null)
                return Unauthorized();

            return Ok(new { Token = token });
        }
    }
}
