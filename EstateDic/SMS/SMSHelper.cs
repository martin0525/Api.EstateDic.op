using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateDic.SMS
{
    public static class SMSHelper
    {
        /// <summary>
        /// NLog对象，记录日志
        /// </summary>
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 检查是否有敏感字符
        /// </summary>
        /// <param name="content">短信内容</param>
        /// <param name="message">错误信息</param>
        /// <returns>true：有敏感字符</returns>
        public static bool CheckSensitiveWords(string content, out string message)
        {
            message = "";
            try
            {
                MessageSendManage mm = new MessageSendManage();
                message = mm.CheckSensitiveWords(content);
                if (string.IsNullOrEmpty(message) || message.Split('|')[0] == "1")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                //message = "敏感字符检测失败！";
                Logger.DebugException(message, ex);
                return true;
            }
        }

    }
}