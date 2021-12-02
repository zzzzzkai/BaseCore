using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
   public class WebApiFuncService:BaseService<WebApiFunc>, IWebApiFuncService
    {
        #region 仓储
        private readonly IWebApiFuncRepository _webApiFuncRepository;


        #endregion
        #region 构造函数注入
        public WebApiFuncService(IWebApiFuncRepository webApiFuncRepository)
        {
            _webApiFuncRepository = webApiFuncRepository;

            //必须初始化
            this._baseRepository = _webApiFuncRepository;
        }
        #endregion
    }
}
