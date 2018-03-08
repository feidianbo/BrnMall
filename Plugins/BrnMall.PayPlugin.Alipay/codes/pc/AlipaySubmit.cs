﻿using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace BrnMall.PayPlugin.Alipay
{
    /// <summary>
    /// 支付宝各接口请求提交类,构造支付宝各接口表单HTML文本，获取远程HTTP数据
    /// </summary>
    public class AlipaySubmit
    {
        /// <summary>
        /// 生成请求时的签名
        /// </summary>
        /// <param name="sPara">请求给支付宝的参数数组</param>
        /// <param name="signType">签名方式</param>
        /// <param name="key">交易安全校验码</param>
        /// <param name="code">字符编码格式</param>
        /// <returns>签名结果</returns>
        private static string BuildRequestMysign(Dictionary<string, string> sPara, string signType, string key, Encoding code)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = AlipayCore.CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign = "";
            switch (signType)
            {
                case "MD5":
                    mysign = AlipayMD5.Sign(prestr, key, code);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="signType">签名方式</param>
        /// <param name="key">交易安全校验码</param>
        /// <param name="code">字符编码格式</param>
        /// <returns>要请求的参数数组</returns>
        public static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp, string signType, string key, Encoding code)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            //签名结果
            string mysign = "";

            //过滤签名参数数组
            sPara = AlipayCore.FilterPara(sParaTemp);

            //获得签名结果
            mysign = BuildRequestMysign(sPara, signType, key, code);

            //签名结果与签名方式加入请求提交参数组中
            sPara.Add("sign", mysign);
            sPara.Add("sign_type", signType);

            return sPara;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="signType">签名方式</param>
        /// <param name="key">交易安全校验码</param>
        /// <param name="code">字符编码</param>
        /// <returns>要请求的参数数组字符串</returns>
        private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, string signType, string key, Encoding code)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            sPara = BuildRequestPara(sParaTemp, signType, key, code);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            string strRequestData = AlipayCore.CreateLinkStringUrlencode(sPara, code);

            return strRequestData;
        }



        /// <summary>
        /// 建立请求，以表单HTML形式构造（默认）
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="signType">Type of the sign.</param>
        /// <param name="key">The key.</param>
        /// <param name="code">The code.</param>
        /// <param name="gateway">The gateway.</param>
        /// <param name="inputCharset">The input charset.</param>
        /// <param name="strMethod">提交方式。两个值可选：post、get</param>
        /// <param name="strButtonValue">确认按钮显示文字</param>
        /// <returns>提交表单HTML文本</returns>
        public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string signType, string key, Encoding code, string gateway, string inputCharset, string strMethod, string strButtonValue)
        {
            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara = BuildRequestPara(sParaTemp, signType, key, code);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + gateway + "_input_charset=" + inputCharset + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        /// <summary>
        /// 用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
        /// 注意：远程解析XML出错，与IIS服务器配置有关
        /// </summary>
        /// <param name="gateway">支付宝网关地址</param>
        /// <param name="partner">合作者身份ID</param>
        /// <param name="inputCharset">字符编码格式(文本)</param>
        /// <returns>时间戳字符串</returns>
        public static string Query_timestamp(string gateway, string partner, string inputCharset)
        {
            string url = string.Format("{0}service=query_timestamp&partner={1}&_input_charset={2}", gateway, partner, inputCharset);
            string encrypt_key = "";

            XmlTextReader Reader = new XmlTextReader(url);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Reader);

            encrypt_key = xmlDoc.SelectSingleNode("/alipay/response/timestamp/encrypt_key").InnerText;

            return encrypt_key;
        }
    }
}