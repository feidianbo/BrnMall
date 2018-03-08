using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城配送公司控制器类
    /// </summary>
    public partial class ShipCompanyController : BaseMallAdminController
    {
        /// <summary>
        /// 配送公司列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult List(int pageSize = 15, int pageNumber = 1)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminShipCompanies.GetShipCompanyCount());

            ShipCompanyListModel model = new ShipCompanyListModel()
            {
                PageModel = pageModel,
                ShipCompanyList = AdminShipCompanies.GetShipCompanyList(pageModel.PageSize, pageModel.PageNumber)
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize));
            return View(model);
        }

        /// <summary>
        /// 配送公司选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult SelectList(int pageSize = 15, int pageNumber = 1)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminShipCompanies.GetShipCompanyCount());
            List<ShipCompanyInfo> shipCompanyList = AdminShipCompanies.GetShipCompanyList(pageModel.PageSize, pageModel.PageNumber);

            StringBuilder result = new StringBuilder("{");
            result.AppendFormat("\"totalPages\":\"{0}\",\"pageNumber\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (ShipCompanyInfo shipCompanyInfo in shipCompanyList)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", shipCompanyInfo.ShipCoId, shipCompanyInfo.Name, "}");
            if (shipCompanyList.Count > 0)
                result.Remove(result.Length - 1, 1);
            result.Append("]}");

            return Content(result.ToString());
        }

        /// <summary>
        /// 添加配送公司
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            ShipCompanyModel model = new ShipCompanyModel();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加配送公司
        /// </summary>
        [HttpPost]
        public ActionResult Add(ShipCompanyModel model)
        {
            if (AdminShipCompanies.GetShipCoIdByName(model.CompanyName) > 0)
                ModelState.AddModelError("CompanyName", "名称已经存在");

            if (ModelState.IsValid)
            {
                ShipCompanyInfo shipCompanyInfo = new ShipCompanyInfo()
                {
                    Name = model.CompanyName,
                    DisplayOrder = model.DisplayOrder
                };

                AdminShipCompanies.CreateShipCompany(shipCompanyInfo);
                AddMallAdminLog("添加配送公司", "添加配送公司,配送公司为:" + model.CompanyName);
                return PromptView("配送公司添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑配送公司
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int shipCoId = -1)
        {
            ShipCompanyInfo shipCompanyInfo = AdminShipCompanies.GetShipCompanyById(shipCoId);
            if (shipCompanyInfo == null)
                return PromptView("配送公司不存在");

            ShipCompanyModel model = new ShipCompanyModel();
            model.DisplayOrder = shipCompanyInfo.DisplayOrder;
            model.CompanyName = shipCompanyInfo.Name;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑配送公司
        /// </summary>
        [HttpPost]
        public ActionResult Edit(ShipCompanyModel model, int shipCoId = -1)
        {
            ShipCompanyInfo shipCompanyInfo = AdminShipCompanies.GetShipCompanyById(shipCoId);
            if (shipCompanyInfo == null)
                return PromptView("配送公司不存在");

            int shipCoId2 = AdminShipCompanies.GetShipCoIdByName(model.CompanyName);
            if (shipCoId2 > 0 && shipCoId2 != shipCoId)
                ModelState.AddModelError("CompanyName", "名称已经存在");

            if (ModelState.IsValid)
            {
                shipCompanyInfo.DisplayOrder = model.DisplayOrder;
                shipCompanyInfo.Name = model.CompanyName;

                AdminShipCompanies.UpdateShipCompany(shipCompanyInfo);
                AddMallAdminLog("修改配送公司", "修改配送公司,配送公司ID为:" + shipCoId);
                return PromptView("配送公司修改成功");
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除配送公司
        /// </summary>
        public ActionResult Del(int shipCoId = -1)
        {
            AdminShipCompanies.DeleteShipCompanyById(shipCoId);
            AddMallAdminLog("删除配送公司", "删除配送公司,配送公司ID为:" + shipCoId);
            return PromptView("配送公司删除成功");
        }
    }
}
