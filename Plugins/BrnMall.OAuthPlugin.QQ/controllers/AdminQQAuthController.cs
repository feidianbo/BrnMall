﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.OAuthPlugin.QQ;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台QQ开放授权控制器类
    /// </summary>
    public class AdminQQOAuthController : BaseMallAdminController
    {
        /// <summary>
        /// 配置
        /// </summary>
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Config()
        {
            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();

            ConfigModel model = new ConfigModel();
            model.AuthUrl = pluginSetInfo.AuthUrl;
            model.AppKey = pluginSetInfo.AppKey;
            model.AppSecret = pluginSetInfo.AppSecret;
            model.Server = pluginSetInfo.Server;
            model.UNamePrefix = pluginSetInfo.UNamePrefix;

            return View("~/plugins/BrnMall.OAuthPlugin.QQ/views/adminqqoauth/config.cshtml", model);
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

                pluginSetInfo.AuthUrl = model.AuthUrl.Trim();
                pluginSetInfo.AppKey = model.AppKey.Trim();
                pluginSetInfo.AppSecret = model.AppSecret.Trim();
                pluginSetInfo.Server = model.Server.Trim();
                pluginSetInfo.UNamePrefix = model.UNamePrefix.Trim();

                PluginUtils.SavePluginSet(pluginSetInfo);
                AddMallAdminLog("修改QQ开放授权插件配置信息");
                return PromptView(Url.Action("config", "plugin", new { configController = "AdminQQOAuth", configAction = "Config" }), "插件配置修改成功");
            }
            return PromptView(Url.Action("config", "plugin", new { configController = "AdminQQOAuth", configAction = "Config" }), "信息有误，请重新填写");
        }
    }
}
