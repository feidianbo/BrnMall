﻿using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 后台插件操作管理类
    /// </summary>
    public partial class AdminPlugins : Plugins
    {
        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="systemName">插件系统名称</param>
        public static void Install(string systemName)
        {
            BMAPlugin.Install(systemName);
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="systemName">插件系统名称</param>
        public static void Uninstall(string systemName)
        {
            BMAPlugin.Uninstall(systemName);
        }

        /// <summary>
        /// 编辑插件信息
        /// </summary>
        /// <param name="systemName">插件系统名称</param>
        /// <param name="friendlyName">插件友好名称</param>
        /// <param name="description">插件描述</param>
        /// <param name="displayOrder">插件排序</param>
        public static void Edit(string systemName, string friendlyName, string description, int displayOrder)
        {
            BMAPlugin.Edit(systemName, friendlyName, description, displayOrder);
        }

        /// <summary>
        /// 设置默认插件
        /// </summary>
        /// <param name="systemName">插件系统名称</param>
        public static void Default(string systemName)
        {
            BMAPlugin.Default(systemName);
        }

        /// <summary>
        /// 获得未安装的插件列表
        /// </summary>
        /// <returns></returns>
        public static List<PluginInfo> GetUnInstalledPluginList()
        {
            return BMAPlugin.UnInstalledPluginList;
        }

        /// <summary>
        /// 根据插件类型获得安装的插件列表
        /// </summary>
        /// <param name="pluginType">插件类型</param>
        /// <returns></returns>
        public static List<PluginInfo> GetInstalledPluginList(PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.OAuthPlugin:
                    return BMAPlugin.OAuthPluginList;
                case PluginType.PayPlugin:
                    return BMAPlugin.PayPluginList;
                default:
                    return new List<PluginInfo>();
            }
        }

        /// <summary>
        /// 根据插件类型获得未安装的插件列表
        /// </summary>
        /// <param name="pluginType">插件类型</param>
        /// <returns></returns>
        public static List<PluginInfo> GetUnInstalledPluginList(PluginType pluginType)
        {
            int type = (int)pluginType;
            List<PluginInfo> unInstalledPluginList = new List<PluginInfo>();
            foreach (PluginInfo pluginInfo in BMAPlugin.UnInstalledPluginList)
            {
                if (pluginInfo.Type == type)
                    unInstalledPluginList.Add(pluginInfo);
            }
            return unInstalledPluginList;
        }

        /// <summary>
        /// 获得插件
        /// </summary>
        /// <param name="systemName">插件系统名称</param>
        /// <returns></returns>
        public static PluginInfo GetPluginBySystemName(string systemName)
        {
            PluginInfo pluginInfo = null;
            Predicate<PluginInfo> condition = x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase);
            pluginInfo = GetOAuthPluginList().Find(condition);
            if (pluginInfo == null)
                pluginInfo = GetPayPluginList().Find(condition);
            if (pluginInfo == null)
                pluginInfo = GetUnInstalledPluginList().Find(condition);

            return pluginInfo;
        }
    }
}
