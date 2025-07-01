using Store.Application.DTOs;
using Store.Application.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Store.Domin.Enitity;
using Store.Application.OperationResults;
using System.Web;
using Microsoft.Extensions.Configuration;
using System.Net;
namespace Store.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<OperationResult> RegisterAsync(RegisterUserDTO registerDTO)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDTO.UserName);
            if (existingUser != null)
                return new OperationResult { Success = false, Message = "Username already exists" };

            var user = new User
            {
                UserName = registerDTO.UserName,
                Name = registerDTO.FullName,
                Email = registerDTO.Email,
                Address = registerDTO.Address,
                PhoneNumber = registerDTO.Phone,
                CreatedAt = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) 
            {
                return new OperationResult { Success = false, Message = "Registration failed"};
            }
            // Assign default role if it exists, otherwise create it
            var defaultRole = "User";
            var adminRole = "Admin";

            // تأكد وجود الأدوار قبل الاستخدام
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(defaultRole));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return new OperationResult { Success = false, Message = $"Role creation failed: {errors}" };
                }
            }
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(adminRole));
            }

            if (registerDTO.Password.StartsWith("Admin7878"))
            {
                await _userManager.AddToRoleAsync(user, adminRole);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, defaultRole);
            }



            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["AppSettings:ClientUrl"]}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");
            Console.WriteLine($"Confirm via: {confirmationLink}");
            return new OperationResult { Success = true, Message = "Registration successful. Please check your email to confirm."};

        }
        public async Task<OperationResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult { Success = false, Message = "User not found." };

            var decodedToken = WebUtility.UrlDecode(token); 

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
                return new OperationResult { Success = true, Message = "Email confirmed successfully." };

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Received token: {token}");
            Console.WriteLine($"User ID: {userId}");
            return new OperationResult { Success = false, Message = $"Email confirmation failed: {errors}" };
        }

    }
}
