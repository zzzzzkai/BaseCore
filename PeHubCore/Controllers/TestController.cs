using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.ClusModel;
using DataModel.Other;
using IdentityModel.Client;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Service.IService;
using ServiceExt;

namespace PeHubCore.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ApiDataFilter]
    public class TestController : BaseApiController
    {
        private readonly IH_AdminService _H_AdminService;
        private IWebHostEnvironment _hostEnvironment;
        public TestController(IH_AdminService H_AdminService,ICommonService commonService, IMemoryCache _cache, IWebHostEnvironment hostEnvironment)
        {
            _H_AdminService = H_AdminService;
            _commonService = commonService;
            _cacheService = _cache;
            _hostEnvironment = hostEnvironment;
        }
        /// <summary>
        /// 测试 vUvh7AGbD9H5rM5MEh2YyLOBu3Oa8UmFH7l/mOAv1tc=
        /// </summary>
        /// <param name="pdata"></param>
        /// <returns></returns>
        [HttpPost("t001")]
        public IEnumerable<WeatherForecast> t001([FromBody]encryData pdata)
        {
            //var stt = JsonConvert.SerializeObject(new { code = "mic", name = "myname" });
            //var sss= SecurityHelper.AESEncrypt(stt, "0123456789abcdef");
            var ss = new MemoryCaching(_cacheService);

            if (ss.Get("hao") == null)
            {
                ss.Set("hao", "dsdsa");
            }

            if (string.IsNullOrEmpty(getSession<string>("openid")))
            {
                setSession("openid", "555566");

            }

            log.Info("testinfo");
            log.Error("testerr");
            log.Debug("testerr");
            log.Warn("testerr");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = pdata.data.code
            })
            .ToArray();
        }

        [HttpGet("logTest")]
        public async Task<IActionResult> logTest(int type = 0)
        {
            ResponsResult model = new ResponsResult();
            var rdata= _H_AdminService.FindAll();
            string msg = "";
            _commonService.SendVerifyCode("155211",ref msg);
            Random n = new Random();

            model.returnData = rdata;
                //Task.Factory.StartNew(() => new { msg = "测试日志信息", time = DateTime.Now.ToString(), rand = n.Next(1000) });
            if (type == 1)
            {
                throw new System.Exception("异常测试!" + n.Next(1000));
            }
            return Ok(model);
        }

        [HttpPost("CancelTest")]
        public async Task<IActionResult> CancelTest()
        {
            var dd = DateTime.Now;
            return Ok();    
        }
        /// <summary>
        /// 前端测试数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("DataListTest")]
        public async Task<IActionResult> DataListTest(string code) {
            return Ok(result);
        }
 
        /// <summary>
        /// 获取测试token
        /// </summary>
        /// <param name="client_id"></param>
        /// <param name="client_secret"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken(string client_id,string client_secret,string username, string password)
        {
            var client = new HttpClient();

            try
            {
                // request token
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = Appsettings.GetSectionValue("AppSettings:IdSHttpsUrl") + "/connect/token",
                    ClientId = client_id,
                    ClientSecret = client_secret,
                    UserName = username,
                    Password = password,
                    Scope = "api1"
                });

                if (tokenResponse.IsError)
                {
                    return Ok(tokenResponse.Error);
                }
                return Ok(tokenResponse.Raw);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 图片文件上传
        /// </summary>
        /// <param name="files">文件接收类型</param>
        /// <returns></returns>
        [HttpPost("imgesFormFile")]
        public async Task<IActionResult> imgesFormFile(List<IFormFile> files)
        {
            try
            {
                var Filedata = Request.Form.Files[0];//获取文件参数                                                   
                List<string> filenames = new List<string>();//自定义参数
                DateTime now = DateTime.Now;//定义时间
                //文件存储路径
                string webRootPath = _hostEnvironment.WebRootPath; //存放系统 wwwroot 文件夹
                string uploadPath = Path.Combine("images", DateTime.Now.ToString("yyyyMMdd"));//文件目录
                string dirPath = Path.Combine(webRootPath, uploadPath);//指定当前图片保存目录

                ////判断是否已存在相同文件夹：是跳过否就创建
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //文件后缀
                string fileExtension = Path.GetExtension(Filedata.FileName);

                //判断后缀是否是图片
                const string fileFilt = ".gif|.jpg|.jpeg|.png";
                if (fileExtension == null)
                {
                    result.success = false;
                    result.returnMsg = "请选择正确的头像证件相片";
                    return Ok(result);
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    result.success = false;
                    result.returnMsg = "请上传正确格式的图片（jpg、png、jpeg）";
                    return Ok(result);
                }
                //判断文件大小    
                long length = Filedata.Length;
                if (length > 1024 * 1024 * 2) //2M
                {
                    result.success = false;
                    result.returnMsg = "上传的图片资源过大请重新选择";
                    return Ok(result);
                }
                string strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                string strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                string saveName = strDateTime + strRan + fileExtension;//图片文件名
                var filePath = Path.Combine(dirPath, saveName);//图片保存的文件目录地址
                //插入图片数据                 
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    Filedata.CopyTo(fs);
                    fs.Flush();
                }
                filenames.Add(filePath);
                result.returnData = filenames;
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 前端base64 图片解码
        /// </summary>
        /// <returns></returns>
        [HttpPost("ImagesBase64Jm")]
        public async Task<IActionResult> ImagesBase64Jm(parameterData urlComed)
        {
            try
            {
                var dataURLcode = urlComed.dataURL.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
                byte[] bytes = Convert.FromBase64String(dataURLcode);
                //var Filedata = Request.Form.Files[0];//                                                   
                //List<string> filenames = new List<string>();
                DateTime now = DateTime.Now;
                //文件存储路径
                string webRootPath = _hostEnvironment.WebRootPath; //存放系统 wwwroot 文件夹
                string uploadPath = Path.Combine("images", DateTime.Now.ToString("yyyyMMdd"));//文件目录
                string dirPath = Path.Combine(webRootPath, uploadPath);//指定当前图片保存目录

                ////判断是否已存在相同文件夹：是跳过否就创建
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //文件后缀
                string fileExtension = Path.GetExtension(urlComed.fileExt);

                //判断后缀是否是图片
                const string fileFilt = ".gif|.jpg|.jpeg|.png";
                if (fileExtension == null)
                {
                    result.success = false;
                    result.returnMsg = "请选择正确的头像证件相片";
                    return Ok(result);
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    result.success = false;
                    result.returnMsg = "请上传正确格式的图片（jpg、png、jpeg）";
                    return Ok(result);
                }
                string strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                string strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                string saveName = strDateTime + strRan + fileExtension;//图片文件名
                var filePath = Path.Combine(dirPath, saveName);//图片保存的文件目录地址
                using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                };
                result.success = true;
                result.returnData = filePath;
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}