namespace EstateDic.SMS
{
    public class MessageSendModel
    {
        /// <summary>
        /// 短信发送方（如Callcenter）
        /// </summary>
        public string message_send_module { get; set; }
        /// <summary>
        /// 手机号（群发的以半角英文逗号分割）
        /// </summary>
        public string mobiles { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string message_content { get; set; }
        /// <summary>
        /// 数据中心分配的key和短信内容的md5值md5（数据中心key+内容）
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 是否群发信息（群发为1，单条为0）默认值0
        /// </summary>
        public int is_group_message { get; set; }
        /// <summary>
        /// 短信发送方（后台负责发送短信的接口，默认1）暂时可不传
        /// </summary>
        public int message_target { get; set; }
        /// <summary>
        /// 短信类型（普通短信1 默认1） 可不传
        /// </summary>
        public int message_type { get; set; }
        /// <summary>
        /// 发送人 可不传，有最好传入
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 定时时间。没有则不传此参数或传入空
        /// </summary>
        public string timing { get; set; }
        /// <summary>
        /// 短信发送方的短信主键或者说唯一标识符（查询短信发送状态时用）如不需查询可以不传
        /// </summary>
        public string message_pkey { get; set; }
        public int? priority { get; set; }
    }
    public class SMSResultModel
    {
        public int state { get; set; }
    }
    public class MessageSendResult
    {
        public int state { get; set; }
        public string result { get; set; }
    }
    
    public class SiviveModel
    {
        public string SENSITIVE_WORD { get; set; }
    }
    
    public class SMSSendResult
    {
        public bool success { get; set; }
        public string info { get; set; }
    }
}