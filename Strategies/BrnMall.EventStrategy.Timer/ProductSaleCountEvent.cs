﻿using System;
using System.Data;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;

namespace BrnMall.EventStrategy.Timer
{
    /// <summary>
    /// 商品销量事件
    /// </summary>
    public class ProductSaleCountEvent : IEvent
    {
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventInfo">事件信息</param>
        public void Execute(object eventInfo)
        {
            EventInfo e = (EventInfo)eventInfo;

            //同步商品销量
            DateTime lastExecuteTime = EventLogs.GetEventLastExecuteTimeByKey(e.Key);
            DataTable dt = OrderActions.GetOrderIdList(lastExecuteTime, DateTime.Now, (int)OrderActionType.Complete);
            foreach (DataRow row in dt.Rows)
            {
                int oid = TypeHelper.ObjectToInt(row["oid"]);
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(oid);
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    Products.AddProductShadowSaleCount(orderProductInfo.Pid, orderProductInfo.RealCount);
                }
            }

            EventLogs.CreateEventLog(e.Key, e.Title, Environment.MachineName, DateTime.Now);
        }
    }
}
