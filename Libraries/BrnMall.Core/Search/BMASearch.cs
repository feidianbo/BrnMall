﻿using System;
using System.IO;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall搜索管理类
    /// </summary>
    public class BMASearch
    {
        private static ISearchStrategy _isearchstrategy = null;//搜索策略

        static BMASearch()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.SearchStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _isearchstrategy = (ISearchStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.SearchStrategy.{0}.SearchStrategy, BrnMall.SearchStrategy.{0}", fileNameList[0].Substring(fileNameList[0].IndexOf("SearchStrategy.") + 15).Replace(".dll", "")),
                                                                                          false,
                                                                                          true));
            }
            catch
            {
                throw new BMAException("创建'搜索策略对象'失败,可能存在的原因:未将'搜索策略对象'添加到bin目录中;'搜索策略对象'文件名不符合'BrnMall.SearchStrategy.{策略名称}.dll'格式");
            }
        }

        /// <summary>
        /// 搜索策略实例
        /// </summary>
        public static ISearchStrategy Instance
        {
            get { return _isearchstrategy; }
        }
    }
}
