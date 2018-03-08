﻿using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 在线用户操作管理类
    /// </summary>
    public partial class OnlineUsers
    {
        //锁对象
        private static object _locker = new object();
        //最后一次删除过期在线用户的时间
        private static int _lastdeleteexpiredonlineuserstime = 0;

        /// <summary>
        /// 创建在线会员
        /// </summary>
        /// <param name="uid">会员id</param>
        /// <param name="sid">sessionId</param>
        /// <param name="nickName">会员昵称</param>
        /// <param name="updateTime">更新时间</param>
        /// <param name="ip">ip</param>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public static int CreateOnlineMember(int uid, string sid, string nickName, DateTime updateTime, string ip, int regionId)
        {
            OnlineUserInfo onlineUserInfo = new OnlineUserInfo();
            onlineUserInfo.Uid = uid;
            onlineUserInfo.Sid = sid;
            onlineUserInfo.NickName = nickName;
            onlineUserInfo.IP = ip;
            onlineUserInfo.RegionId = regionId;
            onlineUserInfo.UpdateTime = updateTime;

            int olid = BrnMall.Data.OnlineUsers.CreateOnlineUser(onlineUserInfo);

            //更新用户最后访问信息
            Users.UpdateUserLastVisit(uid, updateTime, ip, regionId);
            return olid;
        }

        /// <summary>
        /// 创建在线游客
        /// </summary>
        /// <param name="sid">sessionId</param>
        /// <param name="updateTime">更新时间</param>
        /// <param name="ip">ip</param>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public static int CreateOnlineGuest(string sid, DateTime updateTime, string ip, int regionId)
        {
            OnlineUserInfo onlineUserInfo = new OnlineUserInfo();
            onlineUserInfo.Uid = -1;
            onlineUserInfo.Sid = sid;
            onlineUserInfo.NickName = "游客";
            onlineUserInfo.IP = ip;
            onlineUserInfo.RegionId = regionId;
            onlineUserInfo.UpdateTime = updateTime;

            return BrnMall.Data.OnlineUsers.CreateOnlineUser(onlineUserInfo);
        }

        /// <summary>
        /// 更新在线用户
        /// </summary>
        /// <param name="state">UpdateOnlineUserState类型对象</param>
        public static void UpdateOnlineUser(object state)
        {
            lock (_locker)
            {
                UpdateOnlineUserState updateOnlineUserState = (UpdateOnlineUserState)state;

                OnlineUserInfo onlineUserInfo = GetOnlineUserBySid(updateOnlineUserState.Sid);
                if (onlineUserInfo != null)
                {
                    if (onlineUserInfo.IP != updateOnlineUserState.IP)
                        UpdateOnlineUserIP(onlineUserInfo.OlId, updateOnlineUserState.IP);

                    if (onlineUserInfo.Uid != updateOnlineUserState.Uid)
                        UpdateOnlineUserUid(onlineUserInfo.OlId, updateOnlineUserState.Uid);

                    DeleteExpiredOnlineUser();
                }
                else
                {
                    int olid = 0;
                    if (updateOnlineUserState.Uid > 0)
                        olid = CreateOnlineMember(updateOnlineUserState.Uid, updateOnlineUserState.Sid, updateOnlineUserState.NickName, updateOnlineUserState.UpdateTime, updateOnlineUserState.IP, updateOnlineUserState.RegionId);
                    else
                        olid = CreateOnlineGuest(updateOnlineUserState.Sid, updateOnlineUserState.UpdateTime, updateOnlineUserState.IP, updateOnlineUserState.RegionId);

                    if (olid < 2147000000)
                        DeleteExpiredOnlineUser();
                    else
                        ResetOnlineUserTable();
                }
            }
        }

        /// <summary>
        /// 更新在线用户ip
        /// </summary>
        /// <param name="olId">在线用户id</param>
        /// <param name="ip">ip</param>
        public static void UpdateOnlineUserIP(int olId, string ip)
        {
            BrnMall.Data.OnlineUsers.UpdateOnlineUserIP(olId, ip);
        }

        /// <summary>
        /// 更新在线用户uid
        /// </summary>
        /// <param name="olId">在线用户id</param>
        /// <param name="ip">uid</param>
        public static void UpdateOnlineUserUid(int olId, int uid)
        {
            BrnMall.Data.OnlineUsers.UpdateOnlineUserUid(olId, uid);
        }

        /// <summary>
        /// 获得在线用户
        /// </summary>
        /// <param name="sid">sessionId</param>
        /// <returns></returns>
        public static OnlineUserInfo GetOnlineUserBySid(string sid)
        {
            return BrnMall.Data.OnlineUsers.GetOnlineUserBySid(sid);
        }

        /// <summary>
        /// 获得在线用户数量
        /// </summary>
        /// <param name="userType">在线用户类型</param>
        /// <returns></returns>
        public static int GetOnlineUserCount(int userType)
        {
            return BrnMall.Data.OnlineUsers.GetOnlineUserCount(userType);
        }

        /// <summary>
        /// 获得全部在线用户数量
        /// </summary>
        /// <returns></returns>
        public static int GetOnlineUserCount()
        {
            int onlineUserExpire = BMAConfig.MallConfig.OnlineCountExpire;
            if (onlineUserExpire == 0)
                return GetOnlineUserCount(0);

            int onlineAllUserCount = TypeHelper.ObjectToInt(BrnMall.Core.BMACache.Get(CacheKeys.MALL_ONLINEUSER_ALLUSERCOUNT), -1);
            if (onlineAllUserCount == -1)
            {
                onlineAllUserCount = GetOnlineUserCount(0);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_ONLINEUSER_ALLUSERCOUNT, onlineAllUserCount);
            }
            return onlineAllUserCount;
        }

        /// <summary>
        /// 获得在线游客数量
        /// </summary>
        /// <returns></returns>
        public static int GetOnlineGuestCount()
        {
            int onlineUserExpire = BMAConfig.MallConfig.OnlineCountExpire;
            if (onlineUserExpire == 0)
                return GetOnlineUserCount(-1);

            int onlineGuestCount = TypeHelper.ObjectToInt(BrnMall.Core.BMACache.Get(CacheKeys.MALL_ONLINEUSER_GUESTCOUNT), -1);
            if (onlineGuestCount == -1)
            {
                onlineGuestCount = GetOnlineUserCount(-1);
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_ONLINEUSER_GUESTCOUNT, onlineGuestCount);
            }
            return onlineGuestCount;
        }

        /// <summary>
        /// 删除在线用户
        /// </summary>
        /// <param name="sid">sessionId</param>
        public static void DeleteOnlineUserBySid(string sid)
        {
            BrnMall.Data.OnlineUsers.DeleteOnlineUserBySid(sid);
        }

        /// <summary>
        /// 删除过期在线用户
        /// </summary>
        public static void DeleteExpiredOnlineUser()
        {
            if (_lastdeleteexpiredonlineuserstime < (Environment.TickCount - BMAConfig.MallConfig.OnlineUserExpire * 1000 * 60) || _lastdeleteexpiredonlineuserstime == 0)
            {
                BrnMall.Data.OnlineUsers.DeleteExpiredOnlineUser(BMAConfig.MallConfig.OnlineUserExpire);
                _lastdeleteexpiredonlineuserstime = Environment.TickCount;
            }
        }

        /// <summary>
        /// 重置在线用户表
        /// </summary>
        public static void ResetOnlineUserTable()
        {
            try
            {
                BrnMall.Data.OnlineUsers.ResetOnlineUserTable();
            }
            catch
            {
                try
                {
                    BrnMall.Data.OnlineUsers.ResetOnlineUserTable();
                }
                catch
                {
                }
            }
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
            return BrnMall.Data.OnlineUsers.GetOnlineUserList(pageSize, pageNumber, locationType, locationId);
        }

        /// <summary>
        /// 获得在线用户数量
        /// </summary>
        /// <param name="locationType">位置类型(0代表省,1代表市,2代表区或县)</param>
        /// <param name="locationId">位置id</param>
        /// <returns></returns>
        public static int GetOnlineUserCount(int locationType, int locationId)
        {
            return BrnMall.Data.OnlineUsers.GetOnlineUserCount(locationType, locationId);
        }
    }
}
