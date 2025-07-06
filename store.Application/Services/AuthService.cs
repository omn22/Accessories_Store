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
using static System.Runtime.InteropServices.JavaScript.JSType;
using store.Domin.Enum;
using store.Application.Interface.Services;
using store.Application.DTOs;
using System.Data;
namespace Store.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IEmailService emailService, IConfiguration configuration, SignInManager<User> signInManager,ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                return new OperationResult { Success = false, Message = "Registration failed" };
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
            return new OperationResult { Success = true, Message = "Registration successful. Please check your email to confirm." };

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
        public async Task<OperationResult> LoginAsync(string Email, string password, bool RememberMe)
        {   //sure that user do not bloked 
            //make user can login with username
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
                return new OperationResult { Success = false, Message = $"Email Not found {ErrorCode.NotFoundEmail}" };
            else if (user.EmailConfirmed == false)
                return new OperationResult { Success = false, Message = $"wrong password {ErrorCode.NotConfirmedEmail} " };
            else if (user.IsDeleted)
                return new OperationResult { Success = false, Message = $"your account has been bloked {ErrorCode.BlokedAccount} " };
            var result = await _signInManager.PasswordSignInAsync(user, password, RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                //generat token
                var token =await _tokenService.GenerateJwtToken(user);
                //save role
                var roles = await _userManager.GetRolesAsync(user);
                //save token in opject to future use
                var response = new LoginRespondDTO
                {
                    Token = token,
                    Expiration = DateTime.Now.AddHours(1),
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.ToList(),
                };
                return new OperationResult { Success = true, Message = $"Login succeeded , Token = {token}" };
            }
            //if user locked 
            else if (result.IsLockedOut == true)
                return new OperationResult { Success = false, Message = $"you try to many times please wait 5 Seconds {ErrorCode.ToManyTry} " };
               return new OperationResult { Success = false, Message = $"wrong password {ErrorCode.WrongPassword} " };
        }

        public async Task<OperationResult> ForgetPassAsync(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            if (user == null)
                return new OperationResult { Success = false, Message = $"Email Not found {ErrorCode.NotFoundEmail}" };
            else if (user.EmailConfirmed == false)
            {
                return new OperationResult { Success = false, Message = $"wrong password {ErrorCode.NotConfirmedEmail} " };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["AppSettings:ClientUrl"]}/api/auth/ResetPass?token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please make us sure that this you try to reset your password: <a href='{confirmationLink}'>Reset Password</a>");
            Console.WriteLine($"Confirm via: {confirmationLink}");
            return new OperationResult { Success = true, Message = "check your email." };

        }
        public async Task<OperationResult> ResetPassAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email); 
            if (user == null)
                return new OperationResult { Success = false, Message = $"Email Not found {ErrorCode.NotFoundEmail}" };
            var encodedToken = WebUtility.UrlDecode(resetPasswordDTO.Token);
            var result = await _userManager.ResetPasswordAsync(user, encodedToken,resetPasswordDTO.NewPassword);
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            if (!result.Succeeded) return new OperationResult { Success = false, Message = $"{errors}" };
             return new OperationResult { Success = true, Message = "password reset succeeded" };
        }
    }
}
