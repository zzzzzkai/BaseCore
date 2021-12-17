using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.IService;
using ServiceExt;
using WebServiceHub;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackProtectApiController : BaseApiController
    {
        private readonly IPersonSumService _personSumService;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));

        public BackProtectApiController(IPersonSumService personSumService) 
        {
            _personSumService = personSumService;
        }

        #region 表格
        //[HttpGet("GetTable")]
        //public FileResult GetTable()
        //{
        //    //查询出列表
        //    List<Order> list = _orderService.FindAll().ToList();

        //    List<Dictionary<string, string>> LD = new List<Dictionary<string, string>>();
        //    Dictionary<string, string> LDItem = new Dictionary<string, string>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        LDItem = new Dictionary<string, string>();


        //        if (list[i].type == "person")
        //        {
        //            LDItem.Add("姓名", list[i].name);
        //            string sex = list[i].idCard.Substring(14, 3);
        //            if (int.Parse(sex) % 2 == 0)
        //            {
        //                LDItem.Add("性别", "女");
        //            }
        //            else
        //            {
        //                LDItem.Add("性别", "男");
        //            }

        //            //判断年龄
        //            var birthCard = list[i].idCard;
        //            var yearBirth = birthCard.Substring(6, 4);
        //            var monthBirth = birthCard.Substring(10, 2);
        //            var dayBirth = birthCard.Substring(12, 2);
        //            //获取当前年月日并计算年龄
        //            var myDate = DateTime.Now;
        //            var age = DateTime.Now.Year - int.Parse(yearBirth);
        //            if (myDate.Month < int.Parse(monthBirth) || (myDate.Month == int.Parse(monthBirth) && myDate.Day < int.Parse(dayBirth)))
        //            {
        //                age--;
        //            }
        //            //得到年龄
        //            string Age = age.ToString();


        //            LDItem.Add("年龄", Age);
        //            LDItem.Add("联系电话", list[i].tel);
        //            LDItem.Add("单位编码", "无");
        //            LDItem.Add("单位名称", "无");
        //            LDItem.Add("套餐名称", list[i].clus_Name);
        //            LDItem.Add("身份证号", list[i].idCard);
        //            LDItem.Add("预约时间", list[i].begin_Time.ToString("yyyy-MM-dd"));
        //            LDItem.Add("体检类型", "个人体检");
        //        }
        //        else if (list[i].type == "group")
        //        {
        //            LDItem.Add("姓名", list[i].name);
        //            string sex = list[i].idCard.Substring(14, 3);
        //            if (int.Parse(sex) % 2 == 0)
        //            {
        //                LDItem.Add("性别", "女");
        //            }
        //            else
        //            {
        //                LDItem.Add("性别", "男");
        //            }
        //            LDItem.Add("年龄", "");
        //            LDItem.Add("联系电话", list[i].tel);
        //            LDItem.Add("单位编码", list[i].lnc_Code);
        //            LDItem.Add("单位名称", list[i].company_Name);
        //            LDItem.Add("套餐名称", list[i].clus_Name);
        //            LDItem.Add("身份证号", list[i].idCard);
        //            LDItem.Add("预约时间", list[i].begin_Time.ToString("yyyy-MM-dd"));
        //            LDItem.Add("体检类型", "团队体检");
        //        }
        //        LD.Add(LDItem);
        //    }

        //    DataTable outData = ExportHelper.ToDataTableOther(LD);


        //    MemoryStream ms = ExportHelper.RenderDataTableToExcel(outData) as MemoryStream;


        //    return File(ms, "application/octet-stream", "普通健康检查名单表.xls");
        //}

        #endregion
    }
}