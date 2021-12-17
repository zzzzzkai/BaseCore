using DataModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service.IService
{
    public interface IOtherBillService : IBaseService<OtherBill>
    {
        IWorkbook Getversion(string ext, StreamReader stream);
    }
}
