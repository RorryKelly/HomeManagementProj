using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HomeManagement
{
    public class JWTAuthenticationManager
    {
        public static string Authenticate(int id, string username, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes("k1IWRE5DE0tX1OBlmPhBQWc7OJvWHzx3AOEBEdOuolO9gKYNT0zIvwdNI1hynmFw1vfiPeOnVUrfhSMQ3hNxe2kgVgKm3paSkqfbWNE9vgcxA7PNP9WyFWzLnc5x1oerNnkLFTTexrlC7rsghOTNanxeNcZz2ajZNTXZHU0VoCGagcfjADR01xsAnWMXUdw5Qk591o1r");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("Id", id.ToString()),
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
