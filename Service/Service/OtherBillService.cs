using DataModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service.Service
{
    public class OtherBillService : BaseService<OtherBill>, IOtherBillService
    {
        private readonly IOtherBillRepository _otherBillRepository;
       public OtherBillService(IOtherBillRepository otherBillRepository) {

            _otherBillRepository = otherBillRepository;
            base._baseRepository = _otherBillRepository;
        }
       
        /// <summary>
        /// 获取excel版本
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public IWorkbook Getversion(string ext,StreamReader stream) {
            if (ext == "xls")
            {
                return new HSSFWorkbook(stream.BaseStream);
            }
            else
            {
                return new XSSFWorkbook(stream.BaseStream);
            }
           
        }
    }
}
