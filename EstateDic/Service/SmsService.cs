using EstateDic.SMS;
using System;
using System.Configuration;

namespace EstateDic.Service
{
    public class SmsService
    {
        private string target = Helper.ConfigHelper.GetConfigString("SMSTarget");
        DbHelperSQLP db = new DbHelperSQLP(ConfigurationManager.AppSettings["ConnectionSms"]);
        MessageSendManage msService = new MessageSendManage();
        public SMSSendResult SendSms(string mobile,string message,string sign)
        {
            SMSSendResult result = new SMSSendResult();
            try
            {
                Guid id = Guid.NewGuid();
                var re = msService.SendOneMessage(mobile, message, sign, id.ToString(), int.Parse(target));
                if (re!="")
                {
                    result.success = false;
                    result.info = re;                 
                }
                else
                {
                    result.success = true;
                    result.info = "发送成功";
                    
                    try
                    {
                        string str = string.Format("insert into dbo.SMSSendBase(Id,Phone,Message,State,CreateDate,CreateUserId,SenderMark,SendCount,SendState,UnitPrice,UpdateDate,TaskId,"
                                + "ClientName,SmsTempletName,Templet_Id,Category,BatchId) values('{0}','{1}','{2}',1,getdate(),'{3}',{4},1,0,0.07,getdate(),'{3}','{5}','{5}','{3}','CFC','{0}');",
                                    id, mobile, message, Guid.Empty, target, sign);
                        db.ExecuteSql(str);
                    }
                    catch (Exception ex)
                    {
                        result.info="入库失败!";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.info = ex.Message;
                return result;
            }
        }
    }
}