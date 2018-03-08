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
    /// 订单控制器类
    /// </summary>
    public partial class OrderController : BaseWebController
    {
        private static object _locker = new object();//锁对象

        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrder()
        {
            //选中的购物车项键列表
            string selectedCartItemKeyList = WebHelper.GetRequestString("selectedCartItemKeyList");
            //配送地址id
            int saId = WebHelper.GetFormInt("saId");
            //支付插件名称
            string payName = WebHelper.GetFormString("payName");

            //订单商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
            if (orderProductList.Count < 1)
                return PromptView("购物车中没有商品，请先添加商品");

            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);
            if (Carts.SumMallCartOrderProductCount(storeCartList) < 1)
                return PromptView("请先选择购物车商品");

            ConfirmOrderModel model = new ConfirmOrderModel();

            model.SelectedCartItemKeyList = selectedCartItemKeyList;

            if (saId > 0)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            if (model.DefaultFullShipAddressInfo == null)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetDefaultFullShipAddress(WorkContext.Uid);

            if (payName.Length > 0)
                model.DefaultPayPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            if (model.DefaultPayPluginInfo == null)
                model.DefaultPayPluginInfo = Plugins.GetDefaultPayPlugin();
            model.PayPluginList = Plugins.GetPayPluginList();

            model.PayCreditName = Credits.PayCreditName;
            model.UserPayCredits = WorkContext.PartUserInfo.PayCredits;
            model.MaxUsePayCredits = Credits.GetOrderMaxUsePayCredits(WorkContext.PartUserInfo.PayCredits);

            List<StoreOrder> storeOrderList = new List<StoreOrder>();
            foreach (StoreCartInfo item in storeCartList)
            {
                StoreOrder storeOrder = new StoreOrder();
                storeOrder.StoreCartInfo = item;
                storeOrder.ProductAmount = Carts.SumOrderProductAmount(item.SelectedOrderProductList);
                storeOrder.FullCut = Carts.SumFullCut(item);
                storeOrder.ShipFee = model.DefaultFullShipAddressInfo != null ? Orders.GetShipFee(model.DefaultFullShipAddressInfo.ProvinceId, model.DefaultFullShipAddressInfo.CityId, item.SelectedOrderProductList) : 0;
                storeOrder.TotalCount = Carts.SumOrderProductCount(item.SelectedOrderProductList);
                storeOrder.TotalWeight = Carts.SumOrderProductWeight(item.SelectedOrderProductList);
                storeOrderList.Add(storeOrder);

                model.AllShipFee += storeOrder.ShipFee;
                model.AllFullCut += storeOrder.FullCut;
                model.AllProductAmount += storeOrder.ProductAmount;
                model.AllTotalCount += storeOrder.TotalCount;
                model.AllTotalWeight += storeOrder.TotalWeight;
            }
            model.StoreOrderList = storeOrderList;

            IPayPlugin payPlugin = (IPayPlugin)model.DefaultPayPluginInfo.Instance;
            model.PayFee = payPlugin.GetPayFee(model.AllProductAmount, DateTime.Now, WorkContext.PartUserInfo);
            model.AllOrderAmount = model.AllProductAmount - model.AllFullCut + model.AllShipFee + model.PayFee;

            model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);

            return View(model);
        }

        /// <summary>
        /// 获得有效的优惠劵列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidCouponList()
        {
            //选中的购物车项键列表
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");
            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (DataRow row in Coupons.GetUnUsedCouponList(WorkContext.Uid).Rows)
            {
                if (TypeHelper.ObjectToInt(row["state"]) == 0)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) == 0 && TypeHelper.ObjectToDateTime(row["usestarttime"]) > DateTime.Now)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) == 0 && TypeHelper.ObjectToDateTime(row["useendtime"]) <= DateTime.Now)
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) > 0 && TypeHelper.ObjectToDateTime(row["activatetime"]) <= DateTime.Now.AddDays(-1 * TypeHelper.ObjectToInt(row["useexpiretime"])))
                {
                    break;
                }
                else if (TypeHelper.ObjectToInt(row["userranklower"]) > WorkContext.UserRid)
                {
                    break;
                }
                else
                {
                    int storeId = TypeHelper.ObjectToInt(row["storeid"]);
                    StoreCartInfo storeCartInfo = storeCartList.Find(x => x.StoreInfo.StoreId == storeId);
                    if (storeCartInfo == null)
                    {
                        break;
                    }
                    if (TypeHelper.ObjectToInt(row["orderamountlower"]) > Carts.SumOrderProductAmount(storeCartInfo.SelectedOrderProductList))
                    {
                        break;
                    }
                    else
                    {
                        int limitStoreCid = TypeHelper.ObjectToInt(row["limitstorecid"]);
                        if (limitStoreCid > 0)
                        {
                            foreach (OrderProductInfo orderProductInfo in storeCartInfo.SelectedOrderProductList)
                            {
                                if (orderProductInfo.Type == 0 && orderProductInfo.StoreCid != limitStoreCid)
                                {
                                    break;
                                }
                            }
                        }

                        if (TypeHelper.ObjectToInt(row["limitproduct"]) == 1)
                        {
                            List<OrderProductInfo> commonOrderProductList = Carts.GetCommonOrderProductList(storeCartInfo.SelectedOrderProductList);
                            if (!Coupons.IsSameCouponType(TypeHelper.ObjectToInt(row["coupontypeid"]), Carts.GetPidList(commonOrderProductList)))
                            {
                                break;
                            }
                        }
                    }
                }
                sb.AppendFormat("{0}\"couponId\":\"{1}\",\"name\":\"{2}\",\"money\":\"{3}\",\"useMode\":\"{4}\"{5},", "{", row["couponid"], row["name"], row["money"], row["usemode"], "}");
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 验证优惠劵编号
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCouponSN()
        {
            string couponSN = WebHelper.GetQueryString("couponSN");//优惠劵编号
            if (couponSN.Length == 0)
                return AjaxResult("emptycouponsn", "请输入优惠劵编号");
            if (couponSN.Length != 16)
                return AjaxResult("errorcouponsn", "优惠劵编号不正确");

            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
            if (couponInfo == null)//不存在
                return AjaxResult("noexist", "优惠劵不存在");
            return AjaxResult("success", "此优惠劵可以正常使用");
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitOrder()
        {
            lock (_locker)
            {
                string selectedCartItemKeyList = WebHelper.GetFormString("selectedCartItemKeyList");//选中的购物车项键列表
                int saId = WebHelper.GetFormInt("saId");//配送地址id
                string payName = WebHelper.GetFormString("payName");//支付方式名称
                int payCreditCount = WebHelper.GetFormInt("payCreditCount");//支付积分
                string[] couponIdList = StringHelper.SplitString(WebHelper.GetFormString("couponIdList"));//客户已经激活的优惠劵
                string[] couponSNList = StringHelper.SplitString(WebHelper.GetFormString("couponSNList"));//客户还未激活的优惠劵
                int fullCut = WebHelper.GetFormInt("fullCut");//满减金额
                DateTime bestTime = TypeHelper.StringToDateTime(WebHelper.GetFormString("bestTime"), new DateTime(1970, 1, 1));//最佳配送时间
                string buyerRemark = WebHelper.GetFormString("buyerRemark");//买家备注
                string verifyCode = WebHelper.GetFormString("verifyCode");//验证码

                //验证验证码
                if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
                {
                    if (string.IsNullOrWhiteSpace(verifyCode))
                    {
                        return AjaxResult("emptyverifycode", "验证码不能为空");
                    }
                    else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                    {
                        return AjaxResult("wrongverifycode", "验证码不正确");
                    }
                }

                //验证买家备注的内容长度
                if (StringHelper.GetStringLength(buyerRemark) > 125)
                    return AjaxResult("muchbuyerremark", "备注最多填写125个字");

                //验证支付方式是否为空
                PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
                if (payPluginInfo == null)
                    return AjaxResult("empaypay", "请选择支付方式");

                //验证配送地址是否为空
                FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
                if (fullShipAddressInfo == null)
                    return AjaxResult("emptysaid", "请选择配送地址");

                //购物车商品列表
                List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
                if (orderProductList.Count < 1)
                    return AjaxResult("emptycart", "购物车中没有商品");

                //店铺购物车列表
                List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);
                if (Carts.SumMallCartOrderProductCount(storeCartList) < 1)
                    return AjaxResult("nonselected", "购物车中没有选中的商品");

                //被选中的店铺数量
                int selectedStoreCount = Carts.SumSelectedStoreCount(storeCartList);

                //验证支付积分
                if (payCreditCount > 0)
                {
                    if (payCreditCount > WorkContext.PartUserInfo.PayCredits)
                        return AjaxResult("noenoughpaycredit", "你使用的" + Credits.PayCreditName + "数超过你所拥有的" + WorkContext.PartUserInfo.PayCredits + "数");
                    if (payCreditCount > Credits.OrderMaxUsePayCredits * selectedStoreCount)
                        return AjaxResult("maxusepaycredit", "此笔订单最多使用" + Credits.OrderMaxUsePayCredits * selectedStoreCount + "个" + Credits.PayCreditName);
                }

                //验证优惠劵
                List<CouponInfo> couponList = new List<CouponInfo>();
                if (couponIdList.Length > 0)
                {
                    foreach (string couponId in couponIdList)
                    {
                        int tempCouponId = TypeHelper.StringToInt(couponId);
                        if (tempCouponId > 0)
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponId(tempCouponId);
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }
                }
                if (couponSNList.Length > 0)
                {
                    foreach (string couponSN in couponSNList)
                    {
                        if (!string.IsNullOrWhiteSpace(couponSN))
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵" + couponSN + "不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }
                }
                foreach (CouponInfo couponInfo in couponList)
                {
                    #region  验证

                    CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(couponInfo.CouponTypeId);
                    if (couponTypeInfo == null)
                    {
                        return AjaxResult("nocoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型不存在");
                    }
                    else if (couponTypeInfo.State == 0)
                    {
                        return AjaxResult("closecoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型已关闭");
                    }
                    else if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseStartTime > DateTime.Now)
                    {
                        return AjaxResult("unstartcoupon", "编号为" + couponInfo.CouponSN + "优惠劵还未到使用时间");
                    }
                    else if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseEndTime <= DateTime.Now)
                    {
                        return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                    }
                    else if (couponTypeInfo.UseExpireTime > 0 && couponInfo.ActivateTime <= DateTime.Now.AddDays(-1 * couponTypeInfo.UseExpireTime))
                    {
                        return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                    }
                    else if (couponTypeInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                    {
                        return AjaxResult("userranklowercoupon", "你的用户等级太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                    }
                    else if (couponList.Count > 1 && couponTypeInfo.UseMode == 1)
                    {
                        return AjaxResult("nomutcoupon", "编号为" + couponInfo.CouponSN + "优惠劵不能叠加使用");
                    }
                    else
                    {
                        StoreCartInfo storeCartInfo = storeCartList.Find(x => x.StoreInfo.StoreId == couponTypeInfo.StoreId);

                        if (storeCartInfo == null)
                        {
                            return AjaxResult("wrongstorecoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能购买对应店铺的商品");
                        }

                        if (couponTypeInfo.OrderAmountLower > Carts.SumOrderProductAmount(storeCartInfo.SelectedOrderProductList))
                        {
                            return AjaxResult("orderamountlowercoupon", "订单金额太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                        }

                        if (couponTypeInfo.LimitStoreCid > 0)
                        {
                            foreach (OrderProductInfo orderProductInfo in storeCartInfo.SelectedOrderProductList)
                            {
                                if (orderProductInfo.Type == 0 && orderProductInfo.StoreCid != couponTypeInfo.LimitStoreCid)
                                {
                                    return AjaxResult("limitstoreclasscoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能在购买" + Stores.GetStoreClassByStoreIdAndStoreCid(couponTypeInfo.StoreId, couponTypeInfo.LimitStoreCid).Name + "类的商品时使用");
                                }
                            }
                        }

                        if (couponTypeInfo.LimitProduct == 1)
                        {
                            List<OrderProductInfo> commonOrderProductList = Carts.GetCommonOrderProductList(storeCartInfo.SelectedOrderProductList);
                            if (!Coupons.IsSameCouponType(couponTypeInfo.CouponTypeId, Carts.GetPidList(commonOrderProductList)))
                                return AjaxResult("limitproductcoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能在购买指定商品时使用");
                        }
                    }

                    #endregion
                }


                string pidList = Carts.GetMallCartPidList(storeCartList);//商品id列表
                List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList);//商品库存列表
                List<SinglePromotionInfo> singlePromotionList = new List<SinglePromotionInfo>();//单品促销活动列表

                //循环店铺购物车列表，依次验证
                foreach (StoreCartInfo storeCartInfo in storeCartList)
                {
                    if (!storeCartInfo.IsSelected) continue;

                    #region 验证购物车商品

                    foreach (CartProductInfo cartProductInfo in storeCartInfo.CartProductList)
                    {
                        if (!cartProductInfo.IsSelected) continue;

                        OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                        //验证商品信息
                        PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                        if (partProductInfo == null)
                        {
                            return AjaxResult("outsaleproduct", "商品" + orderProductInfo.Name + "已经下架，请删除此商品");
                        }
                        if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                        {
                            return AjaxResult("changeproduct", "商品" + orderProductInfo.Name + "信息有变化，请删除后重新添加");
                        }

                        //验证商品库存
                        ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                        if (productStockInfo.Number < orderProductInfo.RealCount)
                        {
                            return AjaxResult("outstock", "商品" + orderProductInfo.Name + "库存不足");
                        }
                        else
                        {
                            productStockInfo.Number -= orderProductInfo.RealCount;
                        }

                        //验证买送促销活动
                        if (orderProductInfo.ExtCode2 > 0)
                        {
                            BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                            if (buySendPromotionInfo == null)
                            {
                                return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                            }
                            else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                            {
                                return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                            }
                            else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                            {
                                orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                if (cartProductInfo.GiftList.Count > 0)
                                {
                                    foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                    {
                                        gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                    }
                                    Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                }
                                return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                            {
                                return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                            }
                        }

                        //验证单品促销活动
                        if (orderProductInfo.ExtCode1 > 0)
                        {
                            SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                            if (singlePromotionInfo == null)
                            {
                                return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                            }
                            if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                            {
                                return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                            }
                            if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                            {
                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                orderProductInfo.PayCredits = 0;
                                orderProductInfo.CouponTypeId = 0;
                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                            {
                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                orderProductInfo.PayCredits = 0;
                                orderProductInfo.CouponTypeId = 0;
                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                            {
                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                orderProductInfo.PayCredits = 0;
                                orderProductInfo.CouponTypeId = 0;
                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) + orderProductInfo.RealCount > singlePromotionInfo.AllowBuyCount)
                            {
                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                orderProductInfo.PayCredits = 0;
                                orderProductInfo.CouponTypeId = 0;
                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            if (singlePromotionInfo.IsStock == 1)
                            {
                                SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                if (temp == null)
                                {
                                    temp = singlePromotionInfo;
                                    singlePromotionList.Add(temp);
                                }

                                if (temp.Stock < orderProductInfo.RealCount)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else
                                {
                                    temp.Stock -= orderProductInfo.RealCount;
                                }
                            }
                        }

                        //验证赠品促销活动
                        if (orderProductInfo.ExtCode3 > 0)
                        {
                            GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                            if (giftPromotionInfo == null)
                            {
                                return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                            }
                            else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                            {
                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                            }
                            else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                            {
                                orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                            {
                                orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }
                            else if (cartProductInfo.GiftList.Count > 0)
                            {
                                List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                {
                                    return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                }
                                List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                {
                                    OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                    Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                    newGiftOrderProductList.Add(giftOrderProduct);
                                }

                                //验证赠品信息是否改变
                                for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                {
                                    OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                    OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                    if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                        newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                        newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                        newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                        newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                        newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                        newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                        newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                        newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                        newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                        newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                        newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                        newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                    {
                                        return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                    }
                                }

                                //验证赠品库存
                                foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                {
                                    ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                    if (stockInfo.Number < gift.RealCount)
                                    {
                                        if (stockInfo.Number == 0)
                                        {
                                            Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                            if (cartProductInfo.GiftList.Count == 1)
                                            {
                                                orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                            }
                                        }
                                        else
                                        {
                                            gift.RealCount = stockInfo.Number;
                                            Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                        }
                                        return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                    }
                                    else
                                    {
                                        stockInfo.Number -= gift.RealCount;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region 验证购物车套装

                    foreach (CartSuitInfo cartSuitInfo in storeCartInfo.CartSuitList)
                    {
                        if (!cartSuitInfo.IsSelected) continue;

                        SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(cartSuitInfo.PmId, DateTime.Now);
                        if (suitPromotionInfo == null)
                        {
                            return AjaxResult("stopsuit", "商品" + cartSuitInfo.CartProductList[0].OrderProductInfo.Name + "的套装已经停止,请删除此套装");
                        }
                        else if (suitPromotionInfo.UserRankLower > WorkContext.UserRid)
                        {
                            return AjaxResult("userranklowersuit", "你的用户等级太低，无法购买套装" + suitPromotionInfo.Name + "请删除此套装");
                        }
                        else if (suitPromotionInfo.QuotaUpper > 0 && cartSuitInfo.BuyCount > suitPromotionInfo.QuotaUpper)
                        {
                            return AjaxResult("ordermuchsuit", "套装" + suitPromotionInfo.Name + "要求每单最多购买" + suitPromotionInfo.QuotaUpper + "个");
                        }
                        else if (suitPromotionInfo.OnlyOnce == 1 && Promotions.IsJoinSuitPromotion(WorkContext.Uid, suitPromotionInfo.PmId))
                        {
                            return AjaxResult("usermuchsuit", "套装" + suitPromotionInfo.Name + "要求每人最多购买1个");
                        }

                        List<OrderProductInfo> newSuitOrderProductList = new List<OrderProductInfo>();
                        foreach (ExtSuitProductInfo extSuitProductInfo in Promotions.GetExtSuitProductList(cartSuitInfo.PmId))
                        {
                            OrderProductInfo suitOrderProductInfo = Carts.BuildOrderProduct(extSuitProductInfo);
                            Carts.SetSuitOrderProduct(suitOrderProductInfo, cartSuitInfo.BuyCount, extSuitProductInfo.Number, extSuitProductInfo.Discount, suitPromotionInfo);
                            newSuitOrderProductList.Add(suitOrderProductInfo);
                        }
                        List<OrderProductInfo> oldSuitOrderProductList = new List<OrderProductInfo>();
                        foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                        {
                            oldSuitOrderProductList.Add(cartProductInfo.OrderProductInfo);
                        }
                        if (newSuitOrderProductList.Count != oldSuitOrderProductList.Count)
                        {
                            return AjaxResult("changesuit", "套装" + suitPromotionInfo.Name + "已经改变，请删除此套装后重新下单");
                        }
                        else
                        {
                            for (int i = 0; i < newSuitOrderProductList.Count; i++)
                            {
                                OrderProductInfo newSuitOrderProductInfo = newSuitOrderProductList[i];
                                OrderProductInfo oldSuitOrderProductInfo = oldSuitOrderProductList[i];
                                if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                    newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                    newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                    newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                    newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                    newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                    newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                    newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                    newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                    newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2)
                                {
                                    return AjaxResult("changesuit", "套装" + suitPromotionInfo.Name + "已经改变，请删除此套装后重新下单");
                                }
                            }

                            foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                            {
                                OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                                //验证商品信息
                                PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                                if (partProductInfo == null)
                                {
                                    return AjaxResult("outsaleproduct", "套装中的商品" + orderProductInfo.Name + "已经下架，请删除此套装");
                                }
                                if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                                {
                                    return AjaxResult("changeproduct", "套装中的商品" + orderProductInfo.Name + "信息有变化，请删除套装后重新添加");
                                }

                                //验证商品库存
                                ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                                if (productStockInfo.Number < orderProductInfo.RealCount)
                                {
                                    return AjaxResult("outstock", "套装中的商品" + orderProductInfo.Name + "库存不足");
                                }
                                else
                                {
                                    productStockInfo.Number -= orderProductInfo.RealCount;
                                }

                                //验证赠品促销活动
                                if (orderProductInfo.ExtCode3 > 0)
                                {
                                    GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                    if (giftPromotionInfo == null)
                                    {
                                        return AjaxResult("stopgift", "套装中的商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此套装后重新添加");
                                    }
                                    else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                    {
                                        return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                    }
                                    else if (cartProductInfo.GiftList.Count > 0)
                                    {
                                        List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                        if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                        {
                                            return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                        }
                                        List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                        foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                        {
                                            OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                            Carts.SetGiftOrderProduct(giftOrderProduct, 2, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                            newGiftOrderProductList.Add(giftOrderProduct);
                                        }

                                        //验证赠品信息是否改变
                                        for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                        {
                                            OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                            OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                            if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                                newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                                newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                                newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                                newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                                newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                                newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                                newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                                newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                                newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2)
                                            {
                                                return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                            }
                                        }

                                        //验证赠品库存
                                        foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                        {
                                            ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                            if (stockInfo.Number < gift.RealCount)
                                            {
                                                if (stockInfo.Number == 0)
                                                {
                                                    Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                    if (cartProductInfo.GiftList.Count == 1)
                                                    {
                                                        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                    }
                                                }
                                                else
                                                {
                                                    gift.RealCount = stockInfo.Number;
                                                    Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                                }
                                                return AjaxResult("outstock", "套装中的商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                            }
                                            else
                                            {
                                                stockInfo.Number -= gift.RealCount;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    #endregion

                    #region 验证购物车满赠

                    foreach (CartFullSendInfo cartFullSendInfo in storeCartInfo.CartFullSendList)
                    {
                        if (cartFullSendInfo.IsEnough && cartFullSendInfo.FullSendMinorOrderProductInfo != null)
                        {
                            if (cartFullSendInfo.FullSendPromotionInfo.UserRankLower > WorkContext.UserRid)
                            {
                                return AjaxResult("userranklowerfullsend", "你的用户等级太低，无法参加商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "的满赠促销活动,请先删除此赠品");
                            }
                            if (cartFullSendInfo.FullSendMinorOrderProductInfo.DiscountPrice != cartFullSendInfo.FullSendPromotionInfo.AddMoney)
                            {
                                return AjaxResult("fullsendadd", "商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "的满赠促销活动金额错误,请删除后重新添加");
                            }
                            if (!Promotions.IsExistFullSendProduct(cartFullSendInfo.FullSendPromotionInfo.PmId, cartFullSendInfo.FullSendMinorOrderProductInfo.Pid, 1))
                            {
                                return AjaxResult("nonfullsendminor", "商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "不是满赠赠品,请删除后再结账");
                            }

                            ProductStockInfo productStockInfo = Products.GetProductStock(cartFullSendInfo.FullSendMinorOrderProductInfo.Pid, productStockList);
                            if (productStockInfo.Number < cartFullSendInfo.FullSendMinorOrderProductInfo.RealCount)
                            {
                                return AjaxResult("outstock", "满赠赠品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "库存不足,请删除此满赠赠品");
                            }
                            else
                            {
                                productStockInfo.Number -= cartFullSendInfo.FullSendMinorOrderProductInfo.RealCount;
                            }
                        }

                        foreach (CartProductInfo cartProductInfo in cartFullSendInfo.FullSendMainCartProductList)
                        {
                            #region 验证

                            if (cartProductInfo.IsSelected) continue;

                            OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                            //验证商品信息
                            PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                            if (partProductInfo == null)
                            {
                                return AjaxResult("outsaleproduct", "商品" + orderProductInfo.Name + "已经下架，请删除此商品");
                            }
                            if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                            {
                                return AjaxResult("changeproduct", "商品" + orderProductInfo.Name + "信息有变化，请删除后重新添加");
                            }

                            //验证商品库存
                            ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                            if (productStockInfo.Number < orderProductInfo.RealCount)
                            {
                                return AjaxResult("outstock", "商品" + orderProductInfo.Name + "库存不足");
                            }
                            else
                            {
                                productStockInfo.Number -= orderProductInfo.RealCount;
                            }

                            //验证买送促销活动
                            if (orderProductInfo.ExtCode2 > 0)
                            {
                                BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                                if (buySendPromotionInfo == null)
                                {
                                    return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                                }
                                else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                                {
                                    return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                                }
                                else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                    orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                    Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                    if (cartProductInfo.GiftList.Count > 0)
                                    {
                                        foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                        {
                                            gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                        }
                                        Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                    }
                                    return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                                {
                                    return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                                }
                            }

                            //验证单品促销活动
                            if (orderProductInfo.ExtCode1 > 0)
                            {
                                SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                if (singlePromotionInfo == null)
                                {
                                    return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                                }
                                if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                                {
                                    return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                                }
                                if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) + orderProductInfo.RealCount > singlePromotionInfo.AllowBuyCount)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.IsStock == 1)
                                {
                                    SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                    if (temp == null)
                                    {
                                        temp = singlePromotionInfo;
                                        singlePromotionList.Add(temp);
                                    }

                                    if (temp.Stock < orderProductInfo.RealCount)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    else
                                    {
                                        temp.Stock -= orderProductInfo.RealCount;
                                    }
                                }
                            }

                            //验证赠品促销活动
                            if (orderProductInfo.ExtCode3 > 0)
                            {
                                GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                if (giftPromotionInfo == null)
                                {
                                    return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                                }
                                else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                {
                                    return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                }
                                else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                    Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                    return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                                {
                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                    Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                    return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (cartProductInfo.GiftList.Count > 0)
                                {
                                    List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                    if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                    {
                                        return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                    }
                                    List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                    foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                    {
                                        OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                        Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                        newGiftOrderProductList.Add(giftOrderProduct);
                                    }

                                    //验证赠品信息是否改变
                                    for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                    {
                                        OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                        OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                        if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                            newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                            newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                            newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                            newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                            newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                            newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                            newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                            newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                            newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                            newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                            newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                            newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                        {
                                            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                        }
                                    }

                                    //验证赠品库存
                                    foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                    {
                                        ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                        if (stockInfo.Number < gift.RealCount)
                                        {
                                            if (stockInfo.Number == 0)
                                            {
                                                Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                if (cartProductInfo.GiftList.Count == 1)
                                                {
                                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                }
                                            }
                                            else
                                            {
                                                gift.RealCount = stockInfo.Number;
                                                Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                            }
                                            return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                        }
                                        else
                                        {
                                            stockInfo.Number -= gift.RealCount;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region 验证购物车满减

                    foreach (CartFullCutInfo cartFullCutInfo in storeCartInfo.CartFullCutList)
                    {
                        if (cartFullCutInfo.FullCutPromotionInfo.UserRankLower > WorkContext.UserRid)
                        {
                            List<OrderProductInfo> updateFullCutOrderProductList = new List<OrderProductInfo>();
                            foreach (CartProductInfo cartProductInfo in cartFullCutInfo.FullCutCartProductList)
                            {
                                cartProductInfo.OrderProductInfo.ExtCode5 = -1 * cartProductInfo.OrderProductInfo.ExtCode5;
                                updateFullCutOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            }
                            Carts.UpdateOrderProductFullCut(updateFullCutOrderProductList);
                            return AjaxResult("userranklowerfullcut", "你的用户等级太低，无法参加" + cartFullCutInfo.FullCutPromotionInfo.Name + "满减促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                        }

                        foreach (CartProductInfo cartProductInfo in cartFullCutInfo.FullCutCartProductList)
                        {
                            #region 验证

                            if (cartProductInfo.IsSelected) continue;

                            OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                            //验证商品信息
                            PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                            if (partProductInfo == null)
                            {
                                return AjaxResult("outsaleproduct", "商品" + orderProductInfo.Name + "已经下架，请删除此商品");
                            }
                            if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                            {
                                return AjaxResult("changeproduct", "商品" + orderProductInfo.Name + "信息有变化，请删除后重新添加");
                            }

                            //验证商品库存
                            ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                            if (productStockInfo.Number < orderProductInfo.RealCount)
                            {
                                return AjaxResult("outstock", "商品" + orderProductInfo.Name + "库存不足");
                            }
                            else
                            {
                                productStockInfo.Number -= orderProductInfo.RealCount;
                            }

                            //验证买送促销活动
                            if (orderProductInfo.ExtCode2 > 0)
                            {
                                BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                                if (buySendPromotionInfo == null)
                                {
                                    return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                                }
                                else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                                {
                                    return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                                }
                                else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                    orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                    Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                    if (cartProductInfo.GiftList.Count > 0)
                                    {
                                        foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                        {
                                            gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                        }
                                        Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                    }
                                    return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                                {
                                    return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                                }
                            }

                            //验证单品促销活动
                            if (orderProductInfo.ExtCode1 > 0)
                            {
                                SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                if (singlePromotionInfo == null)
                                {
                                    return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                                }
                                if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                                {
                                    return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                                }
                                if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) + orderProductInfo.RealCount > singlePromotionInfo.AllowBuyCount)
                                {
                                    orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                    orderProductInfo.PayCredits = 0;
                                    orderProductInfo.CouponTypeId = 0;
                                    orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                    Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                    return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                if (singlePromotionInfo.IsStock == 1)
                                {
                                    SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                    if (temp == null)
                                    {
                                        temp = singlePromotionInfo;
                                        singlePromotionList.Add(temp);
                                    }

                                    if (temp.Stock < orderProductInfo.RealCount)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    else
                                    {
                                        temp.Stock -= orderProductInfo.RealCount;
                                    }
                                }
                            }

                            //验证赠品促销活动
                            if (orderProductInfo.ExtCode3 > 0)
                            {
                                GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                if (giftPromotionInfo == null)
                                {
                                    return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                                }
                                else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                {
                                    return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                }
                                else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                    Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                    return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                                {
                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                    Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                    return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                }
                                else if (cartProductInfo.GiftList.Count > 0)
                                {
                                    List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                    if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                    {
                                        return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                    }
                                    List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                    foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                    {
                                        OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                        Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                        newGiftOrderProductList.Add(giftOrderProduct);
                                    }

                                    //验证赠品信息是否改变
                                    for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                    {
                                        OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                        OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                        if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                            newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                            newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                            newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                            newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                            newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                            newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                            newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                            newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                            newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                            newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                            newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                            newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                        {
                                            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                        }
                                    }

                                    //验证赠品库存
                                    foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                    {
                                        ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                        if (stockInfo.Number < gift.RealCount)
                                        {
                                            if (stockInfo.Number == 0)
                                            {
                                                Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                if (cartProductInfo.GiftList.Count == 1)
                                                {
                                                    orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                    Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                }
                                            }
                                            else
                                            {
                                                gift.RealCount = stockInfo.Number;
                                                Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                            }
                                            return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                        }
                                        else
                                        {
                                            stockInfo.Number -= gift.RealCount;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
                if (Carts.SumMallCartFullCut(storeCartList) != fullCut)
                    return AjaxResult("wrongfullcutmoney", "满减金额不正确,请刷新页面重新提交");

                //验证已经通过,进行订单保存
                int pCount = 0;
                string oidList = "";
                foreach (StoreCartInfo storeCartInfo in storeCartList)
                {
                    List<SinglePromotionInfo> storeSinglePromotionList = singlePromotionList.FindAll(x => x.StoreId == storeCartInfo.StoreInfo.StoreId);
                    List<CouponInfo> storeCouponList = couponList.FindAll(x => x.StoreId == storeCartInfo.StoreInfo.StoreId);
                    int storeFullCut = Carts.SumFullCut(storeCartInfo);
                    OrderInfo orderInfo = Orders.CreateOrder(WorkContext.PartUserInfo, storeCartInfo.StoreInfo, storeCartInfo.SelectedOrderProductList, storeSinglePromotionList, fullShipAddressInfo, payPluginInfo, ref payCreditCount, storeCouponList, storeFullCut, buyerRemark, bestTime, WorkContext.IP);
                    if (orderInfo != null)
                    {
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = WorkContext.Uid,
                            RealName = "本人",
                            ActionType = (int)OrderActionType.Submit,
                            ActionTime = DateTime.Now,
                            ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                        });
                        //删除剩余的满赠赠品
                        if (storeCartInfo.RemainedOrderProductList.Count > 0)
                        {
                            List<OrderProductInfo> delOrderProductList = Carts.GetFullSendMinorOrderProductList(storeCartInfo.RemainedOrderProductList);
                            if (delOrderProductList.Count > 0)
                            {
                                Carts.DeleteOrderProductList(delOrderProductList);
                                pCount += Carts.SumOrderProductCount(storeCartInfo.RemainedOrderProductList) - delOrderProductList.Count;
                            }
                        }
                        oidList += orderInfo.Oid + ",";
                    }
                    else
                    {
                        return AjaxResult("error", "提交失败，请联系管理员");
                    }
                }
                Carts.SetCartProductCountCookie(pCount);
                return AjaxResult("success", Url.Action("submitresult", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
            }
        }

        /// <summary>
        /// 提交结果
        /// </summary>
        public ActionResult SubmitResult()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            string paySystemName = "";
            decimal allPayMoney = 0M;
            string onlinePayOidList = "";
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && (orderInfo.OrderState == (int)OrderState.WaitPaying || orderInfo.OrderState == (int)OrderState.Confirming) && (paySystemName.Length == 0 || paySystemName == orderInfo.PaySystemName))
                    orderList.Add(orderInfo);
                else
                    return Redirect("/");

                paySystemName = orderInfo.PaySystemName;
                if (orderInfo.OrderState == (int)OrderState.WaitPaying)
                {
                    allPayMoney += orderInfo.SurplusMoney;
                    if (orderInfo.PayMode == 1)
                        onlinePayOidList += orderInfo.Oid + ",";
                }
            }
            onlinePayOidList = onlinePayOidList.TrimEnd(',');

            if (orderList.Count < 1)
                return Redirect("/");

            SubmitResultModel model = new SubmitResultModel();
            model.OidList = oidList;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(paySystemName);
            model.AllPayMoney = allPayMoney;
            model.OnlinePayOidList = onlinePayOidList;
            return View(model);
        }

        /// <summary>
        /// 支付展示
        /// </summary>
        public ActionResult PayShow()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            string paySystemName = "";
            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1 && (paySystemName.Length == 0 || paySystemName == orderInfo.PaySystemName))
                    orderList.Add(orderInfo);
                else
                    return Redirect("/");

                paySystemName = orderInfo.PaySystemName;
                allSurplusMoney += orderInfo.SurplusMoney;
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                return Redirect("/");

            PayShowModel model = new PayShowModel();
            model.OidList = oidList;
            model.AllSurplusMoney = allSurplusMoney;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(paySystemName);
            return View(model);
        }

        /// <summary>
        /// 支付结果
        /// </summary>
        public ActionResult PayResult()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            PayResultModel model = new PayResultModel();
            List<OrderInfo> successOrderList = new List<OrderInfo>();
            List<OrderInfo> failOrderList = new List<OrderInfo>();

            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.Confirming)
                    successOrderList.Add(orderInfo);
                else
                    failOrderList.Add(orderInfo);
            }

            model.SuccessOrderList = successOrderList;
            model.FailOrderList = failOrderList;
            model.State = failOrderList.Count > 0 ? 0 : 1;
            return View(model);
        }

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            if (WorkContext.Uid < 1)
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = AjaxResult("nologin", "请先登录");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
    }
}
