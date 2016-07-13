using EstateDic.Helper;
using EstateDic.Models;
using EstateDic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EstateDic.Controllers
{
    public class EstateDicController : Controller
    {
        EstateDicService us = new EstateDicService();
        // GET: EstateDic
        public ActionResult Index()
        {
            return View();
        }


        //获取楼盘接口
        public ActionResult GetEstate(string data, string city)
        {
            EstateResult r = new EstateResult();
            if (string.IsNullOrEmpty(data))
            {
                r.info = "参数data不能为空!";
                r.success = false;
            }
            else if (string.IsNullOrEmpty(city))
            {
                r.info = "参数city不能为空!";
                r.success = false;
            }
            else
            {                
                r = us.PushEstateInfo(city, data);
            }
            return Content(r.ToJson());
        }

        //获取楼栋接口
        public ActionResult GetBuilding(string data)
        {
            BuildingResult r = new BuildingResult();
            if (string.IsNullOrEmpty(data))
            {
                r.info = "参数data不能为空!";
                r.success = false;
            }
            else
            {
                r = us.PushBuildingInfo(data);
            }
            return Content(r.ToJson());
        }

        //获取房间接口
        public ActionResult GetRoom(string data)
        {
            RoomResult r = new RoomResult();
            if (string.IsNullOrEmpty(data))
            {
                r.info = "参数data不能为空!";
                r.success = false;
            }
            else
            {
                r = us.PushRoomInfo(data);
            }
            return Content(r.ToJson());
        }
    }
}