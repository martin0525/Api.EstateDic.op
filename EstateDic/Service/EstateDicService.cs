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
        #region 楼盘字典相关接口
        public EstateResult PushEstateInfo(string city, string data)
        {
            EstateResult r = new EstateResult();
            try
            {
                List<EstateDicModel> list = new List<EstateDicModel>();
                string sql = string.Format(@"select top 10 ESTATE_NAME,
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

                DbHelperSQLP db = new DbHelperSQLP(ConfigurationManager.AppSettings["ConnectionEstateDic"]);
                using (SqlDataReader rd = db.ExecuteReader(sql))
                {
                    while (rd.Read())
                    {
                        EstateDicModel mode = new EstateDicModel();
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



        #endregion

        private string getRdString(SqlDataReader rd, string column)
        {
            return (rd[column] == null ? "" : rd[column].ToString().Replace("'", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", ""));
        }
    }
}