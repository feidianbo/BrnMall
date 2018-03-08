using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 配送公司数据访问类
    /// </summary>
    public partial class ShipCompanies
    {
        #region 辅助方法

        /// <summary>
        /// 通过IDataReader创建ShipCompanyInfo
        /// </summary>
        public static ShipCompanyInfo BuildShipCompanyFromReader(IDataReader reader)
        {
            ShipCompanyInfo shipCompanyInfo = new ShipCompanyInfo();

            shipCompanyInfo.ShipCoId = TypeHelper.ObjectToInt(reader["shipcoid"]);
            shipCompanyInfo.Name = reader["name"].ToString();
            shipCompanyInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);

            return shipCompanyInfo;
        }

        #endregion

        /// <summary>
        /// 获得配送公司列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public static List<ShipCompanyInfo> GetShipCompanyList(int pageSize, int pageNumber)
        {
            List<ShipCompanyInfo> shipCompanyList = new List<ShipCompanyInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetShipCompanyList(pageSize, pageNumber);
            while (reader.Read())
            {
                ShipCompanyInfo shipCompanyInfo = BuildShipCompanyFromReader(reader);
                shipCompanyList.Add(shipCompanyInfo);
            }

            reader.Close();
            return shipCompanyList;
        }

        /// <summary>
        /// 获得配送公司数量
        /// </summary>
        /// <returns></returns>
        public static int GetShipCompanyCount()
        {
            return BrnMall.Core.BMAData.RDBS.GetShipCompanyCount();
        }

        /// <summary>
        /// 获得配送公司
        /// </summary>
        /// <param name="shipCoId">配送公司id</param>
        /// <returns></returns>
        public static ShipCompanyInfo GetShipCompanyById(int shipCoId)
        {
            ShipCompanyInfo shipCompanyInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetShipCompanyById(shipCoId);
            if (reader.Read())
            {
                shipCompanyInfo = BuildShipCompanyFromReader(reader);
            }

            reader.Close();
            return shipCompanyInfo;
        }

        /// <summary>
        /// 创建配送公司
        /// </summary>
        public static void CreateShipCompany(ShipCompanyInfo shipCompanyInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateShipCompany(shipCompanyInfo);
        }

        /// <summary>
        /// 更新配送公司
        /// </summary>
        public static void UpdateShipCompany(ShipCompanyInfo shipCompanyInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateShipCompany(shipCompanyInfo);
        }

        /// <summary>
        /// 删除配送公司
        /// </summary>
        /// <param name="shipCoId">配送公司id</param>
        public static void DeleteShipCompanyById(int shipCoId)
        {
            BrnMall.Core.BMAData.RDBS.DeleteShipCompanyById(shipCoId);
        }

        /// <summary>
        /// 根据配送公司名称得到配送公司id
        /// </summary>
        /// <param name="shipCoName">配送公司名称</param>
        /// <returns></returns>
        public static int GetShipCoIdByName(string shipCoName)
        {
            return BrnMall.Core.BMAData.RDBS.GetShipCoIdByName(shipCoName);
        }
    }
}
