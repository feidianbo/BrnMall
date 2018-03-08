﻿using System;
using System.IO;

namespace BrnMall.Core
{
    /// <summary>
    /// BrnMall邮件管理类
    /// </summary>
    public class BMAEmail
    {
        private static IEmailStrategy _iemailstrategy = null;//邮件策略

        static BMAEmail()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "BrnMall.EmailStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _iemailstrategy = (IEmailStrategy)Activator.CreateInstance(Type.GetType(string.Format("BrnMall.EmailStrategy.{0}.EmailStrategy, BrnMall.EmailStrategy.{0}", fileNameList[0].Substring(fileNameList[0].IndexOf("EmailStrategy.") + 14).Replace(".dll", "")),
                                                                                       false,
                                                                                       true));
            }
            catch
            {
                throw new BMAException("创建'邮件策略对象'失败,可能存在的原因:未将'邮件策略程序集'添加到bin目录中;'邮件策略程序集'文件名不符合'BrnMall.EmailStrategy.{策略名称}.dll'格式");
            }
        }

        /// <summary>
        /// 邮件策略实例
        /// </summary>
        public static IEmailStrategy Instance
        {
            get { return _iemailstrategy; }
        }
    }
}
