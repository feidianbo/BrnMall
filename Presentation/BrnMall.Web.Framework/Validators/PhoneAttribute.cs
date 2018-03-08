﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BrnMall.Web.Framework
{
    /// <summary>
    /// 固话号验证属性
    /// </summary>
    public class PhoneAttribute : ValidationAttribute
    {
        public PhoneAttribute()
        {
            ErrorMessage = "不是有效的固话号";
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            else return BrnMall.Core.ValidateHelper.IsPhone(value.ToString());

        }
    }
}
