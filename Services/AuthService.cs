using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo1.Interfaces;
using kzy_entities.Entities;

namespace UnitOfWorkDemo.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            //_unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        //public async Task<string> Authenticate(string username, string password)
        //{
        //    // Assuming passwords are stored as plain text for this example, but they should be hashed
        //    var user = await _unitOfWork.GetRepository<User>().Query(x => x.Username == username && x.Password == password).FirstOrDefaultAsync();
        //    if (user == null)
        //        return null;

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        //    var issuer = _configuration["Jwt:Issuer"];
        //    var audience = _configuration["Jwt:Audience"];

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, username)
        //            // Add more claims as needed
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1), // Token expiration
        //        Issuer = issuer,
        //        Audience = audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

        //public async Task<User> Register(string username, string password)
        //{
        //    // Hash the password before storing it
        //    var hashedPassword = HashPassword(password);

        //    var user = new User { Username = username, Password = hashedPassword };
        //    await _unitOfWork.GetRepository<User>().AddAsync(user);
        //    await _unitOfWork.GGWPChangesAsync();
        //    return user;
        //}

        //private string HashPassword(string password)
        //{
        //    // Implement your password hashing logic here
        //    // For example, you can use BCrypt, SHA256, etc.
        //    // Here, just return the password as a placeholder
        //    return password; // Replace with actual hashing logic
        //}
    }
}
