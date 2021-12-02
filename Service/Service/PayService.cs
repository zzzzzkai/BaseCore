using DataModel;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Text;
using Repository.IRepository;

namespace Service.Service
{
    public class PayService : BaseService<PayRecord>, IPayService
    {
        //private readonly IPayRepository _payRepository;

        //public PayService(IPayRepository payRepository)
        //{
        //    _payRepository = payRepository;
        //    _baseRepository = _payRepository;

        //}
    }
}
