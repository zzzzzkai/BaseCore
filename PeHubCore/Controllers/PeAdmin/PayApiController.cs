using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.IRepository;
using Service.IService;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayApiController : BaseApiController
    {
        private readonly IPayRepository _IPayService;

        public PayApiController(IPayRepository IPayService)
        {
            _IPayService = IPayService;
        }

        /// <summary>
        ///  获取缴费
        /// </summary>
        [HttpPost("Getpayy")]
        public async Task<IActionResult> Getpayy() {
            try
            {
                var list = await Task.Factory.StartNew(() =>
                    {
                        return _IPayService.FindAll().ToList().Select(x => new
                        {
                            x?.id,
                            name=x?.name ?? "",
                            x?.openid,
                            clus_Name= x?.clus_Name ?? "",
                            x?.regno,
                            pay_flag = x?.pay_flag == "T" ? "缴费成功" : "缴费失败",
                            x?.price,
                            transaction_id=x?.transaction_id ?? "",
                            create_time = x?.create_time.ToString("yyyy-MM-dd"),
                            pay_time = x.pay_time==null?"":Convert.ToDateTime(x?.pay_time).ToString("yyyy-MM-dd"),
                            out_trade_no=  x?.out_trade_no??"",
                            refund_time = x.refund_time==null?"": Convert.ToDateTime(x.refund_time).ToString("yyyy-MM-dd"),
                            x?.out_refund_no,
                            x?.order_type,
                            update_Time =x.update_Time==null?"": Convert.ToDateTime(x?.update_Time).ToString("yyyy-MM-dd")??"",
                            pay_errmsg= x?.pay_errmsg??""
                        }).ToList();
                    });
              
                result.success = true;
                result.returnData = list;
                return Ok(result);

            }
            catch (Exception ex)
            {
                result.success = false;
                result.returnMsg = "获取缴费订单失败，" + ex.Message;
                return Ok(result);


            }




        }


    }
}