using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 配送公司操作管理类
    /// </summary>
    public partial class ShipCompanies
    {
        /// <summary>
        /// 获得配送公司列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public static List<ShipCompanyInfo> GetShipCompanyList(int pageSize, int pageNumber)
        {
            return BrnMall.Data.ShipCompanies.GetShipCompanyList(pageSize, pageNumber);
        }

        /// <summary>
        /// 获得配送公司数量
        /// </summary>
        /// <returns></returns>
        public static int GetShipCompanyCount()
        {
            return BrnMall.Data.ShipCompanies.GetShipCompanyCount();
        }

        /// <summary>
        /// 获得配送公司
        /// </summary>
        /// <param name="shipCoId">配送公司id</param>
        /// <returns></returns>
        public static ShipCompanyInfo GetShipCompanyById(int shipCoId)
        {
            ShipCompanyInfo shipCompanyInfo = BrnMall.Core.BMACache.Get(CacheKeys.MALL_SHIPCOMPANY_INFO + shipCoId) as ShipCompanyInfo;
            if (shipCompanyInfo == null)
            {
                shipCompanyInfo = BrnMall.Data.ShipCompanies.GetShipCompanyById(shipCoId);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_SHIPCOMPANY_INFO + shipCoId, shipCompanyInfo);
            }

            return shipCompanyInfo;
        }
    }
}
