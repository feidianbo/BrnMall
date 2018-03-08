﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 品牌数据访问类
    /// </summary>
    public partial class Brands
    {
        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建BrandInfo
        /// </summary>
        public static BrandInfo BuildBrandFromReader(IDataReader reader)
        {
            BrandInfo brandInfo = new BrandInfo();

            brandInfo.BrandId = TypeHelper.ObjectToInt(reader["brandid"]);
            brandInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);
            brandInfo.Name = reader["name"].ToString();
            brandInfo.Logo = reader["logo"].ToString();

            return brandInfo;
        }

        #endregion

        /// <summary>
        /// 获得品牌
        /// </summary>
        /// <param name="brandId">品牌id</param>
        /// <returns></returns>
        public static BrandInfo GetBrandById(int brandId)
        {
            BrandInfo brandInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetBrandById(brandId);
            if (reader.Read())
            {
                brandInfo = BuildBrandFromReader(reader);
            }

            reader.Close();
            return brandInfo;
        }

        /// <summary>
        /// 更新品牌
        /// </summary>
        /// <param name="brandInfo"></param>
        public static void UpdateBrand(BrandInfo brandInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateBrand(brandInfo);
        }

        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="brandInfo"></param>
        public static void CreateBrand(BrandInfo brandInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateBrand(brandInfo);
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        /// <param name="brandId">品牌id</param>
        public static void DeleteBrandById(int brandId)
        {
            BrnMall.Core.BMAData.RDBS.DeleteBrandById(brandId);
        }

        /// <summary>
        /// 后台获得列表搜索条件
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public static string AdminGetBrandListCondition(string brandName)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBrandListCondition(brandName);
        }

        /// <summary>
        /// 后台获得品牌列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetBrandList(int pageSize, int pageNumber, string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBrandList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得品牌选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetBrandSelectList(int pageSize, int pageNumber, string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBrandSelectList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得品牌数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetBrandCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBrandCount(condition);
        }

        /// <summary>
        /// 根据品牌名称得到品牌id
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public static int GetBrandIdByName(string brandName)
        {
            return BrnMall.Core.BMAData.RDBS.GetBrandIdByName(brandName);
        }

        /// <summary>
        /// 获得品牌列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public static List<BrandInfo> GetBrandList(int pageSize, int pageNumber, string brandName)
        {
            List<BrandInfo> brandList = new List<BrandInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetBrandList(pageSize, pageNumber, brandName);
            while (reader.Read())
            {
                BrandInfo brandInfo = BuildBrandFromReader(reader);
                brandList.Add(brandInfo);
            }

            reader.Close();
            return brandList;
        }

        /// <summary>
        /// 获得品牌数量
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public static int GetBrandCount(string brandName)
        {
            return BrnMall.Core.BMAData.RDBS.GetBrandCount(brandName);
        }
    }
}
