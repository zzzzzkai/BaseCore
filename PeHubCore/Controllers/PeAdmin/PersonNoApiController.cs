
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonNoApiController : BaseApiController
    {
        private readonly IPersonSumService _sumTimeService;

        public PersonNoApiController(IPersonSumService sumTimeService)
        {
            _sumTimeService = sumTimeService;
        }
        /// <summary>
        /// 获取设置个检号源(GetBooknosum)
        /// </summary>
        /// <param name="pdata"></param>
        /// <returns></returns>


        /// <summary>
        /// 获取一个月的号源(GetMonthofDay)
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>


        /// <summary>
        /// 个检号源修改(UpdateBooknoTotalTTj)
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
       


    }
}