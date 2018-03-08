﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.MallAdmin.Models;

namespace BrnMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台用户等级控制器类
    /// </summary>
    public partial class UserRankController : BaseMallAdminController
    {
        /// <summary>
        /// 会员等级列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            UserRankListModel model = new UserRankListModel()
            {
                UserRankList = AdminUserRanks.GetCustomerUserRankList()
            };
            MallUtils.SetAdminRefererCookie(Url.Action("list"));
            return View(model);
        }

        /// <summary>
        /// 添加会员等级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Add()
        {
            UserRankModel model = new UserRankModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加会员等级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(UserRankModel model)
        {
            if (AdminUserRanks.GetUserRidByTitle(model.UserRankTitle) > 0)
                ModelState.AddModelError("UserRankTitle", "名称已经存在");

            if (ModelState.IsValid)
            {
                UserRankInfo userRankInfo = new UserRankInfo()
                {
                    System = 0,
                    Title = model.UserRankTitle,
                    Avatar = model.Avatar ?? "",
                    CreditsLower = model.CreditsLower,
                    CreditsUpper = model.CreditsUpper,
                    LimitDays = 0
                };

                AdminUserRanks.CreateUserRank(userRankInfo);
                AddMallAdminLog("添加会员等级", "添加会员等级,会员等级为:" + model.UserRankTitle);
                return PromptView("会员等级添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑会员等级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int userRid = -1)
        {
            UserRankInfo userRankInfo = AdminUserRanks.GetUserRankById(userRid);
            if (userRankInfo == null)
                return PromptView("会员等级不存在");

            if (userRankInfo.System == 1)
                return PromptView("系统等级不能编辑");

            UserRankModel model = new UserRankModel();
            model.UserRankTitle = userRankInfo.Title;
            model.Avatar = userRankInfo.Avatar;
            model.CreditsLower = userRankInfo.CreditsLower;
            model.CreditsUpper = userRankInfo.CreditsUpper;

            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑会员等级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserRankModel model, int userRid = -1)
        {
            UserRankInfo userRankInfo = AdminUserRanks.GetUserRankById(userRid);
            if (userRankInfo == null)
                return PromptView("会员等级不存在");

            if (userRankInfo.System == 1)
                return PromptView("系统等级不能编辑");

            int userRid2 = AdminUserRanks.GetUserRidByTitle(model.UserRankTitle);
            if (userRid2 > 0 && userRid2 != userRid)
                ModelState.AddModelError("UserRankTitle", "名称已经存在");

            if (ModelState.IsValid)
            {
                userRankInfo.Title = model.UserRankTitle;
                userRankInfo.Avatar = model.Avatar ?? "";
                userRankInfo.CreditsLower = model.CreditsLower;
                userRankInfo.CreditsUpper = model.CreditsUpper;

                AdminUserRanks.UpdateUserRank(userRankInfo);
                AddMallAdminLog("修改会员等级", "修改会员等级,会员等级ID为:" + userRid);
                return PromptView("会员等级修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除会员等级
        /// </summary>
        /// <returns></returns>
        public ActionResult Del(int userRid = -1)
        {
            int result = AdminUserRanks.DeleteUserRankById(userRid);
            if (result == -1)
                return PromptView("删除失败请先转移或删除此会员等级下的用户");
            else if (result == -2)
                return PromptView("系统等级不能删除");

            AddMallAdminLog("删除会员等级", "删除会员等级,会员等级ID为:" + userRid);
            return PromptView("会员等级删除成功");
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("{0},", imgType.ToLower());
            allowImgType = allowImgType.Replace(".", "");
            allowImgType = allowImgType.TrimEnd(',');

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.UserRankAvatarThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["allowImgType"] = allowImgType;
            ViewData["maxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
    }
}
