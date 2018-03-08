﻿using System;
using System.Data;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 活动专题数据访问类
    /// </summary>
    public partial class Topics
    {
        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建TopicInfo
        /// </summary>
        public static TopicInfo BuildTopicFromReader(IDataReader reader)
        {
            TopicInfo topicInfo = new TopicInfo();

            topicInfo.TopicId = TypeHelper.ObjectToInt(reader["topicid"]);
            topicInfo.StartTime = TypeHelper.ObjectToDateTime(reader["starttime"]);
            topicInfo.EndTime = TypeHelper.ObjectToDateTime(reader["endtime"]);
            topicInfo.IsShow = TypeHelper.ObjectToInt(reader["isshow"]);
            topicInfo.SN = reader["sn"].ToString();
            topicInfo.Title = reader["title"].ToString();
            topicInfo.HeadHtml = reader["headhtml"].ToString();
            topicInfo.BodyHtml = reader["bodyhtml"].ToString();

            return topicInfo;
        }

        #endregion

        /// <summary>
        /// 创建活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void CreateTopic(TopicInfo topicInfo)
        {
            BrnMall.Core.BMAData.RDBS.CreateTopic(topicInfo);
        }

        /// <summary>
        /// 更新活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void UpdateTopic(TopicInfo topicInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateTopic(topicInfo);
        }

        /// <summary>
        /// 删除活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        public static void DeleteTopicById(int topicId)
        {
            BrnMall.Core.BMAData.RDBS.DeleteTopicById(topicId);
        }

        /// <summary>
        /// 后台获得活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        /// <returns></returns>
        public static TopicInfo AdminGetTopicById(int topicId)
        {
            TopicInfo topicInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.AdminGetTopicById(topicId);
            if (reader.Read())
            {
                topicInfo = BuildTopicFromReader(reader);
            }
            reader.Close();
            return topicInfo;
        }

        /// <summary>
        /// 后台获得活动专题列表条件
        /// </summary>
        /// <param name="sn">编号</param>
        /// <param name="title">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static string AdminGetTopicListCondition(string sn, string title, string startTime, string endTime)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetTopicListCondition(sn, title, startTime, endTime);
        }

        /// <summary>
        /// 后台获得活动专题列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetTopicList(int pageSize, int pageNumber, string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetTopicList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得活动专题数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetTopicCount(string condition)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetTopicCount(condition);
        }

        /// <summary>
        /// 判断活动专题编号是否存在
        /// </summary>
        /// <param name="topicSN">活动专题编号</param>
        /// <returns></returns>
        public static bool IsExistTopicSN(string topicSN)
        {
            return BrnMall.Core.BMAData.RDBS.IsExistTopicSN(topicSN);
        }

        /// <summary>
        /// 获得活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static TopicInfo GetTopicByIdAndTime(int topicId, DateTime nowTime)
        {
            TopicInfo topicInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetTopicByIdAndTime(topicId, nowTime);
            if (reader.Read())
            {
                topicInfo = BuildTopicFromReader(reader);
            }
            reader.Close();
            return topicInfo;
        }

        /// <summary>
        /// 获得活动专题
        /// </summary>
        /// <param name="topicSN">活动专题编号</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static TopicInfo GetTopicBySNAndTime(string topicSN, DateTime nowTime)
        {
            TopicInfo topicInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetTopicBySNAndTime(topicSN, nowTime);
            if (reader.Read())
            {
                topicInfo = BuildTopicFromReader(reader);
            }
            reader.Close();
            return topicInfo;
        }
    }
}
