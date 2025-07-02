using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using store.Application.Interface.Services;
using Store.Domin.Enitity;

namespace store.Application.Services
{
    public class GenerateJWT : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        public GenerateJWT(IConfiguration configuration,UserManager<User> userManager)
        {
            _config = configuration;
            _userManager = userManager;
        }
        /// <summary>
        /// الدالة دي بتولّد JSON Web Token (JWT) للمستخدم بعد تسجيل الدخول.
        /// الـ JWT ده بيحتوي على معلومات (Claims) عن المستخدم، وبيكون مُوقَّع
        /// بمفتاح سري علشان السيرفر يقدر يتحقق إنه أصلي ومفيش حد عدّله.
        /// </summary>
        public async Task<string> GenerateJwtToken(User user)
        {
            // 1) نجيب الأدوار (Roles) اللي المستخدم منتمي ليها من قاعدة البيانات.
            //    هتستخدم بعد شوية كـ Claims علشان نعرف الصلاحيات في الـ API.
            var roles = await _userManager.GetRolesAsync(user);

            // 2) نكوّن "قائمة Claims" وهي عبارة عن بيانات هتتغلف جوه التوكن.
            //    كل Claim عبارة عن مفتاح (النوع) وقيمة.
            var claims = new List<Claim>
            {
                 // ▪️ "Sub" = Subject = البريد الإلكتروني للمستخدم (هوية التوكن الأساسية).
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),

                // ▪️ "Jti" = JSON Token Identifier = رقم عشوائي فريد للتوكن (يمنع التكرار).
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        
                // ▪️ NameIdentifier = الـ Id بتاع المستخدم (بنحوّله لـ string).
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // 3) نضيف كل دور Role كم Claim من نوع "Role".
            //    كده أي Controller عليه [Authorize(Roles = "Admin")] هيقدر يتحقق.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 4) ننشئ المفتاح السري (Secret Key) من الإعدادات في appsettings.json:
            //    "Jwt": { "Key": "YourSuperSecretKey" }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            // 5) نحدد خوارزمية التوقيع (هنا HMAC‑SHA256) بالمفتاح اللي فوق.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 6) نبني الـ Jwt نفسه:
            //    ▪️ issuer   = الجهة المصدِرة للتوكن (عادةً الـ Domain بتاعنا).
            //    ▪️ audience = المستلم المتوقع للتوكن (الفرونت‑إند أو الموبايل).
            //    ▪️ claims   = البيانات اللي حطيناها فوق.
            //    ▪️ expires  = مدة صلاحية التوكن (هنا ساعة واحدة).
            //    ▪️ signingCredentials = التوقيع اللي بيضمن سلامة التوكن.
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            // 7) نحول كائن الـ JwtSecurityToken لسلسلة نصية (string) قابلة للإرسال.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
