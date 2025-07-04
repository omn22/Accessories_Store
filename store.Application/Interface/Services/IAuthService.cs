using Microsoft.AspNetCore.Identity;
using store.Application.DTOs;
using Store.Application.DTOs;
using Store.Application.OperationResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Application.Interface.Services
{
    public interface IAuthService
    {
        Task<OperationResult> RegisterAsync(RegisterUserDTO registerDTO);
        Task<OperationResult> ConfirmEmailAsync(string userId, string token);
        Task<OperationResult> LoginAsync (string Email, string password,bool RememberMe);
        Task<OperationResult> ForgetPassAsync(ForgetPasswordDTO forgetPasswordDTO);
        Task<OperationResult> ResetPassAsync(ResetPasswordDTO resetPasswordDTO);
    }
}
