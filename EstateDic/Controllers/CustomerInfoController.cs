using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EstateDic.Models;
using EstateDic.Helper;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace EstateDic.Controllers
{
    public class CustomerInfoController : Controller
    {
        //连接串
        string ConnStringCustomerInfo = ConfigurationManager.ConnectionStrings["CustomerInfoDBConnStr"].ConnectionString;

        /// <summary>
        /// 生成MD5码
        /// </summary>
        /// <param name="SourceString">源串</param>
        /// <returns>MD5码</returns>
        private String GetMD5String(String SourceString)
        {
            if (String.IsNullOrEmpty(SourceString))
            {
                return null;
            }

            StringBuilder ResultBuilder = new StringBuilder();

            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider MD5CSP
                    = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] ByteValue = System.Text.Encoding.UTF8.GetBytes(SourceString);
                byte[] ByteHash = MD5CSP.ComputeHash(ByteValue);
                MD5CSP.Clear();

                for (int i = 0; i < ByteHash.Length; i++)
                {
                    ResultBuilder.Append(ByteHash[i].ToString("x2"));
                }
            }
            catch (Exception)
            {
                return null;
            }

            return ResultBuilder.ToString();
        }

        /// <summary>
        /// 通过身份证号获取年龄
        /// </summary>
        /// <param name="IDNumber">身份证号字符串</param>
        /// <returns>年龄</returns>
        private int GetAgeFromIDNumber(String IDNumber)
        {
            if (IDCardValidation.CheckIDCard(IDNumber) == false)
            {
                return -1;
            }

            DateTime BirthDate = DateTime.MinValue;

            if (IDNumber.Length == 15)
            {
                String BirthDateStringFromIDNumber = IDNumber.Substring(6, 2) + "-"
                    + IDNumber.Substring(8, 2) + "-" + IDNumber.Substring(10, 2);
                if (DateTime.TryParse(BirthDateStringFromIDNumber, out BirthDate) == false)
                {
                    return -1;
                }

                return (int)((DateTime.Now - BirthDate).TotalDays / 365) + 1;
            }
            else if (IDNumber.Length == 18)
            {
                String BirthDateStringFromIDNumber = IDNumber.Substring(6, 4) + "-"
                    + IDNumber.Substring(10, 2) + "-" + IDNumber.Substring(12, 2);
                if (DateTime.TryParse(BirthDateStringFromIDNumber, out BirthDate) == false)
                {
                    return -1;
                }

                return (int)((DateTime.Now - BirthDate).TotalDays / 365) + 1;
            }

            return -1;
        }

        /// <summary>
        /// 通过身份证号获取性别
        /// </summary>
        /// <param name="IDNumber">身份证号字符串</param>
        /// <returns>性别</returns>
        private String GetSexFromIDNumber(String IDNumber)
        {
            if (IDCardValidation.CheckIDCard(IDNumber) == false)
            {
                return "";
            }

            //0为男,1为女
            int Sex = 0;

            if (IDNumber.Length == 15)
            {
                if (int.TryParse(IDNumber.Substring(14, 1), out Sex) == false)
                {
                    return "";
                }

                return Sex % 2 == 1 ? "男" : "女";
            }
            else if (IDNumber.Length == 18)
            {
                if (int.TryParse(IDNumber.Substring(16, 1), out Sex) == false)
                {
                    return "";
                }

                return Sex % 2 == 1 ? "男" : "女";
            }

            return "";
        }

        /// <summary>
        /// 字段内容类别
        /// </summary>
        private enum DataSetResultQueryType
        {
            /// <summary>
            /// 最多出现的单个值
            /// </summary>
            MostOccuredValue,

            /// <summary>
            /// 逗号分隔的多个值拼接
            /// </summary>
            CommaSeparatedValues
        }
        
        /// <summary>
        /// 获取对应数据,以指定方式返回
        /// </summary>
        /// <param name="ds">源</param>
        /// <param name="TableIndex">表序号 0-需求 1-成交 2-同地址成交 3-兴趣爱好</param>
        /// <param name="ColumnName">字段名</param>
        /// <param name="ResultType">返回样式</param>
        /// <param name="MaxValuesCount">返回为逗号分隔值时,最多返回几个值</param>
        /// <returns>返回值字符串</returns>
        private String GetSpecificInfoFromDataset(DataSet ds,
            int TableIndex, String ColumnName, DataSetResultQueryType ResultType, int MaxValuesCount = 2)
        {
            String ResultString = "";

            Dictionary<String, int> RawStringsStat = new Dictionary<string, int>();

            if (ds != null && ds.Tables.Count > TableIndex && ds.Tables[TableIndex].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[TableIndex].Rows.Count; i++)
                {
                    String CurrString = ds.Tables[TableIndex].Rows[i][ColumnName].ToString();

                    //跳过空串
                    if (String.IsNullOrWhiteSpace(CurrString))
                    {
                        continue;
                    }

                    //查重
                    if (RawStringsStat.ContainsKey(CurrString))
                    {
                        RawStringsStat[CurrString] = RawStringsStat[CurrString] + 1;
                    }
                    else
                    {
                        RawStringsStat.Add(CurrString, 0);
                    }
                }
            }

            if (RawStringsStat.Count == 0)
            {
                return "";
            }

            //值排序
            RawStringsStat = RawStringsStat.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);

            if (ResultType == DataSetResultQueryType.MostOccuredValue)
            {
                return RawStringsStat.Keys.ToArray()[0];
            }

            if (ResultType == DataSetResultQueryType.CommaSeparatedValues)
            {
                if (MaxValuesCount == 0)
                {
                    return "";
                }

                //拼接原串
                String RawStringResult = "";
                for (int i = 0; i < RawStringsStat.Count; i++)
                {
                    RawStringResult += RawStringsStat.Keys.ToArray()[i] + ",";
                }

                //按逗号分隔转成数组
                String[] RawStringResultArray = RawStringResult.Split(',');

                //非空去重
                List<String> StringResultArray = new List<string>();
                for (int i = 0; i < RawStringResultArray.Length; i++)
                {
                    if (!String.IsNullOrWhiteSpace(RawStringResultArray[i])
                        && !StringResultArray.Contains(RawStringResultArray[i]))
                    {
                        StringResultArray.Add(RawStringResultArray[i]);
                    }
                }

                int MaxCount = MaxValuesCount < StringResultArray.Count ? MaxValuesCount : StringResultArray.Count;

                ResultString = StringResultArray[0];
                //取前n个标签
                for (int i = 1; i < MaxCount; i++)
                {
                    ResultString += ',' + StringResultArray[i];
                }
            }

            return ResultString;
        }

        /// <summary>
        /// 组装客户信息
        /// </summary>
        /// <param name="Mobile">手机号</param>
        /// <param name="IDNumber">身份证号</param>
        /// <param name="Tag">输出的客户信息</param>
        /// <returns>组装结果</returns>
        private int AssembleCustomerInfo(String Mobile, String IDNumber, out PersonTag Tag)
        {
            Tag = new PersonTag();
            Tag.TradeInfos = new List<TradeInfo>();
            Tag.DemandInfos = new List<DemandInfo>();

            DataSet ResultSet = new DataSet();

            //按给定的Mobile和IDNumber查询客户信息
            try
            {
                ResultSet = new DbHelperSQLP(ConnStringCustomerInfo).RunProcedure("P_QueryCustomerInfo",
                    new IDataParameter[] { new SqlParameter("@Mobile", Mobile), new SqlParameter("@IDNumber", IDNumber) }
                    , "noname");
            }
            catch (Exception)
            {
                Tag = null;
                return -1;
            }

            //没有查到
            if(ResultSet.Tables[0].Rows.Count == 0 &&
                ResultSet.Tables[1].Rows.Count == 0 &&
                ResultSet.Tables[2].Rows.Count == 0 &&
                ResultSet.Tables[3].Rows.Count == 0)
            {
                Tag = null;
                return -3;
            }

            Tag.Mobile = Mobile;
            Tag.IDNumber = IDNumber;


            //同时提供手机号和身份证号时
            if (!String.IsNullOrEmpty(IDNumber) && !String.IsNullOrEmpty(Mobile))
            {
                //检查同时提供手机号和身份证号时,是否与库中数据匹配
                bool MobileMatchesIDNumber = false;
                for (int i = 0; i < ResultSet.Tables[1].Rows.Count; i++)
                {
                    String CheckStringMobile = ResultSet.Tables[1].Rows[i]["mobile"].ToString();
                    String CheckStringIDNumber = ResultSet.Tables[1].Rows[i]["id_number"].ToString();

                    if (CheckStringMobile.Length == 11 && CheckStringIDNumber.Length >= 15 &&
                        CheckStringMobile == Mobile && CheckStringIDNumber == IDNumber)
                    {
                        MobileMatchesIDNumber = true;
                        break;
                    }
                }
                if (MobileMatchesIDNumber == false)
                {
                    return -2;
                }
            }

            if (String.IsNullOrWhiteSpace(Mobile))
            {
                Tag.Mobile = GetSpecificInfoFromDataset(ResultSet,
                    1, "mobile", DataSetResultQueryType.MostOccuredValue);
            }

            if (String.IsNullOrWhiteSpace(IDNumber))
            {
                Tag.IDNumber = GetSpecificInfoFromDataset(ResultSet,
                    1, "id_number", DataSetResultQueryType.MostOccuredValue);
            }

            Tag.Name = GetSpecificInfoFromDataset(ResultSet,
                1, "customer_name", DataSetResultQueryType.MostOccuredValue);

            Tag.Sex = GetSpecificInfoFromDataset(ResultSet,
                1, "sex", DataSetResultQueryType.MostOccuredValue);
            if (String.IsNullOrWhiteSpace(Tag.Sex) && !String.IsNullOrWhiteSpace(Tag.IDNumber))
            {
                Tag.Sex = GetSexFromIDNumber(Tag.IDNumber);
            }

            Tag.Age = "";
            if (!String.IsNullOrWhiteSpace(Tag.IDNumber))
            {
                int IDNumberAge = GetAgeFromIDNumber(Tag.IDNumber);
                if (IDNumberAge >= 0)
                {
                    Tag.Age = IDNumberAge.ToString();
                }
            }

            Tag.RegisterCity = GetSpecificInfoFromDataset(ResultSet,
                1, "census_register", DataSetResultQueryType.MostOccuredValue);

            Tag.Profession = GetSpecificInfoFromDataset(ResultSet,
                3, "industry", DataSetResultQueryType.CommaSeparatedValues, 2);

            Tag.Interests = GetSpecificInfoFromDataset(ResultSet,
                3, "tags", DataSetResultQueryType.CommaSeparatedValues, 3);

            Tag.FamilyIncome = GetSpecificInfoFromDataset(ResultSet,
                3, "family_income", DataSetResultQueryType.MostOccuredValue);

            Tag.Marriage = GetSpecificInfoFromDataset(ResultSet,
                3, "marry", DataSetResultQueryType.MostOccuredValue);

            Tag.FamilyStatus = GetSpecificInfoFromDataset(ResultSet,
                3, "family_structure", DataSetResultQueryType.MostOccuredValue);

            Tag.ChildrenStatus = GetSpecificInfoFromDataset(ResultSet,
                3, "children_info", DataSetResultQueryType.MostOccuredValue);

            //解析家庭
            if (ResultSet.Tables[2].Rows.Count > 1)
            {
                //载入性别年龄
                List<int> Ages = new List<int>();
                List<String> Sexes = new List<string>();
                for (int i = 0; i < ResultSet.Tables[2].Rows.Count; i++)
                {
                    int Age = GetAgeFromIDNumber(ResultSet.Tables[2].Rows[i]["id_number"].ToString());
                    String Sex = GetSexFromIDNumber(ResultSet.Tables[2].Rows[i]["id_number"].ToString());

                    if (Age >= 0 && (Sex == "男" || Sex == "女"))
                    {
                        Ages.Add(Age);
                        Sexes.Add(Sex);
                    }
                }

                ////2异性一代人 异性且年龄差＜18
                ////2同性一代人 同性且年龄差＜18
                ////2两代人 年龄差≥18
                ////3一代人 年龄极差＜18
                ////3两代人 年龄极差≥18且存在两个年龄相差＜18
                ////3三代人 任意两年龄极差≥18
                ////4一代人 年龄极差＜18
                ////4两代人 年龄极差≥18且任意年龄值的min{ 该年龄值与两个极值的差值的绝对值}＜18
                ////4三代人 年龄极差≥18且存在一个年龄值min{ 该年龄值与两个极值的差值的绝对值}≥18
                ////    且存在一个年龄值min{ 该年龄值与其他三个的差值的绝对值}＜18
                ////4四代人 任意两年龄极差≥18  任意两年龄极差≥18
                ////≥5                      署名人数≥5

                //2人情况
                if (Ages.Count == 2)
                {
                    if (Math.Abs(Ages[0] - Ages[1]) < 18)
                    {
                        if (Sexes[0] != Sexes[1])
                        {
                            Tag.FamilyStatus = "2人,异性,一代人";
                            if (Math.Abs(Ages[0] - Ages[1]) <= 5)
                            {
                                Tag.Marriage = "已婚";
                            }
                        }
                        else
                        {
                            Tag.FamilyStatus = "2人,同性,一代人";
                        }
                    }
                    else
                    {
                        Tag.FamilyStatus = "2人,两代人";
                        if (Ages[0] < 18 || Ages[1] < 18)
                        {
                            Tag.FamilyStatus += ",有未成年子女";
                        }
                    }
                }

                int MaxAge = Ages.Max();
                int MinAge = Ages.Min();

                //3人情况
                if (Ages.Count == 3)
                {
                    if (MaxAge - MinAge < 18)
                    {
                        Tag.FamilyStatus = "3人,一代人";
                    }

                    if (MaxAge - MinAge >= 18)
                    {
                        for (int i = 0; i < Ages.Count; i++)
                        {
                            if (Math.Abs(Ages[0] - Ages[1]) < 18 ||
                                Math.Abs(Ages[1] - Ages[2]) < 18 ||
                                Math.Abs(Ages[0] - Ages[2]) < 18)
                            {
                                Tag.FamilyStatus = "3人,两代人";
                                if (MinAge < 18)
                                {
                                    Tag.FamilyStatus += ",有未成年子女";
                                }
                            }
                        }
                    }

                    if (Math.Abs(Ages[0] - Ages[1]) >= 18 &&
                        Math.Abs(Ages[2] - Ages[1]) >= 18 &&
                        Math.Abs(Ages[0] - Ages[2]) >= 18)
                    {
                        Tag.FamilyStatus = "3人,三代人";
                        if (MinAge < 18)
                        {
                            Tag.FamilyStatus += ",有未成年子女";
                        }

                    }
                }

                //4人情况
                if (Ages.Count == 4)
                {
                    if (MaxAge - MinAge < 18)
                    {
                        Tag.FamilyStatus = "4人,一代人";
                    }

                    //排序,从小到大
                    List<int> SortedAges = Ages.ToList<int>();
                    SortedAges.Sort();

                    if (SortedAges[3] - SortedAges[0] >= 18 &&
                        SortedAges[3] - SortedAges[2] < 18 &&
                        SortedAges[1] - SortedAges[0] < 18)
                    {
                        Tag.FamilyStatus = "4人,两代人";
                    }

                    if (SortedAges[3] - SortedAges[0] >= 18 && (
                        (SortedAges[3] - SortedAges[2] < 18 && SortedAges[2] - SortedAges[1] >= 18
                        && SortedAges[1] - SortedAges[0] >= 18) ||
                        (SortedAges[3] - SortedAges[2] >= 18 && SortedAges[2] - SortedAges[1] >= 18
                        && SortedAges[1] - SortedAges[0] < 18)))
                    {
                        Tag.FamilyStatus = "4人,三代人";
                    }

                    if (SortedAges[3] - SortedAges[2] >= 18 && SortedAges[2] - SortedAges[1] >= 18
                        && SortedAges[1] - SortedAges[0] >= 18)
                    {
                        Tag.FamilyStatus = "4人,四代人";
                    }
                }

                if (Ages.Count > 5)
                {
                    Tag.FamilyStatus = "多人共同居住";
                }
            }

            //需求和成交记录
            for (int i = 0; i < ResultSet.Tables[0].Rows.Count; i++)
            {
                DemandInfo DI = new DemandInfo();

                DateTime DemandTime;
                if (DateTime.TryParse(ResultSet.Tables[0].Rows[i]["demand_time"].ToString(), out DemandTime) == false)
                {
                    continue;
                }

                //保护最近180天需求数据
                if ((DateTime.Now - DemandTime).TotalDays <= 180)
                {
                    continue;
                }

                DI.DemandDate = DemandTime.ToString("yyyy-MM-dd");
                DI.DemandCity = ResultSet.Tables[0].Rows[i]["city"].ToString();
                DI.DemandGrade = ResultSet.Tables[0].Rows[i]["buy_grade"].ToString();
                DI.DemandProject = ResultSet.Tables[0].Rows[i]["project_name"].ToString();
                DI.DemandPurpose = ResultSet.Tables[0].Rows[i]["buy_purpose"].ToString();

                Tag.DemandInfos.Add(DI);
            }

            for (int i = 0; i < ResultSet.Tables[1].Rows.Count; i++)
            {
                TradeInfo TI = new TradeInfo();

                DateTime TradeTime;
                if (DateTime.TryParse(ResultSet.Tables[1].Rows[i]["trade_date"].ToString(), out TradeTime) == false)
                {
                    continue;
                }

                //保护最近720天成交数据
                if ((DateTime.Now - TradeTime).TotalDays <= 720)
                {
                    continue;
                }

                TI.TradeDate = TradeTime.ToString("yyyy-MM-dd");
                TI.TradeCity = ResultSet.Tables[1].Rows[i]["trade_city"].ToString();
                TI.TradeGrade = ResultSet.Tables[1].Rows[i]["trade_project_grade"].ToString();
                TI.TradeProject = ResultSet.Tables[1].Rows[i]["trade_project_name"].ToString();
                TI.TradePurpose = ResultSet.Tables[1].Rows[i]["trade_project_purpose"].ToString();

                Tag.TradeInfos.Add(TI);
            }

            return 0;
        }

        /// <summary>
        /// 查询客户信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="Mobile">客户手机号</param>
        /// <param name="IDNumber">客户身份证号</param>
        /// <param name="Sign">校验码</param>
        /// <returns>响应结果</returns>
        public JsonResult QueryCustomerInfo(String UserID, String Mobile, String IDNumber, String Sign)
        {
            CustomerInfoResponse JsonResponse = new CustomerInfoResponse();

            //输入参数有效性检测
            if (String.IsNullOrEmpty(Mobile) && String.IsNullOrEmpty(IDNumber))
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "At least one of [Mobile] or [IDNumber] should be provided.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            if (!String.IsNullOrEmpty(IDNumber) && !IDCardValidation.CheckIDCard(IDNumber))
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "The value of [IDNumber] is not valid.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            if (!String.IsNullOrEmpty(Mobile) &&
                (Mobile.Length != 11 || Mobile[0] != '1'))
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "The value of [Mobile] is not valid.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //检出接口名称
            String InterfaceName = ConfigurationManager.AppSettings["LejuInterfaceName1"].ToString();

            DataSet ResultSet = new DataSet();
            //用户key,一个GUID
            String UserKey = "";
            //每周期(月)最大查询次数
            int MaxQueryTimes = 0;
            //用户帐号过期时间
            DateTime ExpiredTime;
            //用户记录ID
            int UserRecordID = 0;
            //本周期已经查询次数
            int CurrentTimes = 0;

            //获取用户信息记录
            try
            {
                ResultSet = new DbHelperSQLP(ConnStringCustomerInfo).Query(
                    "select [ID],[UserKey],[ExpiredTime],[MaxQueryTimes] from tb_interface_user " +
                    "where InterfaceName = @InterfaceName and UserID = @UserID and Status = 1 ",
                    new SqlParameter[] { new SqlParameter("@InterfaceName", InterfaceName), new SqlParameter("@UserID", UserID) });
            }
            catch (Exception)
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "Validation Service failed.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //解析用户信息
            try
            {
                UserKey = ResultSet.Tables[0].Rows[0]["UserKey"].ToString();
                MaxQueryTimes = (int)ResultSet.Tables[0].Rows[0]["MaxQueryTimes"];
                ExpiredTime = (DateTime)ResultSet.Tables[0].Rows[0]["ExpiredTime"];
                UserRecordID = (int)ResultSet.Tables[0].Rows[0]["ID"];
            }
            catch (Exception)
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "Invalid user.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //用户过期检测
            if(DateTime.Now>ExpiredTime)
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "User account expired.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //获取查询历史信息
            try
            {
                //获取本月访问次数
                CurrentTimes = (int)(new DbHelperSQLP(ConnStringCustomerInfo).GetSingle(
                    "select count(1) from tb_interface_query_log where [UserRecordID] = @UserRecordID " +
                    "and QueryTime > DATENAME(YEAR,GETDATE())+'-'+DATENAME(MONTH,GETDATE())+'-01'",
                    new SqlParameter[] { new SqlParameter("@UserRecordID", UserRecordID) }));

            }
            catch (Exception)
            {
                //次数查询失败时认为查询可用
                CurrentTimes = 0;
            }

            //本周期查询次数检测
            if (CurrentTimes >= MaxQueryTimes)
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "Query times exceeded.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //MD5校验
            String SignStr = UserKey;

            if (!String.IsNullOrEmpty(IDNumber))
            {
                SignStr += "&IDNumber=" + IDNumber;
            }
            if (!String.IsNullOrEmpty(Mobile))
            {
                SignStr += "&Mobile=" + Mobile;
            }
            if (GetMD5String(SignStr) != Sign)
            {
                JsonResponse.Result = "Failed";
                JsonResponse.Message = "Sign check failed.";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }
            
            //组装客户信息
            PersonTag Tag = null;
            int AssembleResult = AssembleCustomerInfo(Mobile, IDNumber, out Tag);
            if (AssembleResult != 0)
            {
                if (AssembleResult == -1)
                {
                    JsonResponse.Message = "DB service failed.";
                }

                if (AssembleResult == -2)
                {
                    JsonResponse.Message = "The value of [Mobile] and [IDNumber] don't match.";
                }

                if (AssembleResult == -3)
                {
                    JsonResponse.Message = "No relative records.";
                }

                JsonResponse.Result = "Failed";
                return Json(JsonResponse, JsonRequestBehavior.AllowGet);
            }

            //输出结果
            JsonResponse.Data = Tag;
            JsonResponse.Result = "Succeeded.";
            //本周期剩余查询次数
            JsonResponse.Message = (MaxQueryTimes - CurrentTimes - 1) + " queries available in this month.";
            JsonResult JR = Json(JsonResponse, JsonRequestBehavior.AllowGet);
            String JRString = JR.Data.ToJsonString();

            //记录查询历史
            String Parameters = Request.Url.ToString().Substring(Request.Url.ToString().IndexOf('?') + 1);
            try
            {
                new DbHelperSQLP(ConnStringCustomerInfo).ExecuteSql(
                    "INSERT INTO [tb_interface_query_log] ([UserRecordID],[UserID],[InterfaceName],[Parameters],[Response],[QueryTime]) " +
                    "values (@UserRecordID, @UserID, @InterfaceName, @Parameters, @Response, GetDate())",
                    new SqlParameter[] {
                        new SqlParameter("@UserRecordID", UserRecordID),
                        new SqlParameter("@UserID", UserID),
                        new SqlParameter("@InterfaceName", InterfaceName),
                        new SqlParameter("@Parameters", Parameters),
                        new SqlParameter("@Response", JRString)
                    });
            }
            catch (Exception)
            {
            }

            return JR;
        }
    }
}