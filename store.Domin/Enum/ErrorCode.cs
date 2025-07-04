using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace store.Domin.Enum
{
    public enum ErrorCode
    {
        NotFoundEmail=100,
        WrongPassword,
        NotConfirmedEmail,
        ToManyTry,
        CanNotResetPassword,
        CanNotLogIn
    }
}
