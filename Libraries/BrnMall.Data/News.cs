﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 新闻数据访问类
    /// </summary>
    public partial class News
    {
        #region 辅助方法

        /// <summary>
        /// 通过IDataReader创建NewsTypeInfo
        /// </summary>
        public static NewsTypeInfo BuildNewsTypeFromReader(IDataReader reader)
        {
            NewsTypeInfo newsTypeInfo = new NewsTypeInfo();

            newsTypeInfo.NewsTypeId = TypeHelper.ObjectToInt(reader["newstypeid"]);
            newsTypeInfo.Name = reader["name"].ToString();
            newsTypeInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);

            return newsTypeInfo;
        }

        /// <summary>
        /// 通过IDataReader创建NewsInfo
        /// </summary>
        public static NewsInfo BuildNewsFromReader(IDataReader reader)
        {
            NewsInfo newsInfo = new NewsInfo();

            newsInfo.NewsId = TypeHelper.ObjectToInt(reader["newsid"]);
            newsInfo.NewsTypeId = TypeHelper.ObjectToInt(reader["newstypeid"]);
            newsInfo.IsShow = TypeHelper.ObjectToInt(reader["isshow"]);
            newsInfo.IsTop = TypeHelper.ObjectToInt(reader["istop"]);
            newsInfo.IsHome = TypeHelper.ObjectToInt(reader["ishome"]);
            newsInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);
            newsInfo.AddTime = TypeHelper.ObjectToDateTime(reader["addtime"]);
            newsInfo.Title = reader["title"].ToString();
            newsInfo.Url = reader["url"].ToString();
            newsInfo.Body = reader["body"].ToString();

            return newsInfo;
        }

        #endregion

        /// <summary>
        /// 创建新闻类型
        /// </summary>
        public static void CreateNewsType(NewsTypeInfo newsTypeInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateNewsType(newsTypeInfo);
        }

        /// <summary>
        /// 删除新闻类型
        /// </summary>
        /// <param name="newsTypeId">新闻类型id</param>
        public static void DeleteNewsTypeById(int newsTypeId)
        {
            BrnMall.Core.BMAData.RDBS.DeleteNewsTypeById(newsTypeId);
        }

        /// <summary>
        /// 更新新闻类型
        /// </summary>
        public static void UpdateNewsType(NewsTypeInfo newsTypeInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateNewsType(newsTypeInfo);
        }

        /// <summary>
        /// 获得新闻类型列表
        /// </summary>
        /// <returns></returns>
        public static List<NewsTypeInfo> GetNewsTypeList()
        {
            List<NewsTypeInfo> newsTypeList = new List<NewsTypeInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetNewsTypeList();
            while (reader.Read())
            {
                NewsTypeInfo newsTypeInfo = BuildNewsTypeFromReader(reader);
                newsTypeList.Add(newsTypeInfo);
            }

            reader.Close();
            return newsTypeList;
        }




        /// <summary>
        /// 创建新闻
        /// </summary>
        public static void CreateNews(NewsInfo newsInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateNews(newsInfo);
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="newsIdList">新闻id列表</param>
        public static void DeleteNewsById(string newsIdList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteNewsById(newsIdList);
        }

        /// <summary>
        /// 更新新闻
        /// </summary>
        public static void UpdateNews(NewsInfo newsInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateNews(newsInfo);
        }

        /// <summary>
        /// 获得新闻
        /// </summary>
        /// <param name="newsId">新闻id</param>
        /// <returns></returns>
        public static NewsInfo GetNewsById(int newsId)
        {
            NewsInfo newsInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetNewsById(newsId);
            if (reader.Read())
            {
                newsInfo = BuildNewsFromReader(reader);
            }

            reader.Close();
            return newsInfo;
        }

        /// <summary>
        /// 后台获得新闻
        /// </summary>
        /// <param name="newsId">新闻id</param>
        /// <returns></returns>
        public static NewsInfo AdminGetNewsById(int newsId)
        {
            NewsInfo newsInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetNewsById(newsId);
            if (reader.Read())
            {
                newsInfo = BuildNewsFromReader(reader);
            }

            reader.Close();
            return newsInfo;
        }

        /// <summary>
        /// 后台获得新闻列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetNewsList(int pageSize, int pageNumber, string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetNewsList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得新闻列表搜索条件
        /// </summary>
        /// <param name="newsTypeId">新闻类型id</param>
        /// <param name="title">新闻标题</param>
        /// <returns></returns>
        public static string AdminGetNewsListCondition(int newsTypeId, string title)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetNewsListCondition(newsTypeId, title);
        }

        /// <summary>
        /// 后台获得新闻数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetNewsCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetNewsCount(condition);
        }

        /// <summary>
        /// 后台根据新闻标题得到新闻id
        /// </summary>
        /// <param name="title">新闻标题</param>
        /// <returns></returns>
        public static int AdminGetNewsIdByTitle(string title)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetNewsIdByTitle(title);
        }

        /// <summary>
        /// 获得置首的新闻列表
        /// </summary>
        /// <param name="newsTypeId">新闻类型id(0代表全部类型)</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static DataTable GetHomeNewsList(int newsTypeId, int count)
        {
            return BrnMall.Core.BMAData.RDBS.GetHomeNewsList(newsTypeId, count);
        }

        /// <summary>
        /// 获得新闻列表条件
        /// </summary>
        /// <param name="newsTypeId">新闻类型id(0代表全部类型)</param>
        /// <param name="title">新闻标题</param>
        /// <returns></returns>
        public static string GetNewsListCondition(int newsTypeId, string title)
        {
            return BrnMall.Core.BMAData.RDBS.GetNewsListCondition(newsTypeId, title);
        }

        /// <summary>
        /// 获得新闻列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable GetNewsList(int pageSize, int pageNumber, string condition)
        {
            return BrnMall.Core.BMAData.RDBS.GetNewsList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 获得新闻数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetNewsCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.GetNewsCount(condition);
        }
    }
}
