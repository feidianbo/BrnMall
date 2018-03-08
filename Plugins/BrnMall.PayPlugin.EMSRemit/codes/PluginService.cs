using System;

using BrnMall.Core;

namespace BrnMall.PayPlugin.EMSRemit
{
    /// <summary>
    /// 插件服务类
    /// </summary>
    public class PluginService : IPayPlugin
    {
        /// <summary>
        /// 插件配置控制器
        /// </summary>
        public string ConfigController
        {
            get { return "adminemsremit"; }
        }

        /// <summary>
        /// 插件配置动作方法
        /// </summary>
        public string ConfigAction
        {
            get { return "config"; }
        }

        /// <summary>
        /// 付款方式(0代表货到付款，1代表在线付款，2代表线下付款)
        /// </summary>
        public int PayMode
        {
            get { return 2; }
        }

        /// <summary>
        /// 获得支付手续费
        /// </summary>
        /// <param name="productAmount">商品合计</param>
        /// <param name="buyTime">购买时间</param>
        /// <param name="partUserInfo">购买用户</param>
        /// <returns></returns>
        public decimal GetPayFee(decimal productAmount, DateTime buyTime, PartUserInfo partUserInfo)
        {
            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            if (productAmount >= pluginSetInfo.FreeMoney)
                return 0M;
            else
                return pluginSetInfo.PayFee;
        }

        /// <summary>
        /// 支付控制器
        /// </summary>
        public string PayController
        {
            get { return ""; }
        }

        /// <summary>
        /// 支付动作方法
        /// </summary>
        public string PayAction
        {
            get { return ""; }
        }

        /// <summary>
        /// App支付控制器
        /// </summary>
        public string AppPayController
        {
            get { return ""; }
        }

        /// <summary>
        /// App支付动作方法
        /// </summary>
        public string AppPayAction
        {
            get { return ""; }
        }

        /// <summary>
        /// App返回控制器
        /// </summary>
        public string AppReturnController
        {
            get { return ""; }
        }

        /// <summary>
        /// App返回动作方法
        /// </summary>
        public string AppReturnAction
        {
            get { return ""; }
        }
    }
}
