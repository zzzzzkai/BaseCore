using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ITokenService : IBaseService<Tokens>
    {
        bool UpdataToken(Tokens token);
        bool DelToken(string refreshToken, string username);
        Tokens GetToken(string refreshToken, string username);
        bool SaveToken(Tokens token);
    }
}
