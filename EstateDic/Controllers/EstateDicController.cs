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
        // GET: EstateDic
        public ActionResult Index()
        {
            return View();
        }

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
                EstateDicService us = new EstateDicService();
                r = us.PushEstateInfo(city, data);
            }
            return Content(r.ToJson());
        }
    }
}