using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstateDic.Models
{
    public class DemandInfo
    {
        public String DemandCity { get; set; }
        public String DemandProject { get; set; }
        public String DemandPurpose { get; set; }
        public String DemandGrade { get; set; }
        public String DemandDate { get; set; }
    }

    public class TradeInfo
    {
        public String TradeCity { get; set; }
        public String TradeProject { get; set; }
        public String TradePurpose { get; set; }
        public String TradeGrade { get; set; }
        public String TradeDate { get; set; }
    }


    public class PersonTag
    {
        public String Name { get; set; }
        public String Sex { get; set; }
        public String Age { get; set; }
        public String RegisterCity { get; set; }
        public String Mobile { get; set; }
        public String IDNumber { get; set; }
        public String Profession { get; set; }
        public String Interests { get; set; }
        public String Marriage { get; set; }
        public String FamilyIncome { get; set; }
        public String FamilyStatus { get; set; }
        public String ChildrenStatus { get; set; }
        public List<TradeInfo> TradeInfos { get; set; }
        public List<DemandInfo> DemandInfos { get; set; }
    }
}