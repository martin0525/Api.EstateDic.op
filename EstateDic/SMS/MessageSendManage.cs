using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using EstateDic.Helper;
using System.Configuration;

namespace EstateDic.SMS
{
    public class MessageSendManage
    {

        /// <summary>
        /// 短信发送接口
        /// </summary>
        private string messageSendInterface = Helper.ConfigHelper.GetConfigString("MessageSend");
        private string messageKey = Helper.ConfigHelper.GetConfigString("MessageSendKey");
        
        /// <summary>
        /// <para> 功   能：是否有敏感词 </para>
        /// <para> 作   者：韩保新</para>
        /// <para> 创建日期：2012/3/22</para>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string CheckSensitiveWords(string content)
        {
            MessageSendModel model = new MessageSendModel();
            model.message_content = content;
            string url = messageSendInterface.ToLower().Replace("messagesend.ashx", "CheckSensitiveWords.ashx");
            return SendContent(model, url);
        }

        /// <summary>
        /// <para> 功    能： 单条短息发送 </para>
        /// <para> 作    者： 韩保新 </para>
        /// <para> 创建日期： 2011-11-21</para>
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <param name="content">内容</param>
        /// <param name="sender">发送者姓名</param>
        /// <param name="sId">发送主键</param>
        /// <returns></returns>
        public string SendOneMessage(string phone, string content, string sender, string sId, int target, string timing = "", int priority = 20)
        {
            MessageSendModel model = new MessageSendModel();
            model.is_group_message = 0;
            model.message_content = content;
            model.message_pkey = sId;
            model.mobiles = phone;
            model.sender = sender;
            model.timing = timing;
            model.message_target = target;
            model.priority = priority;
            return SendContent(model);
        }
        /// <summary>
        /// <para> 功    能： 短信群发 </para>
        /// <para> 作    者： 韩保新 </para>
        /// <para> 创建日期： 2011-11-21</para>
        /// </summary>
        /// <param name="phone">电话号码字符串</param>
        /// <param name="content">内容</param>
        /// <param name="sender">发送者</param>
        /// <param name="sId">主键</param>
        /// <param name="target">通道</param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        public string SendMoreMessage(string phone, string content, string sender, string sId, int target, int priority = 20)
        {
            return SendMoreMessageAuto(phone, content, sender, sId, "", target, priority);
        }
        /// <summary>
        /// <para> 功    能： 定时群发 </para>
        /// <para> 作    者： 韩保新 </para>
        /// <para> 创建日期： 2011-11-21</para>
        /// </summary>
        /// <param name="phone">电话号码字符串</param>
        /// <param name="content">内容</param>
        /// <param name="sender">发送者</param>
        /// <param name="sId">主键</param>
        /// <param name="time">定时时间</param>
        /// <param name="target">通道</param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        public string SendMoreMessageAuto(string phone, string content, string sender, string sId, string time, int target, int priority = 20, int isgroup = 1)
        {
            MessageSendModel model = new MessageSendModel();
            model.is_group_message = isgroup;
            model.message_content = content;
            model.message_pkey = sId;
            model.mobiles = phone;
            model.sender = sender;
            model.timing = time;
            model.message_target = target;
            model.priority = priority;
            return SendContent(model);
        }
        /// <summary>
        /// <para> 功    能： 短信接口 写入 </para>
        /// <para> 作    者： 韩保新 </para>
        /// <para> 创建日期： 2011-11-21</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string SendContent(MessageSendModel model, string url = "")
        {
            model.key = GetMd5Str(messageKey + model.message_content);
            model.message_send_module = "Callcenter";
            model.message_type = 1;

            System.Text.Encoding GB2312 = System.Text.Encoding.GetEncoding("GB2312");
            string strURL = messageSendInterface;
            string strParameters = "";
            if (!string.IsNullOrEmpty(url))
            {
                strURL = url;
                strParameters = string.Format("content={0}", Uri.EscapeUriString(model.message_content));
            }
            else
                strParameters = string.Format("params={0}", Uri.EscapeUriString(Helper.JsonExtension.ToJsonString(model)));
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);
            byte[] szByte = GB2312.GetBytes(strParameters);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = szByte.Length;
            httpWebRequest.Method = "POST";
            using (Stream reqStream = httpWebRequest.GetRequestStream())
            {
                reqStream.Write(szByte, 0, szByte.Length);
            }
            SMSResultModel result = new SMSResultModel();
            string responseString = "";
            using (WebResponse wr = httpWebRequest.GetResponse())
            {
                //在这里对接收到的页面内容进行处理
                Stream responseStream = wr.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                responseString = responseReader.ReadToEnd();
            }
            result = responseString.ToJsonObject<SMSResultModel>();
            string callbackResult = "";
            switch (result.state)
            {
                case 1: callbackResult = ""; break;
                case 0: callbackResult = "传入参数为空！"; break;
                case 2: callbackResult = "手机号码发送次数超标！"; break;
                case 3: callbackResult = "手机号码不符合规范！"; break;
                case 4: callbackResult = "传入参数错误！"; break;
                case 5: callbackResult = "发送方Key错误！"; break;
                case 6:
                    var r0 = responseString.ToJsonObject<MessageSendResult>();
                    var r = r0.result.ToJsonObject<List<SiviveModel>>();
                    callbackResult = r.Select(a => a.SENSITIVE_WORD).ToList().ListToString();
                    break;
                case 10: callbackResult = "接口代码意外错误！"; break;
                case 99: callbackResult = "MD5验证错误！"; break;
                default: callbackResult = "意外错误"; break;

            }
            return callbackResult;

        }
        
        private string GetMd5Str(string ConvertString)
        {
            ConvertString = ConvertString.ToUpper();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)));
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}