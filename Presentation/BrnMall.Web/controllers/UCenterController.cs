﻿using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;
using BrnMall.Web.Models;

namespace BrnMall.Web.Controllers
{
    /// <summary>
    /// 用户中心控制器类
    /// </summary>
    public partial class UCenterController : BaseWebController
    {
        #region 用户信息

        /// <summary>
        /// 用户信息
        /// </summary>
        public ActionResult UserInfo()
        {
            UserInfoModel model = new UserInfoModel();

            model.UserInfo = Users.GetUserById(WorkContext.Uid);
            model.UserRankInfo = WorkContext.UserRankInfo;

            RegionInfo regionInfo = Regions.GetRegionById(model.UserInfo.RegionId);
            if (regionInfo != null)
            {
                ViewData["provinceId"] = regionInfo.ProvinceId;
                ViewData["cityId"] = regionInfo.CityId;
                ViewData["countyId"] = regionInfo.RegionId;
            }
            else
            {
                ViewData["provinceId"] = -1;
                ViewData["cityId"] = -1;
                ViewData["countyId"] = -1;
            }

            return View(model);
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        public ActionResult EditUser()
        {
            string userName = WebHelper.GetFormString("userName");
            string nickName = WebHelper.GetFormString("nickName");
            string avatar = WebHelper.GetFormString("avatar");
            string realName = WebHelper.GetFormString("realName");
            int gender = WebHelper.GetFormInt("gender");
            string idCard = WebHelper.GetFormString("idCard");
            string bday = WebHelper.GetFormString("bday");
            int regionId = WebHelper.GetFormInt("regionId");
            string address = WebHelper.GetFormString("address");
            string bio = WebHelper.GetFormString("bio");

            StringBuilder errorList = new StringBuilder("[");
            //验证用户名
            if (WorkContext.UserName.Length == 0 && userName.Length > 0)
            {
                if (userName.Length < 4 || userName.Length > 10)
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名必须大于3且不大于10个字符", "}");
                }
                else if (userName.Contains(" "))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含空格", "}");
                }
                else if (userName.Contains(":"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含冒号", "}");
                }
                else if (userName.Contains("<"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含'<'符号", "}");
                }
                else if (userName.Contains(">"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含'>'符号", "}");
                }
                else if ((!SecureHelper.IsSafeSqlString(userName)))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "账户名不符合系统要求", "}");
                }
                else if (CommonHelper.IsInArray(userName, WorkContext.MallConfig.ReservedName, "\n"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名已经存在", "}");
                }
                else if (FilterWords.IsContainWords(userName))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名包含禁止单词", "}");
                }
                else if (Users.IsExistUserName(userName))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名已经存在", "}");
                }
            }
            else
            {
                userName = WorkContext.UserName;
            }

            //验证昵称
            if (nickName.Length > 10)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称的长度不能大于10", "}");
            }
            else if (FilterWords.IsContainWords(nickName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称中包含禁止单词", "}");
            }

            //验证真实姓名
            if (realName.Length > 5)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "realName", "真实姓名的长度不能大于5", "}");
            }

            //验证性别
            if (gender < 0 || gender > 2)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "gender", "请选择正确的性别", "}");

            //验证身份证号
            if (idCard.Length > 0 && !ValidateHelper.IsIdCard(idCard))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "idCard", "请输入正确的身份证号", "}");
            }

            //验证出生日期
            if (bday.Length == 0)
            {
                string bdayY = WebHelper.GetFormString("bdayY");
                string bdayM = WebHelper.GetFormString("bdayM");
                string bdayD = WebHelper.GetFormString("bdayD");
                bday = string.Format("{0}-{1}-{2}", bdayY, bdayM, bdayD);
            }
            if (bday.Length > 0 && bday != "--" && !ValidateHelper.IsDate(bday))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bday", "请选择正确的日期", "}");

            //验证区域
            if (regionId > 0)
            {
                RegionInfo regionInfo = Regions.GetRegionById(regionId);
                if (regionInfo == null || regionInfo.Layer != 3)
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "regionId", "请选择正确的地址", "}");
            }

            //验证详细地址
            if (address.Length > 75)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "详细地址的长度不能大于75", "}");
            }

            //验证简介
            if (bio.Length > 150)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bio", "简介的长度不能大于150", "}");
            }

            if (errorList.Length == 1)
            {
                if (bday.Length == 0 || bday == "--")
                    bday = "1900-1-1";

                if (regionId < 1)
                    regionId = 0;

                Users.UpdateUser(WorkContext.Uid, userName, WebHelper.HtmlEncode(nickName), WebHelper.HtmlEncode(avatar), gender, WebHelper.HtmlEncode(realName), TypeHelper.StringToDateTime(bday), idCard, regionId, WebHelper.HtmlEncode(address), WebHelper.HtmlEncode(bio));
                if (userName.Length > 0 && nickName.Length > 0 && avatar.Length > 0 && realName.Length > 0 && bday != "1900-1-1" && idCard.Length > 0 && regionId > 0 && address.Length > 0)
                {
                    Credits.SendCompleteUserInfoCredits(ref WorkContext.PartUserInfo, DateTime.Now);
                }
                return AjaxResult("success", "信息更新成功");
            }
            else
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
        }

        #endregion

        #region 安全中心

        /// <summary>
        /// 账户安全信息
        /// </summary>
        public ActionResult SafeInfo()
        {
            return View(WorkContext.PartUserInfo);
        }

        /// <summary>
        /// 安全验证
        /// </summary>
        public ActionResult SafeVerify()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string mode = WebHelper.GetQueryString("mode").ToLower();

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }) || (mode.Length > 0 && !CommonHelper.IsInArray(mode, new string[3] { "password", "mobile", "email" })))
                return HttpNotFound();

            SafeVerifyModel model = new SafeVerifyModel();
            model.Action = action;

            if (mode.Length == 0)
            {
                if (WorkContext.PartUserInfo.VerifyMobile == 1)//通过手机验证
                    model.Mode = "mobile";
                else if (WorkContext.PartUserInfo.VerifyEmail == 1)//通过邮箱验证
                    model.Mode = "email";
                else//通过密码验证
                    model.Mode = "password";
            }
            else
            {
                if (mode == "mobile" && WorkContext.PartUserInfo.VerifyMobile == 1)
                    model.Mode = "mobile";
                else if (mode == "email" && WorkContext.PartUserInfo.VerifyEmail == 1)
                    model.Mode = "email";
                else
                    model.Mode = "password";
            }

            return View(model);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        public ActionResult VerifyPassword()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string password = WebHelper.GetFormString("password");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
                return AjaxResult("noaction", "动作不存在");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("password", "密码不能为空");
            }
            if (Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt) != WorkContext.Password)
            {
                return AjaxResult("password", "密码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = Url.Action("safeupdate", new RouteValueDictionary { { "v", v } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送验证手机短信
        /// </summary>
        public ActionResult SendVerifyMobile()
        {
            if (WorkContext.PartUserInfo.VerifyMobile == 0)
                return AjaxResult("unverifymobile", "手机号没有通过验证,所以不能发送验证短信");

            string moibleCode = Randoms.CreateRandomValue(6);
            //发送验证手机短信
            SMSes.SendSCVerifySMS(WorkContext.UserMobile, moibleCode);
            //将验证值保存在session中
            Sessions.SetItem(WorkContext.Sid, "ucsvMoibleCode", moibleCode);

            return AjaxResult("success", "短信已经发送,请查收");
        }

        /// <summary>
        /// 验证手机
        /// </summary>
        public ActionResult VerifyMobile()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string moibleCode = WebHelper.GetFormString("moibleCode");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
                return AjaxResult("noaction", "动作不存在");
            if (WorkContext.PartUserInfo.VerifyMobile == 0)
                return AjaxResult("unverifymobile", "手机号没有通过验证,所以不能进行验证");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查手机码
            if (string.IsNullOrWhiteSpace(moibleCode))
            {
                return AjaxResult("moiblecode", "手机码不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsvMoibleCode") != moibleCode)
            {
                return AjaxResult("moiblecode", "手机码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = Url.Action("safeupdate", new RouteValueDictionary { { "v", v } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送验证邮箱邮件
        /// </summary>
        public ActionResult SendVerifyEmail()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
                return AjaxResult("noaction", "动作不存在");
            if (WorkContext.PartUserInfo.VerifyEmail == 0)
                return AjaxResult("unverifyemail", "邮箱没有通过验证,所以不能发送验证邮件");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("safeupdate", new RouteValueDictionary { { "v", v } }));
            //发送验证邮件
            Emails.SendSCVerifyEmail(WorkContext.UserEmail, WorkContext.UserName, url);
            return AjaxResult("success", "邮件已经发送,请前往你的邮箱进行验证");
        }

        /// <summary>
        /// 安全更新
        /// </summary>
        public ActionResult SafeUpdate()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV;
            try
            {
                realV = MallUtils.AESDecrypt(v);
            }
            catch (Exception ex)
            {
                //如果v来自邮件，那么需要url解码
                realV = MallUtils.AESDecrypt(WebHelper.UrlDecode(v));
            }

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return HttpNotFound();

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return HttpNotFound();
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return PromptView("此链接已经失效，请重新验证");

            SafeUpdateModel model = new SafeUpdateModel();
            model.Action = action;
            model.V = WebHelper.UrlEncode(v);

            return View(model);
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        public ActionResult UpdatePassword()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV = MallUtils.AESDecrypt(v);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string password = WebHelper.GetFormString("password");
            string confirmPwd = WebHelper.GetFormString("confirmPwd");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("password", "密码不能为空");
            }
            if (password.Length < 4 || password.Length > 32)
            {
                return AjaxResult("password", "密码不能小于3且不大于32个字符");
            }
            if (password != confirmPwd)
            {
                return AjaxResult("confirmpwd", "两次密码不相同");
            }

            string p = Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt);
            //设置新密码
            Users.UpdateUserPasswordByUid(WorkContext.Uid, p);
            //同步cookie中密码
            MallUtils.SetCookiePassword(p);

            string url = Url.Action("safesuccess", new RouteValueDictionary { { "act", "updatePassword" } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送更新手机确认短信
        /// </summary>
        public ActionResult SendUpdateMobile()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV = MallUtils.AESDecrypt(v);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string mobile = WebHelper.GetFormString("mobile");

            //检查手机号
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return AjaxResult("mobile", "手机号不能为空");
            }
            if (!ValidateHelper.IsMobile(mobile))
            {
                return AjaxResult("mobile", "手机号格式不正确");
            }
            int tempUid = Users.GetUidByMobile(mobile);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return AjaxResult("mobile", "手机号已经存在");

            string mobileCode = Randoms.CreateRandomValue(6);
            //发送短信
            SMSes.SendSCUpdateSMS(mobile, mobileCode);
            //将验证值保存在session中
            Sessions.SetItem(WorkContext.Sid, "ucsuMobile", mobile);
            Sessions.SetItem(WorkContext.Sid, "ucsuMobileCode", mobileCode);

            return AjaxResult("success", "短信已发送,请查收");
        }

        /// <summary>
        /// 更新手机号
        /// </summary>
        public ActionResult UpdateMobile()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV = MallUtils.AESDecrypt(v);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string mobile = WebHelper.GetFormString("mobile");
            string moibleCode = WebHelper.GetFormString("moibleCode");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查手机号
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return AjaxResult("mobile", "手机号不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsuMobile") != mobile)
            {
                return AjaxResult("mobile", "接收手机不一致");
            }

            //检查手机码
            if (string.IsNullOrWhiteSpace(moibleCode))
            {
                return AjaxResult("moiblecode", "手机码不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsuMobileCode") != moibleCode)
            {
                return AjaxResult("moiblecode", "手机码不正确");
            }

            //更新手机号
            Users.UpdateUserMobileByUid(WorkContext.Uid, mobile);
            //发放验证手机积分
            Credits.SendVerifyMobileCredits(ref WorkContext.PartUserInfo, DateTime.Now);

            string url = Url.Action("safesuccess", new RouteValueDictionary { { "act", "updateMobile" } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送更新邮箱确认邮件
        /// </summary>
        public ActionResult SendUpdateEmail()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV = MallUtils.AESDecrypt(v);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string email = WebHelper.GetFormString("email");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查邮箱
            if (string.IsNullOrWhiteSpace(email))
            {
                return AjaxResult("email", "邮箱不能为空");
            }
            if (!ValidateHelper.IsEmail(email))
            {
                return AjaxResult("email", "邮箱格式不正确");
            }
            if (!SecureHelper.IsSafeSqlString(email, false))
            {
                return AjaxResult("email", "邮箱名不符合系统要求");
            }
            int tempUid = Users.GetUidByEmail(email);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return AjaxResult("email", "邮箱已经存在");


            string v2 = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, email, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("updateemail", new RouteValueDictionary { { "v", v2 } }));

            //发送验证邮件
            Emails.SendSCUpdateEmail(email, WorkContext.UserName, url);
            return AjaxResult("success", "邮件已经发送，请前往你的邮箱进行验证");
        }

        /// <summary>
        /// 更新邮箱
        /// </summary>
        public ActionResult UpdateEmail()
        {
            string v = WebHelper.GetQueryString("v");
            //解密字符串
            string realV;
            try
            {
                realV = MallUtils.AESDecrypt(v);
            }
            catch (Exception ex)
            {
                //如果v来自邮件，那么需要url解码
                realV = MallUtils.AESDecrypt(WebHelper.UrlDecode(v));
            }

            //数组第一项为uid，第二项为邮箱名，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return HttpNotFound();

            int uid = TypeHelper.StringToInt(result[0]);
            string email = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return HttpNotFound();
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return PromptView("此链接已经失效，请重新验证");
            int tempUid = Users.GetUidByEmail(email);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return PromptView("此链接已经失效，邮箱已经存在");

            //更新邮箱名
            Users.UpdateUserEmailByUid(WorkContext.Uid, email);
            //发放验证邮箱积分
            Credits.SendVerifyEmailCredits(ref WorkContext.PartUserInfo, DateTime.Now);

            return RedirectToAction("safesuccess", new RouteValueDictionary { { "act", "updateEmail" }, { "remark", email } });
        }

        /// <summary>
        /// 安全成功
        /// </summary>
        public ActionResult SafeSuccess()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string remark = WebHelper.GetQueryString("remark");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
                return HttpNotFound();

            SafeSuccessModel model = new SafeSuccessModel();
            model.Action = action;
            model.Remark = remark;

            return View(model);
        }

        #endregion

        #region 订单

        /// <summary>
        /// 订单列表
        /// </summary>
        public ActionResult OrderList()
        {
            int page = WebHelper.GetQueryInt("page");
            string startAddTime = WebHelper.GetQueryString("startAddTime");
            string endAddTime = WebHelper.GetQueryString("endAddTime");
            int orderState = WebHelper.GetQueryInt("orderState");

            PageModel pageModel = new PageModel(7, page, Orders.GetUserOrderCount(WorkContext.Uid, startAddTime, endAddTime, orderState));

            DataTable orderList = Orders.GetUserOrderList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber, startAddTime, endAddTime, orderState);
            StringBuilder oidList = new StringBuilder();
            foreach (DataRow row in orderList.Rows)
            {
                oidList.AppendFormat("{0},", row["oid"]);
            }
            if (oidList.Length > 0)
                oidList.Remove(oidList.Length - 1, 1);

            OrderListModel model = new OrderListModel()
            {
                PageModel = pageModel,
                OrderList = orderList,
                OrderProductList = Orders.GetOrderProductList(oidList.ToString()),
                StartAddTime = startAddTime,
                EndAddTime = endAddTime,
                OrderState = orderState
            };

            return View(model);
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        public ActionResult OrderInfo()
        {
            int oid = WebHelper.GetQueryInt("oid");
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return PromptView("订单不存在");

            OrderInfoModel model = new OrderInfoModel();
            model.OrderInfo = orderInfo;
            model.RegionInfo = Regions.GetRegionById(orderInfo.RegionId);
            model.OrderProductList = AdminOrders.GetOrderProductList(oid);
            model.OrderActionList = OrderActions.GetOrderActionList(oid);

            return View(model);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public ActionResult CancelOrder()
        {
            int oid = WebHelper.GetFormInt("oid");
            int cancelReason = WebHelper.GetFormInt("cancelReason");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noorder", "订单不存在");

            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                return AjaxResult("donotcancel", "订单当前不能取消");

            //取消订单
            Orders.CancelOrder(ref WorkContext.PartUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            //创建订单处理
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = "本人",
                ActionType = (int)OrderActionType.Cancel,
                ActionTime = DateTime.Now,
                ActionDes = "您取消了订单"
            });
            return AjaxResult("success", oid.ToString());
        }

        #endregion

        #region 商品收藏夹

        /// <summary>
        /// 收藏夹商品列表
        /// </summary>
        public ActionResult FavoriteProductList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数
            string storeName = WebHelper.GetQueryString("storeName").Trim();//店铺名称
            string productName = WebHelper.GetQueryString("productName").Trim();//商品名称

            PageModel pageModel = new PageModel(10, page, (storeName.Length > 0 || productName.Length > 0) ? FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid, storeName, productName) : FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid));

            FavoriteProductListModel model = new FavoriteProductListModel()
            {
                ProductList = (storeName.Length > 0 || productName.Length > 0) ? FavoriteProducts.GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid, storeName, productName) : FavoriteProducts.GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel,
                ProductName = productName
            };

            return View(model);
        }

        /// <summary>
        /// 添加商品到收藏夹
        /// </summary>
        public ActionResult AddProductToFavorite()
        {
            //商品id
            int pid = WebHelper.GetQueryInt("pid");
            //商品信息
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            //当收藏夹中已经存在此商品时
            if (FavoriteProducts.IsExistFavoriteProduct(WorkContext.Uid, pid))
                return AjaxResult("exist", "商品已经存在");

            //收藏夹已满时
            if (WorkContext.MallConfig.FavoriteProductCount <= FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid))
                return AjaxResult("full", "收藏夹已满");

            bool result = FavoriteProducts.AddProductToFavorite(WorkContext.Uid, pid, 0, DateTime.Now);

            if (result)//添加成功
                return AjaxResult("success", "收藏成功");
            else//添加失败
                return AjaxResult("error", "收藏失败");
        }

        /// <summary>
        /// 删除收藏夹中的商品
        /// </summary>
        public ActionResult DelFavoriteProduct()
        {
            int pid = WebHelper.GetQueryInt("pid");//商品id
            bool result = FavoriteProducts.DeleteFavoriteProductByUidAndPid(WorkContext.Uid, pid);
            if (result)//删除成功
                return AjaxResult("success", pid.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        #endregion

        #region 店铺收藏夹

        /// <summary>
        /// 收藏夹店铺列表
        /// </summary>
        public ActionResult FavoriteStoreList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数

            PageModel pageModel = new PageModel(10, page, FavoriteStores.GetFavoriteStoreCount(WorkContext.Uid));

            FavoriteStoreListModel model = new FavoriteStoreListModel()
            {
                StoreList = FavoriteStores.GetFavoriteStoreList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel
            };

            return View(model);
        }

        /// <summary>
        /// 添加店铺到收藏夹
        /// </summary>
        public ActionResult AddStoreToFavorite()
        {
            //店铺id
            int storeId = WebHelper.GetQueryInt("storeId");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            //当收藏夹中已经存在此店铺时
            if (FavoriteStores.IsExistFavoriteStore(WorkContext.Uid, storeId))
                return AjaxResult("exist", "店铺已经存在");

            //收藏夹已满时
            if (WorkContext.MallConfig.FavoriteStoreCount <= FavoriteStores.GetFavoriteStoreCount(WorkContext.Uid))
                return AjaxResult("full", "收藏夹已满");

            bool result = FavoriteStores.AddStoreToFavorite(WorkContext.Uid, storeId, DateTime.Now);

            if (result)//添加成功
                return AjaxResult("success", "收藏成功");
            else//添加失败
                return AjaxResult("error", "收藏失败");
        }

        /// <summary>
        /// 删除收藏夹中的店铺
        /// </summary>
        public ActionResult DelFavoriteStore()
        {
            int storeId = WebHelper.GetQueryInt("storeId");//店铺id
            bool result = FavoriteStores.DeleteFavoriteStoreByUidAndStoreId(WorkContext.Uid, storeId);
            if (result)//删除成功
                return AjaxResult("success", storeId.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        #endregion

        #region 配送地址

        /// <summary>
        /// 配送地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxShipAddressList()
        {
            List<FullShipAddressInfo> shipAddressList = ShipAddresses.GetFullShipAddressList(WorkContext.Uid);
            int shipAddressCount = shipAddressList.Count;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"count\":");
            sb.AppendFormat("\"{0}\",\"list\":[", shipAddressCount);
            foreach (FullShipAddressInfo fullShipAddressInfo in shipAddressList)
            {
                sb.AppendFormat("{0}\"saId\":\"{1}\",\"user\":\"{2}&nbsp;&nbsp;&nbsp;{3}\",\"address\":\"{4}&nbsp;{5}&nbsp;{6}&nbsp;{7}\"{8},", "{", fullShipAddressInfo.SAId, fullShipAddressInfo.Consignee, fullShipAddressInfo.Mobile.Length > 0 ? fullShipAddressInfo.Mobile : fullShipAddressInfo.Phone, fullShipAddressInfo.ProvinceName, fullShipAddressInfo.CityName, fullShipAddressInfo.CountyName, fullShipAddressInfo.Address, "}");
            }
            if (shipAddressCount > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 配送地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipAddressList()
        {
            ShipAddressListModel model = new ShipAddressListModel();

            model.ShipAddressList = ShipAddresses.GetFullShipAddressList(WorkContext.Uid);
            model.ShipAddressCount = model.ShipAddressList.Count;

            return View(model);
        }

        /// <summary>
        /// 配送地址信息
        /// </summary>
        public ActionResult ShipAddressInfo()
        {
            int saId = WebHelper.GetQueryInt("saId");
            FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            //检查地址
            if (fullShipAddressInfo == null)
                return AjaxResult("noexist", "地址不存在");

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}\"saId\":\"{1}\",\"uid\":\"{2}\",\"regionId\":\"{3}\",\"isDefault\":\"{4}\",\"alias\":\"{5}\",\"consignee\":\"{6}\",\"mobile\":\"{7}\",\"phone\":\"{8}\",\"email\":\"{9}\",\"zipCode\":\"{10}\",\"address\":\"{11}\",\"provinceId\":\"{12}\",\"provinceName\":\"{13}\",\"cityId\":\"{14}\",\"cityName\":\"{15}\",\"countyId\":\"{16}\",\"countyName\":\"{17}\"{18}", "{", fullShipAddressInfo.SAId, fullShipAddressInfo.Uid, fullShipAddressInfo.RegionId, fullShipAddressInfo.IsDefault, fullShipAddressInfo.Alias, fullShipAddressInfo.Consignee, fullShipAddressInfo.Mobile, fullShipAddressInfo.Phone, fullShipAddressInfo.Email, fullShipAddressInfo.ZipCode, fullShipAddressInfo.Address, fullShipAddressInfo.ProvinceId, fullShipAddressInfo.ProvinceName, fullShipAddressInfo.CityId, fullShipAddressInfo.CityName, fullShipAddressInfo.CountyId, fullShipAddressInfo.CountyName, "}");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 添加配送地址
        /// </summary>
        public ActionResult AddShipAddress()
        {
            int regionId = WebHelper.GetFormInt("regionId");
            string alias = WebHelper.GetFormString("alias");
            string consignee = WebHelper.GetFormString("consignee");
            string mobile = WebHelper.GetFormString("mobile");
            string phone = WebHelper.GetFormString("phone");
            string email = WebHelper.GetFormString("email");
            string zipcode = WebHelper.GetFormString("zipcode");
            string address = WebHelper.GetFormString("address");
            int isDefault = WebHelper.GetFormInt("isDefault");

            string verifyResult = VerifyShipAddress(regionId, alias, consignee, mobile, phone, email, zipcode, address);

            if (verifyResult.Length == 0)
            {
                //检查配送地址数量是否达到系统所允许的最大值
                int shipAddressCount = ShipAddresses.GetShipAddressCount(WorkContext.Uid);
                if (shipAddressCount >= WorkContext.MallConfig.MaxShipAddress)
                    return AjaxResult("full", "配送地址的数量已经达到系统所允许的最大值");

                ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
                shipAddressInfo.Uid = WorkContext.Uid;
                shipAddressInfo.RegionId = regionId;
                shipAddressInfo.IsDefault = isDefault == 0 ? 0 : 1;
                shipAddressInfo.Alias = WebHelper.HtmlEncode(alias);
                shipAddressInfo.Consignee = WebHelper.HtmlEncode(consignee);
                shipAddressInfo.Mobile = mobile;
                shipAddressInfo.Phone = phone;
                shipAddressInfo.Email = email;
                shipAddressInfo.ZipCode = zipcode;
                shipAddressInfo.Address = WebHelper.HtmlEncode(address);
                int saId = ShipAddresses.CreateShipAddress(shipAddressInfo);
                return AjaxResult("success", saId.ToString());
            }
            else
            {
                return AjaxResult("error", verifyResult, true);
            }
        }

        /// <summary>
        /// 编辑配送地址
        /// </summary>
        public ActionResult EditShipAddress()
        {
            int saId = WebHelper.GetQueryInt("saId");
            int regionId = WebHelper.GetFormInt("regionId");
            string alias = WebHelper.GetFormString("alias");
            string consignee = WebHelper.GetFormString("consignee");
            string mobile = WebHelper.GetFormString("mobile");
            string phone = WebHelper.GetFormString("phone");
            string email = WebHelper.GetFormString("email");
            string zipcode = WebHelper.GetFormString("zipcode");
            string address = WebHelper.GetFormString("address");
            int isDefault = WebHelper.GetFormInt("isDefault");

            string verifyResult = VerifyShipAddress(regionId, alias, consignee, mobile, phone, email, zipcode, address);
            if (verifyResult.Length == 0)
            {
                ShipAddressInfo shipAddressInfo = ShipAddresses.GetShipAddressBySAId(saId, WorkContext.Uid);
                //检查地址
                if (shipAddressInfo == null)
                    return AjaxResult("noexist", "配送地址不存在");

                shipAddressInfo.Uid = WorkContext.Uid;
                shipAddressInfo.RegionId = regionId;
                shipAddressInfo.IsDefault = isDefault == 0 ? 0 : 1;
                shipAddressInfo.Alias = WebHelper.HtmlEncode(alias);
                shipAddressInfo.Consignee = WebHelper.HtmlEncode(consignee);
                shipAddressInfo.Mobile = mobile;
                shipAddressInfo.Phone = phone;
                shipAddressInfo.Email = email;
                shipAddressInfo.ZipCode = zipcode;
                shipAddressInfo.Address = WebHelper.HtmlEncode(address);
                ShipAddresses.UpdateShipAddress(shipAddressInfo);
                return AjaxResult("success", "编辑成功");
            }
            else
            {
                return AjaxResult("error", verifyResult, true);
            }
        }

        /// <summary>
        /// 删除配送地址
        /// </summary>
        public ActionResult DelShipAddress()
        {
            int saId = WebHelper.GetQueryInt("saId");
            bool result = ShipAddresses.DeleteShipAddress(saId, WorkContext.Uid);
            if (result)//删除成功
                return AjaxResult("success", saId.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        /// <summary>
        /// 设置默认配送地址
        /// </summary>
        public ActionResult SetDefaultShipAddress()
        {
            int saId = WebHelper.GetQueryInt("saId");
            bool result = ShipAddresses.UpdateShipAddressIsDefault(saId, WorkContext.Uid, 1);
            if (result)//设置成功
                return AjaxResult("success", saId.ToString());
            else//设置失败
                return AjaxResult("error", "设置失败");
        }

        /// <summary>
        /// 验证配送地址
        /// </summary>
        private string VerifyShipAddress(int regionId, string alias, string consignee, string mobile, string phone, string email, string zipcode, string address)
        {
            StringBuilder errorList = new StringBuilder("[");

            //检查区域
            RegionInfo regionInfo = Regions.GetRegionById(regionId);
            if (regionInfo == null || regionInfo.Layer != 3)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "regionId", "请选择有效的区域", "}");

            //检查地址别名
            if (string.IsNullOrWhiteSpace(alias))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "alias", "别名不能为空", "}");
            else if (alias.Length > 25)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "alias", "最多只能输入25个字", "}");

            //检查收货人
            if (string.IsNullOrWhiteSpace(consignee))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "consignee", "收货人不能为空", "}");
            else if (consignee.Length > 10)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "consignee", "最多只能输入10个字", "}");

            //检查手机号和固话号
            if (string.IsNullOrWhiteSpace(mobile) && string.IsNullOrWhiteSpace(phone))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "mobile", "手机号和固话号必填一项", "}");
            }
            else
            {
                if (!ValidateHelper.IsMobile(mobile))
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "mobile", "手机号格式不正确", "}");
                if (!ValidateHelper.IsPhone(phone))
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "phone", "固话号格式不正确", "}");
            }

            //检查邮箱
            if (!ValidateHelper.IsEmail(email))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "email", "邮箱格式不正确", "}");

            //检查邮编
            if (!ValidateHelper.IsZipCode(zipcode))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "zipcode", "邮编格式不正确", "}");

            //检查详细地址
            if (string.IsNullOrWhiteSpace(address))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "详细地址不能为空", "}");
            else if (address.Length > 75)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "最多只能输入75个字", "}");

            if (errorList.Length > 1)
                return errorList.Remove(errorList.Length - 1, 1).Append("]").ToString();
            else
                return "";
        }

        #endregion

        #region 用户支付积分

        /// <summary>
        /// 支付积分日志列表
        /// </summary>
        public ActionResult PayCreditLogList()
        {
            int type = WebHelper.GetQueryInt("type", 2);
            int page = WebHelper.GetQueryInt("page");

            PageModel pageModel = new PageModel(10, page, Credits.GetPayCreditLogCount(WorkContext.Uid, type));
            PayCreditLogListModel model = new PayCreditLogListModel()
            {
                ListType = type,
                PageModel = pageModel,
                PayCreditLogList = Credits.GetPayCreditLogList(WorkContext.Uid, type, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        #endregion

        #region 优惠劵

        /// <summary>
        /// 优惠劵列表
        /// </summary>
        public ActionResult CouponList()
        {
            int type = WebHelper.GetQueryInt("type");

            CouponListModel model = new CouponListModel()
            {
                ListType = type,
                CouponList = Coupons.GetCouponList(WorkContext.Uid, type)
            };

            return View(model);
        }

        /// <summary>
        /// 激活优惠劵
        /// </summary>
        public ActionResult ActivateCoupon()
        {
            string activateKey1 = WebHelper.GetFormString("activateKey1");
            string activateKey2 = WebHelper.GetFormString("activateKey2");
            string activateKey3 = WebHelper.GetFormString("activateKey3");
            string activateKey4 = WebHelper.GetFormString("activateKey4");

            if (activateKey1.Length != 4 || activateKey2.Length != 4 || activateKey3.Length != 4 || activateKey4.Length != 4)
                return AjaxResult("errorcouponsn", "优惠劵编号不正确");

            //优惠劵编号
            string couponSN = activateKey1 + activateKey2 + activateKey3 + activateKey4;
            //优惠劵
            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
            if (couponInfo == null)
                return AjaxResult("noexist", "优惠劵不存在");
            if (couponInfo.Uid > 0)
                return AjaxResult("used", "优惠劵已使用");
            //优惠劵类型
            CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(couponInfo.CouponTypeId);
            if (couponTypeInfo == null)
                return AjaxResult("nocoupontype", "优惠劵类型不存在");
            if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseEndTime <= DateTime.Now)
                return AjaxResult("expired", "此优惠劵已过期");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(couponTypeInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            Coupons.ActivateCoupon(couponInfo.CouponId, WorkContext.Uid, DateTime.Now, WorkContext.IP);
            return AjaxResult("success", "优惠劵激活成功");
        }

        #endregion

        #region 商品咨询

        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public ActionResult ProductConsultList()
        {
            int page = WebHelper.GetQueryInt("page");

            PageModel pageModel = new PageModel(10, page, ProductConsults.GetUserProductConsultCount(WorkContext.Uid));
            UserProductConsultListModel model = new UserProductConsultListModel()
            {
                PageModel = pageModel,
                ProductConsultList = ProductConsults.GetUserProductConsultList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        #endregion

        #region  订单评价

        /// <summary>
        /// 评价订单
        /// </summary>
        public ActionResult ReviewOrder()
        {
            int oid = WebHelper.GetQueryInt("oid");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return PromptView("订单当前不能评价");
            if (orderInfo.IsReview == 1)
                return PromptView("此订单已经评价");

            ReviewOrderModel model = new ReviewOrderModel()
            {
                OrderInfo = orderInfo,
                OrderProductList = Orders.GetOrderProductList(oid),
                StoreReviewInfo = Stores.GetStoreReviewByOid(oid)
            };
            return View(model);
        }

        /// <summary>
        /// 评价商品
        /// </summary>
        public ActionResult ReviewProduct()
        {
            int oid = WebHelper.GetQueryInt("oid");//订单id
            int recordId = WebHelper.GetQueryInt("recordId");//订单商品记录id
            int star = WebHelper.GetFormInt("star");//星星
            string message = WebHelper.GetFormString("message");//评价内容

            if (star > 5 || star < 1)
                return AjaxResult("wrongstar", "请选择正确的星星");

            if (message.Length == 0)
                return AjaxResult("emptymessage", "请填写评价内容");
            if (message.Length > 100)
                return AjaxResult("muchmessage", "评价内容最多输入100个字");
            //禁止词
            string bannedWord = FilterWords.GetWord(message);
            if (bannedWord != "")
                return AjaxResult("bannedWord", "评价内容中不能包含违禁词");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noexistorder", "订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("nocomplete", "订单还未完成,不能评价");

            OrderProductInfo orderProductInfo = null;
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(oid);
            foreach (OrderProductInfo item in orderProductList)
            {
                if (item.RecordId == recordId)
                {
                    orderProductInfo = item;
                    break;
                }
            }
            if (orderProductInfo == null)
                return AjaxResult("noproduct", "商品不存在");
            //商品已评价
            if (orderProductInfo.IsReview == 1)
                return AjaxResult("reviewed", "商品已经评价");

            int payCredits = Credits.SendReviewProductCredits(ref WorkContext.PartUserInfo, orderProductInfo, DateTime.Now);
            ProductReviewInfo productReviewInfo = new ProductReviewInfo()
            {
                Pid = orderProductInfo.Pid,
                Uid = orderProductInfo.Uid,
                OPRId = orderProductInfo.RecordId,
                Oid = orderProductInfo.Oid,
                ParentId = 0,
                State = 0,
                StoreId = orderProductInfo.StoreId,
                Star = star,
                Quality = 0,
                Message = WebHelper.HtmlEncode(FilterWords.HideWords(message)),
                ReviewTime = DateTime.Now,
                PayCredits = payCredits,
                PName = orderProductInfo.Name,
                PShowImg = orderProductInfo.ShowImg,
                BuyTime = orderProductInfo.AddTime,
                IP = WorkContext.IP
            };
            ProductReviews.ReviewProduct(productReviewInfo);

            orderProductInfo.IsReview = 1;
            if (Orders.IsReviewAllOrderProduct(orderProductList) && Stores.GetStoreReviewByOid(oid) != null)
                Orders.UpdateOrderIsReview(oid, 1);

            return AjaxResult("success", recordId.ToString());
        }

        /// <summary>
        /// 评价店铺
        /// </summary>
        public ActionResult ReviewStore()
        {
            int oid = WebHelper.GetQueryInt("oid");//订单id
            int descriptionStar = WebHelper.GetFormInt("descriptionStar");//商品描述星星
            int serviceStar = WebHelper.GetFormInt("serviceStar");//商家服务星星
            int shipStar = WebHelper.GetFormInt("shipStar");//商家配送星星

            if (descriptionStar > 5 || descriptionStar < 1)
                return AjaxResult("wrongdescriptionstar", "请选择正确的商品描述星星");
            if (serviceStar > 5 || serviceStar < 1)
                return AjaxResult("wrongservicestar", "请选择正确的商家服务星星");
            if (shipStar > 5 || shipStar < 1)
                return AjaxResult("wrongshipstar", "请选择正确的商家配送星星");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noexistorder", "订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("nocomplete", "订单还未完成,不能评价");

            StoreReviewInfo storeReviewInfo = Stores.GetStoreReviewByOid(oid);
            if (storeReviewInfo != null)
                return AjaxResult("reviewed", "店铺已经评价");

            storeReviewInfo = new StoreReviewInfo()
            {
                Oid = oid,
                StoreId = orderInfo.StoreId,
                DescriptionStar = descriptionStar,
                ServiceStar = serviceStar,
                ShipStar = shipStar,
                Uid = WorkContext.Uid,
                ReviewTime = DateTime.Now,
                IP = WorkContext.IP
            };
            Stores.CreateStoreReview(storeReviewInfo);

            if (Orders.IsReviewAllOrderProduct(Orders.GetOrderProductList(oid)))
                Orders.UpdateOrderIsReview(oid, 1);

            return AjaxResult("success", "店铺评价成功");
        }

        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult ProductReviewList()
        {
            int page = WebHelper.GetQueryInt("page", 1);

            PageModel pageModel = new PageModel(10, page, ProductReviews.GetUserProductReviewCount(WorkContext.Uid));
            UserProductReviewListModel model = new UserProductReviewListModel()
            {
                PageModel = pageModel,
                ProductReviewList = ProductReviews.GetUserProductReviewList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        #endregion

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            if (WorkContext.Uid < 1)
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = Content("nologin");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
    }
}
