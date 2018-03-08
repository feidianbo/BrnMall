﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using BrnMall.Core;

namespace BrnMall.Services
{
    /// <summary>
    /// 商城管理员组操作管理类
    /// </summary>
    public partial class MallAdminGroups
    {
        //商城后台导航菜单栏缓存文件夹
        private const string MallAdminNavMeunCacheFolder = "/admin_mall/menu";

        /// <summary>
        /// 检查当前动作的授权
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <param name="controller">控制器名称</param>
        /// <param name="action">动作方法名称</param>
        /// <returns></returns>
        public static bool CheckAuthority(int mallAGid, string controller, string pageKey)
        {
            //非管理员
            if (mallAGid == 1)
                return false;

            //系统管理员具有一切权限
            if (mallAGid == 2)
                return true;

            HashSet<string> mallAdminActionHashSet = MallAdminActions.GetMallAdminActionHashSet();
            HashSet<string> mallAdminGroupActionHashSet = GetMallAdminGroupActionHashSet(mallAGid);

            //动作方法的优先级大于控制器的优先级
            if ((mallAdminActionHashSet.Contains(pageKey) && mallAdminGroupActionHashSet.Contains(pageKey)) ||
                                    (mallAdminActionHashSet.Contains(controller) && mallAdminGroupActionHashSet.Contains(controller)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得商城管理员组操作HashSet
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static HashSet<string> GetMallAdminGroupActionHashSet(int mallAGid)
        {
            HashSet<string> actionHashSet = BrnMall.Core.BMACache.Get(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid) as HashSet<string>;
            if (actionHashSet == null)
            {
                MallAdminGroupInfo mallAdminGroupInfo = GetMallAdminGroupById(mallAGid);
                if (mallAdminGroupInfo != null)
                {
                    actionHashSet = new HashSet<string>();
                    string[] actionList = StringHelper.SplitString(mallAdminGroupInfo.ActionList);//将动作列表字符串分隔成动作列表
                    foreach (string action in actionList)
                    {
                        actionHashSet.Add(action);
                    }
                    BrnMall.Core.BMACache.Insert(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid, actionHashSet);
                }
            }
            return actionHashSet;
        }

        /// <summary>
        /// 获得商城管理员组列表
        /// </summary>
        /// <returns></returns>
        public static MallAdminGroupInfo[] GetMallAdminGroupList()
        {
            MallAdminGroupInfo[] mallAdminGroupList = BrnMall.Core.BMACache.Get(CacheKeys.MALL_MALLADMINGROUP_LIST) as MallAdminGroupInfo[];
            if (mallAdminGroupList == null)
            {
                mallAdminGroupList = BrnMall.Data.MallAdminGroups.GetMallAdminGroupList();
                BrnMall.Core.BMACache.Insert(CacheKeys.MALL_MALLADMINGROUP_LIST, mallAdminGroupList);
            }
            return mallAdminGroupList;
        }

        /// <summary>
        /// 获得用户级商城管理员组列表
        /// </summary>
        /// <returns></returns>
        public static MallAdminGroupInfo[] GetCustomerMallAdminGroupList()
        {
            MallAdminGroupInfo[] mallAdminGroupList = GetMallAdminGroupList();
            MallAdminGroupInfo[] customerMallAdminGroupList = new MallAdminGroupInfo[mallAdminGroupList.Length - 2];

            int i = 0;
            foreach (MallAdminGroupInfo mallAdminGroupInfo in mallAdminGroupList)
            {
                if (mallAdminGroupInfo.MallAGid > 2)
                {
                    customerMallAdminGroupList[i] = mallAdminGroupInfo;
                    i++;
                }
            }

            return customerMallAdminGroupList;
        }

        /// <summary>
        /// 获得商城管理员组
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static MallAdminGroupInfo GetMallAdminGroupById(int mallAGid)
        {
            foreach (MallAdminGroupInfo mallAdminGroupInfo in GetMallAdminGroupList())
            {
                if (mallAGid == mallAdminGroupInfo.MallAGid)
                    return mallAdminGroupInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得商城管理员组id
        /// </summary>
        /// <param name="title">商城管理员组标题</param>
        /// <returns></returns>
        public static int GetMallAdminGroupIdByTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                foreach (MallAdminGroupInfo mallAdminGroupInfo in GetMallAdminGroupList())
                {
                    if (mallAdminGroupInfo.Title == title)
                        return mallAdminGroupInfo.MallAGid;
                }
            }
            return -1;
        }

        /// <summary>
        /// 创建管理员组
        /// </summary>
        /// <param name="mallAdminGroupInfo">管理员组信息</param>
        public static void CreateMallAdminGroup(MallAdminGroupInfo mallAdminGroupInfo)
        {
            mallAdminGroupInfo.ActionList = mallAdminGroupInfo.ActionList.ToLower();
            int mallAGid = BrnMall.Data.MallAdminGroups.CreateMallAdminGroup(mallAdminGroupInfo);
            if (mallAGid > 0)
            {
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
                mallAdminGroupInfo.MallAGid = mallAGid;
                WriteMallAdminNavMenuCache(mallAdminGroupInfo);
            }
        }

        /// <summary>
        /// 删除商城管理员组
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns>-2代表内置管理员不能删除，-1代表此管理员组下还有会员未删除，0代表删除失败，1代表删除成功</returns>
        public static int DeleteMallAdminGroupById(int mallAGid)
        {
            if (mallAGid < 3)
                return -2;

            if (AdminUsers.GetUserCountByMallAGid(mallAGid) > 0)
                return -1;

            if (mallAGid > 0)
            {
                BrnMall.Data.MallAdminGroups.DeleteMallAdminGroupById(mallAGid);
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid);
                BrnMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
                File.Delete(IOHelper.GetMapPath(MallAdminNavMeunCacheFolder + "/" + mallAGid + ".js"));
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新商城管理员组
        /// </summary>
        public static void UpdateMallAdminGroup(MallAdminGroupInfo mallAdminGroupInfo)
        {
            mallAdminGroupInfo.ActionList = mallAdminGroupInfo.ActionList.ToLower();
            BrnMall.Data.MallAdminGroups.UpdateMallAdminGroup(mallAdminGroupInfo);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAdminGroupInfo.MallAGid);
            BrnMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
            WriteMallAdminNavMenuCache(mallAdminGroupInfo);
        }

        /// <summary>
        /// 将商城管理员组的导航菜单栏缓存写入到文件中
        /// </summary>
        private static void WriteMallAdminNavMenuCache(MallAdminGroupInfo mallAdminGroupInfo)
        {
            HashSet<string> mallAdminGroupActionHashSet = new HashSet<string>();
            string[] actionList = StringHelper.SplitString(mallAdminGroupInfo.ActionList);//将后台操作列表字符串分隔成后台操作列表
            foreach (string action in actionList)
            {
                mallAdminGroupActionHashSet.Add(action);
            }

            bool flag = false;
            StringBuilder menu = new StringBuilder();
            StringBuilder menuList = new StringBuilder("var menuList = [");

            #region 商品管理

            menu.AppendFormat("{0}\"title\":\"商品管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("product"))
            {
                menu.AppendFormat("{0}\"title\":\"添加商品\",\"url\":\"/malladmin/product/addproduct\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"添加SKU\",\"url\":\"/malladmin/product/addsku\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"在售商品\",\"url\":\"/malladmin/product/onsaleproductlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"下架商品\",\"url\":\"/malladmin/product/outsaleproductlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"定时商品\",\"url\":\"/malladmin/product/timeproductlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"回收站\",\"url\":\"/malladmin/product/recyclebinproductlist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 促销活动

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"促销活动\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("promotion"))
            {
                menu.AppendFormat("{0}\"title\":\"单品促销\",\"url\":\"/malladmin/promotion/singlepromotionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"买送促销\",\"url\":\"/malladmin/promotion/buysendpromotionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"赠品促销\",\"url\":\"/malladmin/promotion/giftpromotionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"套装促销\",\"url\":\"/malladmin/promotion/suitpromotionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"满赠促销\",\"url\":\"/malladmin/promotion/fullsendpromotionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"满减促销\",\"url\":\"/malladmin/promotion/fullcutpromotionlist\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("topic"))
            {
                menu.AppendFormat("{0}\"title\":\"专题管理\",\"url\":\"/malladmin/topic/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("coupon"))
            {
                menu.AppendFormat("{0}\"title\":\"优惠劵\",\"url\":\"/malladmin/coupon/coupontypelist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 订单管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"订单管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("order"))
            {
                menu.AppendFormat("{0}\"title\":\"订单列表\",\"url\":\"/malladmin/order/orderlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"退款列表\",\"url\":\"/malladmin/order/refundlist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 咨询评价

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"咨询评价\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("productreview"))
            {
                menu.AppendFormat("{0}\"title\":\"商品评价\",\"url\":\"/malladmin/productreview/productreviewlist\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("productconsult"))
            {
                menu.AppendFormat("{0}\"title\":\"商品咨询\",\"url\":\"/malladmin/productconsult/productconsultlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"咨询类型\",\"url\":\"/malladmin/productconsult/productconsulttypelist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 用户管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"用户管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("user"))
            {
                menu.AppendFormat("{0}\"title\":\"用户列表\",\"url\":\"/malladmin/user/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("userrank"))
            {
                menu.AppendFormat("{0}\"title\":\"会员等级\",\"url\":\"/malladmin/userrank/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("malladmingroup"))
            {
                menu.AppendFormat("{0}\"title\":\"管理员组\",\"url\":\"/malladmin/malladmingroup/list\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 店铺管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"店铺管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("store"))
            {
                menu.AppendFormat("{0}\"title\":\"店铺列表\",\"url\":\"/malladmin/store/storelist\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("storeindustry"))
            {
                menu.AppendFormat("{0}\"title\":\"店铺行业\",\"url\":\"/malladmin/storeindustry/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("storerank"))
            {
                menu.AppendFormat("{0}\"title\":\"店铺等级\",\"url\":\"/malladmin/storerank/list\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 新闻管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"新闻管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("news"))
            {
                menu.AppendFormat("{0}\"title\":\"新闻类型\",\"url\":\"/malladmin/news/newstypelist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"新闻列表\",\"url\":\"/malladmin/news/newslist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 广告管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"广告管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("advert"))
            {
                menu.AppendFormat("{0}\"title\":\"广告位置\",\"url\":\"/malladmin/advert/advertpositionlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"广告列表\",\"url\":\"/malladmin/advert/advertlist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 商城内容

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"商城内容\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("nav"))
            {
                menu.AppendFormat("{0}\"title\":\"导航菜单\",\"url\":\"/malladmin/nav/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("help"))
            {
                menu.AppendFormat("{0}\"title\":\"商城帮助\",\"url\":\"/malladmin/help/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("friendlink"))
            {
                menu.AppendFormat("{0}\"title\":\"友情链接\",\"url\":\"/malladmin/friendlink/list\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 商品性质

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"商品性质\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("brand"))
            {
                menu.AppendFormat("{0}\"title\":\"商品品牌\",\"url\":\"/malladmin/brand/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("category"))
            {
                menu.AppendFormat("{0}\"title\":\"分类管理\",\"url\":\"/malladmin/category/categorylist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 报表统计

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"报表统计\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("stat"))
            {
                menu.AppendFormat("{0}\"title\":\"在线用户\",\"url\":\"/malladmin/stat/onlineuserlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"搜索分析\",\"url\":\"/malladmin/stat/searchwordstatlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"商品统计\",\"url\":\"/malladmin/stat/productstat\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"销售明细\",\"url\":\"/malladmin/stat/saleproductlist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"订单统计\",\"url\":\"/malladmin/stat/orderstat\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"地区统计\",\"url\":\"/malladmin/stat/regionstat\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"客户端统计\",\"url\":\"/malladmin/stat/clientstat\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 系统设置

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"系统设置\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("set"))
            {
                menu.AppendFormat("{0}\"title\":\"站点信息\",\"url\":\"/malladmin/set/site\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"商城设置\",\"url\":\"/malladmin/set/mall\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"账号设置\",\"url\":\"/malladmin/set/account\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"上传设置\",\"url\":\"/malladmin/set/upload\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"性能设置\",\"url\":\"/malladmin/set/performance\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"访问控制\",\"url\":\"/malladmin/set/access\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"邮箱设置\",\"url\":\"/malladmin/set/email\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"短信设置\",\"url\":\"/malladmin/set/sms\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"积分设置\",\"url\":\"/malladmin/set/credit\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"打印订单\",\"url\":\"/malladmin/set/printorder\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("shipcompany"))
            {
                menu.AppendFormat("{0}\"title\":\"配送公司\",\"url\":\"/malladmin/shipcompany/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("bannedip"))
            {
                menu.AppendFormat("{0}\"title\":\"禁止IP\",\"url\":\"/malladmin/bannedip/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("filterword"))
            {
                menu.AppendFormat("{0}\"title\":\"筛选词\",\"url\":\"/malladmin/filterword/list\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 商城插件

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"商城插件\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("plugin"))
            {
                menu.AppendFormat("{0}\"title\":\"授权插件\",\"url\":\"/malladmin/plugin/list?type=0\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"支付插件\",\"url\":\"/malladmin/plugin/list?type=1\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 日志管理

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"日志管理\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("log"))
            {
                menu.AppendFormat("{0}\"title\":\"商城日志\",\"url\":\"/malladmin/log/malladminloglist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"店铺日志\",\"url\":\"/malladmin/log/storeadminloglist\"{1},", "{", "}");
                menu.AppendFormat("{0}\"title\":\"积分日志\",\"url\":\"/malladmin/log/creditloglist\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            #region 开发人员

            flag = false;
            menu = menu.Clear();
            menu.AppendFormat("{0}\"title\":\"开发人员\",\"subMenuList\":[", "{");
            if (mallAdminGroupActionHashSet.Contains("event"))
            {
                menu.AppendFormat("{0}\"title\":\"事件管理\",\"url\":\"/malladmin/event/list\"{1},", "{", "}");
                flag = true;
            }
            if (mallAdminGroupActionHashSet.Contains("database"))
            {
                menu.AppendFormat("{0}\"title\":\"数据库管理\",\"url\":\"/malladmin/database/manage\"{1},", "{", "}");
                flag = true;
            }
            if (flag)
            {
                menu.Remove(menu.Length - 1, 1);
                menu.Append("]},");
                menuList.Append(menu.ToString());
            }

            #endregion

            if (menuList.Length > 16)
                menuList.Remove(menuList.Length - 1, 1);
            menuList.Append("]");

            try
            {
                string fileName = IOHelper.GetMapPath(MallAdminNavMeunCacheFolder + "/" + mallAdminGroupInfo.MallAGid + ".js");
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Byte[] info = System.Text.Encoding.UTF8.GetBytes(menuList.ToString());
                    fs.Write(info, 0, info.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            { }
        }
    }
}
