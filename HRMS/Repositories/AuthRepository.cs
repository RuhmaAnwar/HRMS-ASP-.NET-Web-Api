
using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace HRMS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<Employee> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthRepository(UserManager<Employee> userManager, ApplicationDbContext context)
        {

            _userManager = userManager;
            _context = context;
        }

        public async Task<Employee> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Employee> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> ResetPasswordAsync(Employee user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Employee user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> AddToRolesAsync(Employee user, string[] roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(Employee user, string[] roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<string[]> GetRolesAsync(Employee user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToArray();
        }

        public async Task<string> GenerateJwtToken(Employee user)
        {
            var roles = await GetRolesAsync(user);

            // Retrieve the active signing key from the SigningKeys table
            var signingKey = _context.SigningKeys.FirstOrDefault(k => k.IsActive);
            if (signingKey == null)
            {
                throw new KeyNotFoundException("No active signing key available.");
            }

            // creating credentials to sign the token

            // Convert the Base64-encoded private key string back to a byte array
            var privateKeyBytes = Convert.FromBase64String(signingKey.PrivateKey);
            // Create a new RSA instance for cryptographic operations
            var rsa = RSA.Create();
            // Import the RSA private key into the RSA instance
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            // Create a new RsaSecurityKey using the RSA instance
            var rsaSecurityKey = new RsaSecurityKey(rsa)
            {
                // Assign the Key ID to link the JWT with the correct public key
                KeyId = signingKey.KeyId
            };
            // Define the signing credentials using the RSA security key and specifying the algorithm
            var creds = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);



            // Initialize a list of claims to include in the JWT
            var claims = new List<Claim>
            {
                // Subject (sub) claim with the user's ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // JWT ID (jti) claim with a unique identifier for the token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Name claim with the user's first name
                new Claim(ClaimTypes.Name, user.FirstName),
                // NameIdentifier claim with the user's email
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                // Email claim with the user's email
                new Claim(ClaimTypes.Email, user.Email)
            };
            // Iterate through the user's roles and add each as a Role claim
            foreach (var userRole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            // Define the JWT token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: "localhost", // The token issuer, typically your application's URL
                audience: "Profile", // The intended recipient of the token, typically the client's URL
                claims: claims, // The list of claims to include in the token
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds // The credentials used to sign the token
            );


            // Create a JWT token handler to serialize the token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Serialize the token to a string
            var token = tokenHandler.WriteToken(tokenDescriptor);
            return token;
        }
    }
}
