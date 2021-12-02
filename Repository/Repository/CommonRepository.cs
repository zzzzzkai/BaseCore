using DataModel;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class CommonRepository : BaseRepository<UserInfo>, ICommonRepository
    {
        public bool InitDB()
        {
            try
            {
                var tarr = Assembly.Load("DataModel").GetTypes().Where(x => x.Namespace == "DataModel").ToArray();
 
                _db.CodeFirst.InitTables(tarr);//执行完数据库就有这个表了
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
