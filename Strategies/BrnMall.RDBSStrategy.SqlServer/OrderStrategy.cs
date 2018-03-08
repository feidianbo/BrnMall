﻿using System;
using System.Data;
using System.Text;
using System.Data.Common;

using BrnMall.Core;

namespace BrnMall.RDBSStrategy.SqlServer
{
    /// <summary>
    /// SqlServer策略之订单分部类
    /// </summary>
    public partial class RDBSStrategy : IRDBSStrategy
    {
        #region 订单

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns>订单信息</returns>
        public IDataReader GetOrderByOid(int oid)
        {
            DbParameter[] parms = {
	                                 GenerateInParam("@oid", SqlDbType.Int,4,oid)
                                    };
            return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
                                            string.Format("{0}getorderbyoid", RDBSHelper.RDBSTablePre),
                                            parms);
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="osn">订单编号</param>
        /// <returns>订单信息</returns>
        public IDataReader GetOrderByOSN(string osn)
        {
            DbParameter[] parms = {
	                                 GenerateInParam("@osn", SqlDbType.Char,30,osn)
                                    };
            return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
                                            string.Format("{0}getorderbyosn", RDBSHelper.RDBSTablePre),
                                            parms);
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public int GetOrderCountByCondition(int storeId, int orderState, string startTime, string endTime)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0}", storeId);
            if (orderState > 0)
                condition.AppendFormat(" AND [orderstate] = {0}", orderState);
            if (!string.IsNullOrEmpty(startTime))
                condition.AppendFormat(" AND [addtime] >= '{0}'", TypeHelper.StringToDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"));
            if (!string.IsNullOrEmpty(endTime))
                condition.AppendFormat(" AND [addtime] <= '{0}'", TypeHelper.StringToDateTime(endTime).ToString("yyyy-MM-dd HH:mm:ss"));

            string commandText;
            if (condition.Length > 0)
            {
                commandText = string.Format("SELECT COUNT([oid]) FROM [{0}orders] WHERE {1}",
                                             RDBSHelper.RDBSTablePre,
                                             condition.Remove(0, 4).ToString());
            }
            else
            {
                commandText = string.Format("SELECT COUNT([oid]) FROM [{0}orders]",
                                            RDBSHelper.RDBSTablePre);
            }

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 获得订单列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public DataTable GetOrderList(int pageSize, int pageNumber, string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT [temp1].[oid],[temp1].[osn],[temp1].[uid],[temp1].[orderstate],[temp1].[orderamount],[temp1].[surplusmoney],[temp1].[parentid],[temp1].[isreview],[temp1].[addtime],[temp1].[storeid],[temp1].[storename],[temp1].[paymode],[temp1].[regionid],[temp1].[consignee],[temp1].[mobile],[temp1].[phone],[temp1].[email],[temp1].[zipcode],[temp1].[address],[temp1].[besttime],[temp2].[username],[temp3].[provincename],[temp3].[cityname],[temp3].[name] AS [countyname] FROM (SELECT TOP {0} [oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM [{1}orders] ORDER BY [oid] DESC) AS [temp1] LEFT JOIN [{1}users] AS [temp2] ON [temp1].[uid]=[temp2].[uid] LEFT JOIN [{1}regions] AS [temp3] ON [temp1].[regionid]=[temp3].[regionid]",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT [temp1].[oid],[temp1].[osn],[temp1].[uid],[temp1].[orderstate],[temp1].[orderamount],[temp1].[surplusmoney],[temp1].[parentid],[temp1].[isreview],[temp1].[addtime],[temp1].[storeid],[temp1].[storename],[temp1].[paymode],[temp1].[regionid],[temp1].[consignee],[temp1].[mobile],[temp1].[phone],[temp1].[email],[temp1].[zipcode],[temp1].[address],[temp1].[besttime],[temp2].[username],[temp3].[provincename],[temp3].[cityname],[temp3].[name] AS [countyname] FROM (SELECT TOP {0} [oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM [{1}orders] WHERE {2} ORDER BY [oid] DESC) AS [temp1] LEFT JOIN [{1}users] AS [temp2] ON [temp1].[uid]=[temp2].[uid] LEFT JOIN [{1}regions] AS [temp3] ON [temp1].[regionid]=[temp3].[regionid]",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT [temp1].[oid],[temp1].[osn],[temp1].[uid],[temp1].[orderstate],[temp1].[orderamount],[temp1].[surplusmoney],[temp1].[parentid],[temp1].[isreview],[temp1].[addtime],[temp1].[storeid],[temp1].[storename],[temp1].[paymode],[temp1].[regionid],[temp1].[consignee],[temp1].[mobile],[temp1].[phone],[temp1].[email],[temp1].[zipcode],[temp1].[address],[temp1].[besttime],[temp2].[username],[temp3].[provincename],[temp3].[cityname],[temp3].[name] AS [countyname] FROM (SELECT [oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM (SELECT TOP {0} ROW_NUMBER() OVER (ORDER BY [oid] DESC) AS [rowid],[oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM [{1}orders]) AS [temp] WHERE [temp].[rowid] BETWEEN {2} AND {3}) AS [temp1] LEFT JOIN [{1}users] AS [temp2] ON [temp1].[uid]=[temp2].[uid] LEFT JOIN [{1}regions] AS [temp3] ON [temp1].[regionid]=[temp3].[regionid]",
                                                pageSize * pageNumber,
                                                RDBSHelper.RDBSTablePre,
                                                pageSize * (pageNumber - 1) + 1,
                                                pageSize * pageNumber);

                else
                    commandText = string.Format("SELECT [temp1].[oid],[temp1].[osn],[temp1].[uid],[temp1].[orderstate],[temp1].[orderamount],[temp1].[surplusmoney],[temp1].[parentid],[temp1].[isreview],[temp1].[addtime],[temp1].[storeid],[temp1].[storename],[temp1].[paymode],[temp1].[regionid],[temp1].[consignee],[temp1].[mobile],[temp1].[phone],[temp1].[email],[temp1].[zipcode],[temp1].[address],[temp1].[besttime],[temp2].[username],[temp3].[provincename],[temp3].[cityname],[temp3].[name] AS [countyname] FROM (SELECT [oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM (SELECT TOP {0} ROW_NUMBER() OVER (ORDER BY [oid] DESC) AS [rowid],[oid],[osn],[uid],[orderstate],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[paymode],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime] FROM [{1}orders] WHERE {4}) AS [temp] WHERE [temp].[rowid] BETWEEN {2} AND {3}) AS [temp1] LEFT JOIN [{1}users] AS [temp2] ON [temp1].[uid]=[temp2].[uid] LEFT JOIN [{1}regions] AS [temp3] ON [temp1].[regionid]=[temp3].[regionid]",
                                                pageSize * pageNumber,
                                                RDBSHelper.RDBSTablePre,
                                                pageSize * (pageNumber - 1) + 1,
                                                pageSize * pageNumber,
                                                condition);
            }

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }

        /// <summary>
        /// 获得订单列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <param name="uid">用户id</param>
        /// <param name="consignee">收货人</param>
        /// <param name="orderState">订单状态</param>
        /// <returns></returns>
        public string GetOrderListCondition(int storeId, string osn, int uid, string consignee, int orderState)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn) && SecureHelper.IsSafeSqlString(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            if (uid > 0)
                condition.AppendFormat(" AND [uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(consignee) && SecureHelper.IsSafeSqlString(consignee))
                condition.AppendFormat(" AND [consignee] like '{0}%' ", consignee);
            if (orderState > 0)
                condition.AppendFormat(" AND [orderstate] = {0} ", orderState);

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int GetOrderCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(oid) FROM [{0}orders]", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(oid) FROM [{0}orders] WHERE {1}", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public IDataReader GetOrderProductList(int oid)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid)    
                                   };
            return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
                                            string.Format("{0}getorderproductlistbyoid", RDBSHelper.RDBSTablePre),
                                            parms);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oidList">订单id列表</param>
        /// <returns></returns>
        public IDataReader GetOrderProductList(string oidList)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oidlist", SqlDbType.NVarChar, 1000, oidList)    
                                   };
            return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
                                            string.Format("{0}getorderproductlistbyoidlist", RDBSHelper.RDBSTablePre),
                                            parms);
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="discount">折扣</param>
        /// <param name="surplusMoney">剩余金额</param>
        public void UpdateOrderDiscount(int oid, decimal discount, decimal surplusMoney)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@discount", SqlDbType.Decimal, 8, discount), 
                                    GenerateInParam("@surplusmoney", SqlDbType.Decimal, 8, surplusMoney)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}updateorderdiscount", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">配送费用</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        public void UpdateOrderShipFee(int oid, decimal shipFee, decimal orderAmount, decimal surplusMoney)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@shipfee", SqlDbType.Decimal, 8, shipFee), 
                                    GenerateInParam("@orderamount", SqlDbType.Decimal, 8, orderAmount),
                                    GenerateInParam("@surplusmoney", SqlDbType.Decimal, 8, surplusMoney)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}updateordershipfee", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public void UpdateOrderState(int oid, OrderState orderState)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@orderstate", SqlDbType.TinyInt, 1, (int)orderState)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}updateorderstate", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="shipSN">配送单号</param>
        /// <param name="shipCoId">配送公司id</param>
        /// <param name="shipCoName">配送公司名称</param>
        /// <param name="shipTime">配送时间</param>
        public void SendOrderProduct(int oid, OrderState orderState, string shipSN, int shipCoId, string shipCoName, DateTime shipTime)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@orderstate", SqlDbType.TinyInt, 1, (int)orderState),
                                    GenerateInParam("@shipsn", SqlDbType.Char, 30, shipSN),
                                    GenerateInParam("@shipcoid", SqlDbType.SmallInt, 2, shipCoId), 
                                    GenerateInParam("@shipconame", SqlDbType.NChar, 30, shipCoName), 
                                    GenerateInParam("@shiptime", SqlDbType.DateTime, 8, shipTime)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}sendorderproduct", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="paySN">支付单号</param>
        /// <param name="payTime">支付时间</param>
        public void PayOrder(int oid, OrderState orderState, string paySN, DateTime payTime)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@orderstate", SqlDbType.TinyInt, 1, (int)orderState),
                                    GenerateInParam("@paysn", SqlDbType.Char, 30, paySN),
                                    GenerateInParam("@paytime", SqlDbType.DateTime, 8, payTime)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}payorder", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 更新订单的评价
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="isReview">是否评价</param>
        public void UpdateOrderIsReview(int oid, int isReview)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@oid", SqlDbType.Int, 4, oid), 
                                    GenerateInParam("@isreview", SqlDbType.TinyInt, 1, isReview)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}updateorderisreview", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public DataTable GetUserOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime, int orderState)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@uid", SqlDbType.Int, 4, uid), 
                                    GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize), 
                                    GenerateInParam("@pagenumber", SqlDbType.Int, 4, pageNumber), 
                                    GenerateInParam("@startaddtime", SqlDbType.VarChar, 30, startAddTime.Length > 0? TypeHelper.StringToDateTime(startAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@endaddtime", SqlDbType.VarChar, 30, endAddTime.Length > 0? TypeHelper.StringToDateTime(endAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@orderstate", SqlDbType.TinyInt, 1, orderState)
                                   };
            return RDBSHelper.ExecuteDataset(CommandType.StoredProcedure,
                                             string.Format("{0}getuserorderlist", RDBSHelper.RDBSTablePre),
                                             parms).Tables[0];
        }

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public int GetUserOrderCount(int uid, string startAddTime, string endAddTime, int orderState)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@uid", SqlDbType.Int, 4, uid), 
                                    GenerateInParam("@startaddtime", SqlDbType.VarChar, 30, startAddTime.Length > 0? TypeHelper.StringToDateTime(startAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@endaddtime", SqlDbType.VarChar, 30, endAddTime.Length > 0? TypeHelper.StringToDateTime(endAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@orderstate", SqlDbType.TinyInt, 1, orderState)
                                   };
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.StoredProcedure,
                                                                   string.Format("{0}getuserordercount", RDBSHelper.RDBSTablePre),
                                                                   parms));
        }

        /// <summary>
        /// 获得用户未评价订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <returns></returns>
        public DataTable GetUserUnReviewOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@uid", SqlDbType.Int, 4, uid), 
                                    GenerateInParam("@pagesize", SqlDbType.Int, 4, pageSize), 
                                    GenerateInParam("@pagenumber", SqlDbType.Int, 4, pageNumber), 
                                    GenerateInParam("@startaddtime", SqlDbType.VarChar, 30, startAddTime.Length > 0? TypeHelper.StringToDateTime(startAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@endaddtime", SqlDbType.VarChar, 30, endAddTime.Length > 0? TypeHelper.StringToDateTime(endAddTime).ToString("yyyy-MM-dd HH:mm:ss") : "")
                                   };
            return RDBSHelper.ExecuteDataset(CommandType.StoredProcedure,
                                             string.Format("{0}getuserunrevieworderlist", RDBSHelper.RDBSTablePre),
                                             parms).Tables[0];
        }

        /// <summary>
        /// 获得用户未评价订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <returns></returns>
        public int GetUserUnReviewOrderCount(int uid, string startAddTime, string endAddTime)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@uid", SqlDbType.Int, 4, uid), 
                                    GenerateInParam("@startaddtime", SqlDbType.VarChar, 30, startAddTime.Length > 0? TypeHelper.StringToDateTime(startAddTime).ToString("yyyy-MM-dd HH:mm:ss") : ""),
                                    GenerateInParam("@endaddtime", SqlDbType.VarChar, 30, endAddTime.Length > 0? TypeHelper.StringToDateTime(endAddTime).ToString("yyyy-MM-dd HH:mm:ss") : "")
                                   };
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.StoredProcedure,
                                                                   string.Format("{0}getuserunreviewordercount", RDBSHelper.RDBSTablePre),
                                                                   parms));
        }

        /// <summary>
        /// 获得销售商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderState">订单状态</param>
        /// <returns></returns>
        public DataTable GetSaleProductList(int pageSize, int pageNumber, int storeId, string startTime, string endTime, int orderState)
        {
            string condition = GetSaleProductListCondition(storeId, startTime, endTime, orderState);
            string commandText;
            if (pageNumber == 1)
            {
                commandText = string.Format("SELECT TOP {2} [temp2].[psn],[temp2].[name],[temp2].[realcount],[temp2].[shopprice],[temp1].[osn],[temp1].[addtime] FROM (SELECT [oid],[osn],[addtime] FROM [{0}orders] {1}) AS [temp1] LEFT JOIN [{0}orderproducts] AS [temp2] ON [temp1].[oid]=[temp2].[oid] ORDER BY [recordid] DESC",
                                             RDBSHelper.RDBSTablePre,
                                             condition,
                                             pageSize);
            }
            else
            {
                commandText = string.Format("SELECT [psn],[name],[realcount],[shopprice],[osn],[addtime] FROM (SELECT TOP {1} ROW_NUMBER() OVER (ORDER BY [recordid] DESC) AS [rowid],[temp2].[psn],[temp2].[name],[temp2].[realcount],[temp2].[shopprice],[temp1].[osn],[temp1].[addtime] FROM (SELECT [oid],[osn],[addtime] FROM [{0}orders] {3}) AS [temp1] LEFT JOIN [{0}orderproducts] AS [temp2] ON [temp1].[oid]=[temp2].[oid]) AS [temp] WHERE [temp].[rowid] BETWEEN {2} AND {1}",
                                             RDBSHelper.RDBSTablePre,
                                             pageNumber * pageSize,
                                             (pageNumber - 1) * pageSize + 1,
                                             condition);
            }

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }

        /// <summary>
        /// 获得销售商品数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderState">订单状态</param>
        /// <returns></returns>
        public int GetSaleProductCount(int storeId, string startTime, string endTime, int orderState)
        {
            string condition = GetSaleProductListCondition(storeId, startTime, endTime, orderState);
            string commandText = string.Format("SELECT COUNT([temp2].[recordid]) FROM (SELECT [oid],[osn],[addtime] FROM [{0}orders] {1}) AS [temp1] LEFT JOIN [{0}orderproducts] AS [temp2] ON [temp1].[oid]=[temp2].[oid]",
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 获得销售商品列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderState">订单状态</param>
        /// <returns></returns>
        private string GetSaleProductListCondition(int storeId, string startTime, string endTime, int orderState)
        {
            StringBuilder condition = new StringBuilder();

            if (orderState > 0)
                condition.AppendFormat(" AND [orderstate]={0} ", orderState);
            if (storeId > 0)
                condition.AppendFormat(" AND [storeid]={0} ", storeId);
            if (!string.IsNullOrEmpty(startTime))
                condition.AppendFormat(" AND [addtime]>='{0}' ", TypeHelper.StringToDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"));
            if (!string.IsNullOrEmpty(endTime))
                condition.AppendFormat(" AND [addtime]<='{0}' ", TypeHelper.StringToDateTime(endTime).ToString("yyyy-MM-dd HH:mm:ss"));

            return condition.Length > 0 ? condition.Remove(0, 4).Insert(0, " WHERE").ToString() : "";
        }

        /// <summary>
        /// 获得订单统计
        /// </summary>
        /// <param name="statType">统计类型(0代表订单数，1代表订单合计)</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public DataTable GetOrderStat(int statType, int storeId, string startTime, string endTime)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid]={0} ", storeId);
            if (!string.IsNullOrEmpty(startTime))
                condition.AppendFormat(" AND [addtime]>='{0}' ", TypeHelper.StringToDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"));
            if (!string.IsNullOrEmpty(endTime))
                condition.AppendFormat(" AND [addtime]<'{0}' ", TypeHelper.StringToDateTime(endTime).ToString("yyyy-MM-dd HH:mm:ss"));

            if (condition.Length > 0)
                condition = condition.Remove(0, 4).Insert(0, " WHERE");

            string commandText = "";
            if (statType == 0)
            {
                commandText = string.Format("SELECT COUNT([oid]) AS [value],[orderstate] FROM (SELECT [oid],[orderstate] FROM [{0}orders] {1}) AS [temp] GROUP BY [temp].[orderstate]",
                                             RDBSHelper.RDBSTablePre,
                                             condition.ToString());
            }
            else
            {
                commandText = string.Format("SELECT SUM([orderamount]) AS [value],[orderstate] FROM (SELECT [oid],[orderstate],[orderamount] FROM [{0}orders] {1}) AS [temp] GROUP BY [temp].[orderstate]",
                                             RDBSHelper.RDBSTablePre,
                                             condition.ToString());
            }
            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }

        #endregion

        #region 订单处理

        /// <summary>
        /// 创建订单处理
        /// </summary>
        /// <param name="orderActionInfo">订单处理信息</param>
        public void CreateOrderAction(OrderActionInfo orderActionInfo)
        {
            DbParameter[] parms = {
	                                    GenerateInParam("@oid", SqlDbType.Int,4,orderActionInfo.Oid),
	                                    GenerateInParam("@uid", SqlDbType.Int,4 ,orderActionInfo.Uid),
	                                    GenerateInParam("@realname", SqlDbType.NVarChar,10,orderActionInfo.RealName),
	                                    GenerateInParam("@actiontype", SqlDbType.TinyInt,1 ,orderActionInfo.ActionType),
                                        GenerateInParam("@actiontime", SqlDbType.DateTime, 8,orderActionInfo.ActionTime),
                                        GenerateInParam("@actiondes", SqlDbType.NVarChar, 250,orderActionInfo.ActionDes)
                                    };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}createorderaction", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 获得订单处理列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public IDataReader GetOrderActionList(int oid)
        {
            DbParameter[] parms = {
	                                    GenerateInParam("@oid", SqlDbType.Int,4,oid)
                                    };
            return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
                                            string.Format("{0}getorderactionlist", RDBSHelper.RDBSTablePre),
                                            parms);
        }

        /// <summary>
        /// 获得订单id列表
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderActionType">订单操作类型</param>
        /// <returns></returns>
        public DataTable GetOrderIdList(DateTime startTime, DateTime endTime, int orderActionType)
        {
            DbParameter[] parms = {
                                    GenerateInParam("@starttime", SqlDbType.DateTime, 8, startTime),
                                    GenerateInParam("@endtime", SqlDbType.DateTime, 8, endTime),
                                    GenerateInParam("@orderactiontype", SqlDbType.TinyInt, 1, orderActionType)
                                   };
            string commandText = string.Format("SELECT [oid] FROM [{0}orderactions] WHERE [actiontype]=@orderactiontype AND [actiontime]>=@starttime AND [actiontime]<@endtime",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }

        #endregion

        #region 订单退款

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="orderRefundInfo">订单退款信息</param>
        public void ApplyRefund(OrderRefundInfo orderRefundInfo)
        {
            DbParameter[] parms = {
	                                GenerateInParam("@storeid", SqlDbType.Int, 4, orderRefundInfo.StoreId),
	                                GenerateInParam("@storename", SqlDbType.NVarChar,60,orderRefundInfo.StoreName),
	                                GenerateInParam("@oid", SqlDbType.Int, 4, orderRefundInfo.Oid),
	                                GenerateInParam("@osn", SqlDbType.VarChar,30,orderRefundInfo.OSN),
	                                GenerateInParam("@uid", SqlDbType.Int,4 ,orderRefundInfo.Uid),
	                                GenerateInParam("@state", SqlDbType.TinyInt,1 ,orderRefundInfo.State),
	                                GenerateInParam("@applytime", SqlDbType.DateTime,8,orderRefundInfo.ApplyTime),
	                                GenerateInParam("@paymoney", SqlDbType.Decimal,8,orderRefundInfo.PayMoney),
	                                GenerateInParam("@refundmoney", SqlDbType.Decimal,8,orderRefundInfo.RefundMoney),
                                    GenerateInParam("@refundsn", SqlDbType.VarChar,30 ,orderRefundInfo.RefundSN),
                                    GenerateInParam("@refundsystemname", SqlDbType.VarChar,20 ,orderRefundInfo.RefundSystemName),
                                    GenerateInParam("@refundfriendname", SqlDbType.NVarChar,30 ,orderRefundInfo.RefundFriendName),
                                    GenerateInParam("@refundtime", SqlDbType.DateTime,8 ,orderRefundInfo.RefundTime),
                                    GenerateInParam("@paysn", SqlDbType.VarChar,30 ,orderRefundInfo.PaySN),
                                    GenerateInParam("@paysystemname", SqlDbType.VarChar,20 ,orderRefundInfo.PaySystemName),
                                    GenerateInParam("@payfriendname", SqlDbType.NVarChar,30 ,orderRefundInfo.PayFriendName)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}applyrefund", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        public void RefundOrder(int refundId, string refundSN, string refundSystemName, string refundFriendName, DateTime refundTime)
        {
            DbParameter[] parms = {
	                                GenerateInParam("@refundid", SqlDbType.Int, 4, refundId),
                                    GenerateInParam("@refundsn", SqlDbType.VarChar,30 ,refundSN),
                                    GenerateInParam("@refundsystemname", SqlDbType.VarChar,20 ,refundSystemName),
                                    GenerateInParam("@refundfriendname", SqlDbType.NVarChar,30 ,refundFriendName),
                                    GenerateInParam("@refundtime", SqlDbType.DateTime,8 ,refundTime)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                       string.Format("{0}refundorder", RDBSHelper.RDBSTablePre),
                                       parms);
        }

        /// <summary>
        /// 获得订单退款列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public IDataReader GetOrderRefundList(int pageSize, int pageNumber, string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} {2} FROM [{1}orderrefunds] ORDER BY [refundid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                RDBSFields.ORDER_REFUNDS);

                else
                    commandText = string.Format("SELECT TOP {0} {3} FROM [{1}orderrefunds] WHERE {2} ORDER BY [refundid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition,
                                                RDBSFields.ORDER_REFUNDS);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} {3} FROM [{1}orderrefunds] WHERE [refundid] < (SELECT MIN([refundid]) FROM (SELECT TOP {2} [refundid] FROM [{1}orderrefunds] ORDER BY [refundid] DESC) AS [temp]) ORDER BY [refundid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                RDBSFields.ORDER_REFUNDS);
                else
                    commandText = string.Format("SELECT TOP {0} {4} FROM [{1}orderrefunds] WHERE [refundid] < (SELECT MIN([refundid]) FROM (SELECT TOP {2} [refundid] FROM [{1}orderrefunds] WHERE {3} ORDER BY [refundid] DESC) AS [temp]) AND {3} ORDER BY [refundid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                condition,
                                                RDBSFields.ORDER_REFUNDS);
            }

            return RDBSHelper.ExecuteReader(CommandType.Text, commandText);
        }

        /// <summary>
        /// 获得订单退款列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <returns></returns>
        public string GetOrderRefundListCondition(int storeId, string osn)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn) && SecureHelper.IsSafeSqlString(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }

        /// <summary>
        /// 获得订单退款数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int GetOrderRefundCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(refundid) FROM [{0}orderrefunds]", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(refundid) FROM [{0}orderrefunds] WHERE {1}", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        #endregion
    }
}
