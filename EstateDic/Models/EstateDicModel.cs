using System.Collections.Generic;

namespace EstateDic.Models
{
    public class EstateDicModel
    {
        //楼盘名称
        public string ESTATE_NAME { get; set; }
        //楼盘拼音
        public string ESTATE_NAME_PY { get; set; }
        //楼盘别名
        public string MULTI_NAMES { get; set; }
        //城市
        public string CITY_NAME { get; set; }
        //区域
        public string DISTRICT_NAME { get; set; }
        //板块
        public string PLATE_NAME { get; set; }
        //详细地址
        public string ADDRESS { get; set; }
        //多地址
        public string MULTI_ADDRESSES { get; set; }
        //坐标X
        public string X { get; set; }
        //坐标Y
        public string Y { get; set; }
        //开发商
        public string DEVELOPER { get; set; }
        //物业公司
        public string PROPERTY_COMPANY { get; set; }
        //用途
        public string PROPERTY_TYPE { get; set; }
        //类型
        public string CONFORMATION { get; set; }
        //建筑年代
        public string BUILD_DATE { get; set; }
        //总面积
        public string AREA_TOTAL { get; set; }
        //总户数
        public string ROOM_TOTAL { get; set; }
        //车位数
        public string PARKING_INFO { get; set; }
        //绿化率
        public string GREENING_RATE { get; set; }
        //容积率
        public string PLOT_RATIO { get; set; }
    }

    public class EstateResult
    {
        public bool success { get; set; }
        public string info { get; set; }
        public List<EstateDicModel> data { get; set; }
    }
}
