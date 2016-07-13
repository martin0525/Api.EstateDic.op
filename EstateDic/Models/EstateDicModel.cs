using System.Collections.Generic;

namespace EstateDic.Models
{
    //楼盘实体
    public class EstateDicModel
    {
        //楼盘ID
        public string ESTATE_GUID { get; set; }
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

    //楼盘接口返回实体
    public class EstateResult
    {
        public bool success { get; set; }
        public string info { get; set; }
        public List<EstateDicModel> data { get; set; }
    }

    //楼栋接口返回实体
    public class BuildingResult
    {
        public bool success { get; set; }
        public string info { get; set; }
        public List<BuildingModel> data { get; set; }
    }

    //楼栋实体
    public class BuildingModel
    {
        //楼栋ID
        public string BUILDING_GUID { get; set; }
        //楼栋名称
        public string BUILDING_NAME { get; set; }
        //建筑类型
        public string CONFORMATION { get; set; }
        //楼盘ID
        public string ESTATE_GUID { get; set;}
        //楼栋地址
        public string DISPLAY_BUILDING_ADDRESS { get; set; }
        //总楼层
        public string FLOOR_TOTAL { get; set; }
        //总户数
        public string ROOM_TOTAL { get; set; }
        //每层户数
        public string ROOM_PER_FLOOR { get; set; }
    }

    //房间接口返回实体
    public class RoomResult
    {
        public bool success { get; set; }
        public string info { get; set; }
        public List<RoomModel> data { get; set; }
    }
    
    //房间实体
    public class RoomModel
    {
        //房间ID
        public string ROOM_GUID { get; set; }
        //房间号
        public string ROOM_NUMBER { get; set; }
        //楼层
        public string FLOOR_NUMBER { get; set; }
        //楼栋ID
        public string BUILDING_GUID { get; set; }
        //楼盘ID
        public string ESTATE_GUID { get; set; }
        //地址
        public string ADDRESS { get; set; }
        //房型
        public string ROOM_TYPE { get; set; }
        //面积
        public string AREA { get; set; }
        //朝向
        public string DIRECTION { get; set; }
        //厅
        public string SITTING_ROOM_TOTAL { get; set; }
        //房
        public string BEDROOM_TOTAL { get; set; }
        //卫生间
        public string WASHROOM_TOTAL { get; set; }
        //阳台
        public string BALCONY_TOTAL { get; set; }
        //厨房
        public string KITCHEN_TOTAL { get; set; }


    }
}
