using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ICommonService : IBaseService<Tokens>
    {
        bool VerifyCode(string phone, string code);
        bool SendVerifyCode(string phoneNumber, ref string msg);
        bool InitDB();
        dynamic getAnyDate(string sql, object para);

        Dictionary<string, string> getUserInfoDetail(string idCard);
    }
}
