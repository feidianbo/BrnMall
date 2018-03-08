using System;
using System.Web;
using System.Web.Mvc;

using BrnMall.Core;
using BrnMall.Web.Framework;
using BrnMall.PayPlugin.ChinaBank;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台网银在线控制器类
    /// </summary>
    public class AdminChinaBankController : BaseMallAdminController
    {
        /// <summary>
        /// 配置
        /// </summary>
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Config()
        {
            ConfigModel model = new ConfigModel();

            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            model.Mid = pluginSetInfo.Mid;
            model.Key = pluginSetInfo.Key;
            model.PayFee = pluginSetInfo.PayFee;
            model.FreeMoney = pluginSetInfo.FreeMoney;

            return View("~/plugins/BrnMall.PayPlugin.ChinaBank/views/adminchinabank/config.cshtml", model);
        }

        /// <summary>
        /// 配置
        /// </summary>
        [HttpPost]
        public ActionResult Config(ConfigModel model)
        {
            if (ModelState.IsValid)
            {
                PluginSetInfo pluginSetInfo = new PluginSetInfo();
                pluginSetInfo.Mid = model.Mid.Trim();
                pluginSetInfo.Key = model.Key.Trim();
                pluginSetInfo.PayFee = model.PayFee;
                pluginSetInfo.FreeMoney = model.FreeMoney;
                PluginUtils.SavePluginSet(pluginSetInfo);

                AddMallAdminLog("修改网银在线插件配置信息");
                return PromptView(Url.Action("config", "plugin", new { configController = "AdminChinaBank", configAction = "Config" }), "插件配置修改成功");
            }
            return PromptView(Url.Action("config", "plugin", new { configController = "AdminChinaBank", configAction = "Config" }), "信息有误，请重新填写");
        }
    }
}
