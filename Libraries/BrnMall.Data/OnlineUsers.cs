﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 在线用户数据访问类
    /// </summary>
    public partial class OnlineUsers
    {
        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建OnlineUserInfo
        /// </summary>
        public static OnlineUserInfo BuildOnlineUserFromReader(IDataReader reader)
        {
            OnlineUserInfo onlineUserInfo = new OnlineUserInfo();

            onlineUserInfo.OlId = TypeHelper.ObjectToInt(reader["olid"]);
            onlineUserInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            onlineUserInfo.Sid = reader["sid"].ToString();
            onlineUserInfo.NickName = reader["nickname"].ToString();
            onlineUserInfo.IP = reader["ip"].ToString();
            onlineUserInfo.RegionId = TypeHelper.ObjectToInt(reader["regionid"]);
            onlineUserInfo.UpdateTime = TypeHelper.ObjectToDateTime(reader["updatetime"]);

            return onlineUserInfo;
        }

        #endregion

        /// <summary>
        /// 创建在线用户
        /// </summary>
        public static int CreateOnlineUser(OnlineUserInfo onlineUserInfo)
        {
            return BrnMall.Core.BMAData.RDBS.CreateOnlineUser(onlineUserInfo);
        }

        /// <summary>
        /// 更新在线用户ip
        /// </summary>
        /// <param name="olId">在线用户id</param>
        /// <param name="ip">ip</param>
        public static void UpdateOnlineUserIP(int olId, string ip)
        {
            BrnMall.Core.BMAData.RDBS.UpdateOnlineUserIP(olId, ip);
        }

        /// <summary>
        /// 更新在线用户uid
        /// </summary>
        /// <param name="olId">在线用户id</param>
        /// <param name="ip">uid</param>
        public static void UpdateOnlineUserUid(int olId, int uid)
        {
            BrnMall.Core.BMAData.RDBS.UpdateOnlineUserUid(olId, uid);
        }

        /// <summary>
        /// 获得在线用户
        /// </summary>
        /// <param name="sid">sessionId</param>
        /// <returns></returns>
        public static OnlineUserInfo GetOnlineUserBySid(string sid)
        {
            OnlineUserInfo onlineUserInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetOnlineUserBySid(sid);
            if (reader.Read())
            {
                onlineUserInfo = BuildOnlineUserFromReader(reader);
            }

            reader.Close();
            return onlineUserInfo;
        }

        /// <summary>
        /// 获得在线用户数量
        /// </summary>
        /// <param name="userType">在线用户类型</param>
        /// <returns></returns>
        public static int GetOnlineUserCount(int userType)
        {
            return BrnMall.Core.BMAData.RDBS.GetOnlineUserCount(userType);
        }

        /// <summary>
        /// 删除在线用户
        /// </summary>
        /// <param name="sid">sessionId</param>
        public static void DeleteOnlineUserBySid(string sid)
        {
            BrnMall.Core.BMAData.RDBS.DeleteOnlineUserBySid(sid);
        }

        /// <summary>
        /// 删除过期在线用户
        /// </summary>
        /// <param name="onlineUserExpire">过期时间</param>
        public static void DeleteExpiredOnlineUser(int onlineUserExpire)
        {
            BrnMall.Core.BMAData.RDBS.DeleteExpiredOnlineUser(onlineUserExpire);
        }

        /// <summary>
        /// 重置在线用户表
        /// </summary>
        public static void ResetOnlineUserTable()
        {
            BrnMall.Core.BMAData.RDBS.ResetOnlineUserTable();
        }

        /// <summary>
        /// 获得在线用户列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="locationType">位置类型(0代表省,1代表市,2代表区或县)</param>
        /// <param name="locationId">位置id</param>
        /// <returns></returns>
        public static List<OnlineUserInfo> GetOnlineUserList(int pageSize, int pageNumber, int locationType, int locationId)
        {
            List<OnlineUserInfo> onlineUserList = new List<OnlineUserInfo>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetOnlineUserList(pageSize, pageNumber, locationType, locationId);
            while (reader.Read())
            {
                OnlineUserInfo onlineUserInfo = BuildOnlineUserFromReader(reader);
                onlineUserList.Add(onlineUserInfo);
            }

            reader.Close();
            return onlineUserList;
        }

        /// <summary>
        /// 获得在线用户数量
        /// </summary>
        /// <param name="locationType">位置类型(0代表省,1代表市,2代表区或县)</param>
        /// <param name="locationId">位置id</param>
        /// <returns></returns>
        public static int GetOnlineUserCount(int locationType, int locationId)
        {
            return BrnMall.Core.BMAData.RDBS.GetOnlineUserCount(locationType, locationId);
        }
    }
}
