using System;
using System.Web;
using System.Web.Mvc;

using BrnMall.Core;
using BrnMall.Web.Framework;
using BrnMall.PayPlugin.COD;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 后台货到付款控制器类
    /// </summary>
    public class AdminCODController : BaseMallAdminController
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
            model.PayFee = pluginSetInfo.PayFee;
            model.FreeMoney = pluginSetInfo.FreeMoney;

            return View("~/plugins/BrnMall.PayPlugin.COD/views/admincod/config.cshtml", model);
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
                pluginSetInfo.PayFee = model.PayFee;
                pluginSetInfo.FreeMoney = model.FreeMoney;
                PluginUtils.SavePluginSet(pluginSetInfo);

                AddMallAdminLog("修改货到付款插件配置信息");
                return PromptView(Url.Action("config", "plugin", new { configController = "AdminCOD", configAction = "Config" }), "插件配置修改成功");
            }
            return PromptView(Url.Action("config", "plugin", new { configController = "AdminCOD", configAction = "Config" }), "信息有误，请重新填写");
        }
    }
}
