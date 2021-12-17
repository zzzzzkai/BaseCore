using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
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
        public List<OtherBill> ImportBill()
        {

            try
            {
                
                return null;
            }
            catch (Exception e)
            {

                throw e ; 
            }
        }
    }
}
