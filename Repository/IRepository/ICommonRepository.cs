using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICommonRepository :IBaseRepository<UserInfo>
    {
        bool InitDB();
    }
}
