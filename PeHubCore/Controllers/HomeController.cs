using Microsoft.AspNetCore.Mvc;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PeHubCore.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> pdf(string code)
        {
            try
            {
                string urlpath = AES.AESDecrypt(code, "abcdefgabcdefg12");
                LogHelper.Info("下载PDF", "PDF文件路径：" + urlpath);

                string[] arr = Regex.Split(urlpath, ".pdf/");
                if (arr.Length < 2)
                {
                    LogHelper.Error("下载PDF参数错误", $"参数：{urlpath}，错误信息：参数错误");
                    return Content("参数错误");
                }

                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var timeNum = Convert.ToInt64(ts.TotalSeconds);
                var pdfTimeNum = Convert.ToInt64(arr[1]);
                if (pdfTimeNum > timeNum + 60 || pdfTimeNum < timeNum - 60)
                {
                    LogHelper.Error("下载PDF超时", $"参数：{urlpath}，错误信息：当前查看pdf报告已超时，请重新打开");
                    return Content("当前查看pdf报告已超时，请重新打开");
                }

                urlpath = urlpath.Replace("/" + arr[1], "");

                string tjServiceUrl = Appsettings.GetSectionValue("WebserviceHub:ReportPdfUrl");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(tjServiceUrl + "/" + urlpath);
                req.Method = "GET";

                try
                {
                    WebResponse wr = req.GetResponse();
                    return File(wr.GetResponseStream(), "application/pdf", "体检报告.pdf");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("404"))
                    {
                        LogHelper.Error("下载PDF404", $"参数：{urlpath}，错误信息：" + e.Message);
                        return Content("暂无下载的PDF报告");
                    }

                    throw e;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("下载PDF异常", "参数code：" + code + "；错误信息：" + ex.Message);
                return Content("下载PDF报告失败");
            }
        }
    }
}
