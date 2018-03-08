﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 导航列表模型类
    /// </summary>
    public class NavListModel
    {
        /// <summary>
        /// 导航列表
        /// </summary>
        public List<NavInfo> NavList { get; set; }
    }

    /// <summary>
    /// 导航模型类
    /// </summary>
    public class NavModel
    {
        /// <summary>
        /// 父导航id
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择父导航")]
        [DisplayName("父导航")]
        public int Pid { get; set; }
        /// <summary>
        /// 导航名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(25, ErrorMessage = "名称长度不能大于25")]
        public string NavName { get; set; }

        /// <summary>
        /// 导航提示
        /// </summary>
        [StringLength(125, ErrorMessage = "提示长度不能大于125")]
        public string NavTitle { get; set; }

        /// <summary>
        /// 导航地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(125, ErrorMessage = "地址长度不能大于125")]
        [BrnMall.Web.Framework.Url]
        public string NavUrl { get; set; }

        /// <summary>
        /// 导航打开目标
        /// </summary>
        [Range(0, 1, ErrorMessage = "请选择正确的目标类型")]
        [DisplayName("目标")]
        public int Target { get; set; }

        /// <summary>
        /// 导航排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }
    }
}
