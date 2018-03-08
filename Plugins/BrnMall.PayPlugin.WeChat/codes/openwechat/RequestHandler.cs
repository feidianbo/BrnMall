﻿using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BrnMall.PayPlugin.OpenWeChat
{
    /**
    '签名工具类
     ============================================================================/// <summary>
    'api说明：
    'init();
    '初始化函数，默认给一些参数赋值。
    'setKey(key_)'设置商户密钥
    'createMd5Sign(signParams);字典生成Md5签名
    'genPackage(packageParams);获取package包
    'createSHA1Sign(signParams);创建签名SHA1
    'parseXML();输出xml
     * 
     * ============================================================================
     */

    public class RequestHandler
    {
        //页面 this
        protected HttpContext httpContext;
        //哈希值
        protected Hashtable parameters;

        public RequestHandler(HttpContext httpContext)
        {
            parameters = new Hashtable();
            this.httpContext = httpContext;
        }

        #region 参数=======================================
        /** 初始化函数 */
        public virtual void init()
        {
        }

        /** 设置参数值 */
        public void setParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }
        #endregion

        #region 辅助方法===================================
        /// <summary>
        /// 获取paySign 签名
        /// </summary>
        /// <param name="key">key 秘钥的字符名称 就是叫 key</param>
        /// <param name="value">秘钥</param>
        /// <returns></returns>
        public virtual string CreateMd5Sign(string key, string value)
        {
            var sb = new StringBuilder();

            var akeys = new ArrayList(parameters.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                var v = (string) parameters[k];
                if (null != v && "".CompareTo(v) != 0 && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append(key + "=" + value);
            //return sb.ToString();

            string sign = MD5Util.GetMD5(sb.ToString(), getCharset()).ToUpper();
            return sign;
        }

        /// <summary>
        /// 获取预支付 XML 参数组合
        /// </summary>
        /// <returns></returns>
        public string parseXML()
        {
            var sb = new StringBuilder();
            sb.Append("<xml>");
            var akeys = new ArrayList(parameters.Keys);
            foreach (string k in akeys)
            {
                var v = (string) parameters[k];
                if (Regex.IsMatch(v, @"^[0-9.]$"))
                {
                    sb.Append("<" + k + ">" + v + "</" + k + ">");
                }
                else
                {
                    sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
                }
            }
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取编码方式
        /// </summary>
        /// <returns></returns>
        protected virtual string getCharset()
        {
            return httpContext.Request.ContentEncoding.BodyName;
        }
        #endregion
    }
}