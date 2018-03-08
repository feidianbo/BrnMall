﻿using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using BrnMall.Core;
using BrnMall.Services;
using BrnMall.Web.Framework;

namespace BrnMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 活动专题列表模型类
    /// </summary>
    public class TopicListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 专题列表
        /// </summary>
        public DataTable TopicList { get; set; }
        /// <summary>
        /// 专题编号
        /// </summary>
        public string TopicSN { get; set; }
        /// <summary>
        /// 专题标题
        /// </summary>
        public string TopicTitle { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
    }

    /// <summary>
    /// 活动专题模型类
    /// </summary>
    public class TopicModel
    {
        public TopicModel()
        {
            StartTime = EndTime = DateTime.Now;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required(ErrorMessage = "请选择开始时间")]
        [DisplayName("开始时间")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Required(ErrorMessage = "请选择结束时间")]
        [DateTimeNotLess("StartTime", "开始时间")]
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "请填写标题")]
        [StringLength(50, ErrorMessage = "最多输入50个字")]
        public string Title { get; set; }
        /// <summary>
        /// 头部html
        /// </summary>
        [AllowHtml]
        public string HeadHtml { get; set; }
        /// <summary>
        /// 主体html
        /// </summary>
        [AllowHtml]
        public string BodyHtml { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }
    }
}
