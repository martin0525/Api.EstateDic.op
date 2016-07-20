using EstateDic.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EstateDic.Service;
using EstateDic.Helper;

namespace EstateDic.Controllers
{
    public class MsgController : Controller
    {
        SmsService se = new SmsService();
        // GET: Msg
        public ActionResult Index()
        {
            return View();
        }

        //短信发送接口（mobile:手机号|txt:短信内容|sign：短信签名）
        public ActionResult SendMsg(string mobile, string txt,string sign)
        {
            SMSSendResult re = new SMSSendResult();
            if (string.IsNullOrEmpty(mobile))
            {
                re.success = false;
                re.info = "手机号不能为空!";
            }
            else if (!IsMobile(mobile))
            {
                re.success = false;
                re.info = "手机号格式错误!";
            }
            else if (string.IsNullOrEmpty(sign))
            {
                re.success = false;
                re.info = "短信签名不能为空!";
            }
            else
            {
                string msg = "";
                //短信内容 （格式：【签名】+内容）                    
                string allmsg = string.Format(@"【{0}】{1}", sign, txt);
                try
                {
                    //检测敏感字
                    if (SMSHelper.CheckSensitiveWords(allmsg, out msg))
                    {
                        re.success = false;
                        re.info = (string.IsNullOrEmpty(msg) ? "敏感字检测失败！" : (msg + "为敏感字!"));
                        return Content(re.ToJson());
                    }
                    re = se.SendSms(mobile, allmsg, sign);
                }
                catch (Exception ex)
                {
                    re.success = false;
                    re.info = ex.Message;
                }
            }
            return Content(re.ToJson());
        }

        public bool IsMobile(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+\d{10}");

        }
    }
}