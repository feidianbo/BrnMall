﻿using System;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Web.Framework
{
    /// <summary>
    /// PC前台工作上下文类
    /// </summary>
    public class WebWorkContext
    {
        public MallConfigInfo MallConfig = BMAConfig.MallConfig;//商城配置信息

        public bool IsHttpAjax;//当前请求是否为ajax请求

        public string IP;//用户ip

        public RegionInfo RegionInfo;//区域信息

        public int RegionId;//区域id

        public string Url;//当前url

        public string UrlReferrer;//上一次访问的url

        public string Sid;//用户sid

        public int Uid = -1;//用户id

        public string UserName;//用户名

        public string UserEmail;//用户邮箱

        public string UserMobile;//用户手机号

        public string NickName;//用户昵称

        public string Avatar;//用户头像

        public string Password;//用户密码

        public string EncryptPwd;//加密密码

        public string PayCreditName;//支付积分名称

        public int PayCreditCount = 0;//支付积分数量

        public string RankCreditName;//等级积分名称

        public int RankCreditCount = 0;//等级积分数量

        public PartUserInfo PartUserInfo;//用户信息

        public int UserRid = -1;//用户等级id

        public UserRankInfo UserRankInfo;//用户等级信息

        public string UserRTitle;//用户等级标题

        public int MallAGid = -1;//用户商城管理员组id

        public MallAdminGroupInfo MallAdminGroupInfo;//用户商城管理员组信息

        public string MallAGTitle;//商城管理员组标题

        public int StoreId = 0;//店铺id

        public StoreInfo StoreInfo = null;//店铺信息

        public string Controller;//控制器

        public string Action;//动作方法

        public string PageKey;//页面标示符

        public string ImageCDN;//图片cdn

        public string CSSCDN;//csscdn

        public string ScriptCDN;//脚本cdn

        public int OnlineUserCount = 0;//在线总人数

        public int OnlineMemberCount = 0;//在线会员数

        public int OnlineGuestCount = 0;//在线游客数

        public string SearchWord;//搜索词

        public int CartProductCount = 0;//购物车中商品数量

        public List<NavInfo> NavList;//导航列表

        public FriendLinkInfo[] FriendLinkList;//友情链接列表

        public List<HelpInfo> HelpList;//帮助列表

        public DateTime StartExecuteTime;//页面开始执行时间

        public double ExecuteTime;//页面执行时间

        public int ExecuteCount = 0;//执行的sql语句数目

        public string ExecuteDetail;//执行的sql语句细节

        public string MallVersion = BMAVersion.MALL_VERSION;//商城版本

        public string MallCopyright = BMAVersion.MALL_COPYRIGHT;//商城版权

    }
}