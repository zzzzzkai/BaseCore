using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.IService
{
    public interface IOtherBillService : IBaseService<OtherBill>
    {
       List<OtherBill> ImportBill();
    }
}
