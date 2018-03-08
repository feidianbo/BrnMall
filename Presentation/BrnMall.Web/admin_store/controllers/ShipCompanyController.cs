using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.StoreAdmin.Models;

namespace BrnMall.Web.StoreAdmin.Controllers
{
    /// <summary>
    /// 店铺配送公司控制器类
    /// </summary>
    public partial class ShipCompanyController : BaseStoreAdminController
    {
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
    }
}
