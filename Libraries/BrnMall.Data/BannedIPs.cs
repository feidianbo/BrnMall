﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Data
{
    /// <summary>
    /// 禁止IP数据访问类
    /// </summary>
    public partial class BannedIPs
    {
        /// <summary>
        /// 获得禁止的ip列表
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetBannedIPList()
        {
            HashSet<string> ipList = new HashSet<string>();
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetBannedIPList();
            while (reader.Read())
            {
                ipList.Add(reader["ip"].ToString());
            }
            reader.Close();
            return ipList;
        }

        /// <summary>
        /// 获得禁止的ip
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static BannedIPInfo GetBannedIPById(int id)
        {
            BannedIPInfo bannedIPInfo = null;
            IDataReader reader = BrnMall.Core.BMAData.RDBS.GetBannedIPById(id);
            if (reader.Read())
            {
                bannedIPInfo = new BannedIPInfo();
                bannedIPInfo.Id = TypeHelper.ObjectToInt(reader["id"]);
                bannedIPInfo.IP = reader["ip"].ToString();
                bannedIPInfo.LiftBanTime = TypeHelper.ObjectToDateTime(reader["liftbantime"]);
            }

            reader.Close();
            return bannedIPInfo;
        }

        /// <summary>
        /// 获得禁止IP的id
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static int GetBannedIPIdByIP(string ip)
        {
            return BrnMall.Core.BMAData.RDBS.GetBannedIPIdByIP(ip);
        }

        /// <summary>
        /// 添加禁止的ip
        /// </summary>
        public static void AddBannedIP(BannedIPInfo bannedIPInfo)
        {
            BrnMall.Core.BMAData.RDBS.AddBannedIP(bannedIPInfo);
        }

        /// <summary>
        /// 更新禁止的ip
        /// </summary>
        public static void UpdateBannedIP(BannedIPInfo bannedIPInfo)
        {
            BrnMall.Core.BMAData.RDBS.UpdateBannedIP(bannedIPInfo);
        }

        /// <summary>
        /// 删除禁止的ip
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void DeleteBannedIPById(string idList)
        {
            BrnMall.Core.BMAData.RDBS.DeleteBannedIPById(idList);
        }

        /// <summary>
        /// 后台获得禁止的ip列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static DataTable AdminGetBannedIPList(int pageSize, int pageNumber, string ip)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBannedIPList(pageSize, pageNumber, ip);
        }

        /// <summary>
        /// 后台获得禁止的ip数量
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static int AdminGetBannedIPCount(string ip)
        {
            return BrnMall.Core.BMAData.RDBS.AdminGetBannedIPCount(ip);
        }
    }
}
