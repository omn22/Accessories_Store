using Microsoft.AspNetCore.Identity;
using Store.Domin.Enitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace store.Application.Interface.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
    }
}
