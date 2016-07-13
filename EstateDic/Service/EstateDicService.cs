using EstateDic.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EstateDic.Service
{
    public class EstateDicService
    {
        DbHelperSQLP db = new DbHelperSQLP(ConfigurationManager.AppSettings["ConnectionEstateDic"]);
        #region 楼盘字典相关接口
        //获取楼盘信息接口（参数：城市|关键字）
        public EstateResult PushEstateInfo(string city, string data)
        {
            EstateResult r = new EstateResult();
            try
            {
                List<EstateDicModel> list = new List<EstateDicModel>();
                string sql = string.Format(@"select top 10 ESTATE_GUID,
                                ESTATE_NAME,
                                ESTATE_NAME_PY,
                                MULTI_NAMES,
                                CITY_NAME,
                                DISTRICT_NAME,
                                PLATE_NAME,
                                ADDRESS,
                                MULTI_ADDRESSES,
                                X,
                                Y,
                                DEVELOPER,
                                PROPERTY_COMPANY,
                                PROPERTY_TYPE,
                                CONFORMATION,
                                BUILD_DATE,
                                AREA_TOTAL,
                                ROOM_TOTAL,
                                PARKING_INFO,
                                GREENING_RATE,
                                PLOT_RATIO FROM TB_ESTATE(NOLOCK)
                                WHERE (MULTI_NAMES LIKE '%{0}%' OR ESTATE_NAME_PY LIKE '%{0}%' OR MULTI_ADDRESSES LIKE '%{0}%')
                                AND CITY_NAME='{1}' AND STATUS=1;", data, city);

                
                using (SqlDataReader rd = db.ExecuteReader(sql))
                {
                    while (rd.Read())
                    {
                        EstateDicModel mode = new EstateDicModel();
                        mode.ESTATE_GUID = getRdString(rd, "ESTATE_GUID");
                        mode.ESTATE_NAME = getRdString(rd, "ESTATE_NAME");
                        mode.ESTATE_NAME_PY = getRdString(rd, "ESTATE_NAME_PY");
                        mode.MULTI_NAMES = getRdString(rd, "MULTI_NAMES");
                        mode.CITY_NAME = getRdString(rd, "CITY_NAME");
                        mode.DISTRICT_NAME = getRdString(rd, "DISTRICT_NAME");
                        mode.PLATE_NAME = getRdString(rd, "PLATE_NAME");
                        mode.ADDRESS = getRdString(rd, "ADDRESS");
                        mode.X = getRdString(rd, "X");
                        mode.Y = getRdString(rd, "Y");
                        mode.DEVELOPER = getRdString(rd, "DEVELOPER");
                        mode.PROPERTY_COMPANY = getRdString(rd, "PROPERTY_COMPANY");
                        mode.PROPERTY_TYPE = getRdString(rd, "PROPERTY_TYPE");
                        mode.CONFORMATION = getRdString(rd, "CONFORMATION");
                        mode.BUILD_DATE = getRdString(rd, "BUILD_DATE");
                        mode.AREA_TOTAL = getRdString(rd, "AREA_TOTAL");
                        mode.ROOM_TOTAL = getRdString(rd, "ROOM_TOTAL");
                        mode.PARKING_INFO = getRdString(rd, "PARKING_INFO");
                        mode.GREENING_RATE = getRdString(rd, "GREENING_RATE");
                        mode.PLOT_RATIO = getRdString(rd, "PLOT_RATIO");
                        list.Add(mode);
                    }
                    rd.Close();
                    rd.Dispose();
                }
                r.success = true;
                r.info = "成功";
                r.data = list;
            }
            catch (Exception ex)
            {
                r.success = false;
                r.info = ex.Message;
            }
            return r;
        }

        //获取楼栋信息接口（参数：楼盘ID）
        public BuildingResult PushBuildingInfo(string data)
        {
            BuildingResult r = new BuildingResult();
            try
            {
                List<BuildingModel> list = new List<BuildingModel>();
                string sql = string.Format(@"select * from TB_BUILDING(nolock) where STATUS=1 and ESTATE_GUID='{0}' order by BUILDING_NAME;", data);

                using (SqlDataReader rd = db.ExecuteReader(sql))
                {
                    while (rd.Read())
                    {
                        BuildingModel mode = new BuildingModel();
                        mode.BUILDING_GUID = getRdString(rd, "BUILDING_GUID");
                        mode.BUILDING_NAME = getRdString(rd, "BUILDING_NAME");
                        mode.CONFORMATION = getRdString(rd, "CONFORMATION");
                        mode.ESTATE_GUID = getRdString(rd, "ESTATE_GUID");
                        mode.DISPLAY_BUILDING_ADDRESS = getRdString(rd, "DISPLAY_BUILDING_ADDRESS");
                        mode.FLOOR_TOTAL = getRdString(rd, "FLOOR_TOTAL");
                        mode.ROOM_TOTAL = getRdString(rd, "ROOM_TOTAL");
                        mode.ROOM_PER_FLOOR = getRdString(rd, "ROOM_PER_FLOOR");
                        list.Add(mode);
                    }                    
                    rd.Close();
                    rd.Dispose();
                }
                r.success = true;
                r.info = "成功";
                r.data = list;
            }
            catch (Exception ex)
            {
                r.success = false;
                r.info = ex.Message;
            }
            return r;
        }

        //获取房间信息接口（参数：楼栋ID）
        public RoomResult PushRoomInfo(string data)
        {
            RoomResult r = new RoomResult();
            try
            {
                List<RoomModel> list = new List<RoomModel>();
                string sql = string.Format(@"select * from TB_ROOM(nolock) where STATUS=1 and BUILDING_GUID='{0}' order by ROOM_NUMBER;", data);

                using (SqlDataReader rd = db.ExecuteReader(sql))
                {
                    while (rd.Read())
                    {
                        RoomModel mode = new RoomModel();
                        mode.ROOM_GUID = getRdString(rd, "ROOM_GUID");
                        mode.ROOM_NUMBER = getRdString(rd, "ROOM_NUMBER");
                        mode.FLOOR_NUMBER = getRdString(rd, "FLOOR_NUMBER");
                        mode.BUILDING_GUID = getRdString(rd, "BUILDING_GUID");
                        mode.ESTATE_GUID = getRdString(rd, "ESTATE_GUID");
                        mode.ADDRESS = getRdString(rd, "ADDRESS");
                        mode.ROOM_TYPE = getRdString(rd, "ROOM_TYPE");
                        mode.AREA = getRdString(rd, "AREA");
                        mode.DIRECTION = getRdString(rd, "DIRECTION");
                        mode.SITTING_ROOM_TOTAL = getRdString(rd, "SITTING_ROOM_TOTAL");
                        mode.BEDROOM_TOTAL = getRdString(rd, "BEDROOM_TOTAL");
                        mode.WASHROOM_TOTAL = getRdString(rd, "WASHROOM_TOTAL");
                        mode.BALCONY_TOTAL = getRdString(rd, "BALCONY_TOTAL");
                        mode.KITCHEN_TOTAL = getRdString(rd, "KITCHEN_TOTAL");
                        list.Add(mode);
                    }
                    rd.Close();
                    rd.Dispose();
                }
                r.success = true;
                r.info = "成功";
                r.data = list;
            }
            catch (Exception ex)
            {
                r.success = false;
                r.info = ex.Message;
            }
            return r;
        }
        #endregion

        private string getRdString(SqlDataReader rd, string column)
        {
            return (rd[column] == null ? "" : rd[column].ToString().Replace("'", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", ""));
        }
    }
}