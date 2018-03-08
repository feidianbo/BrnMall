﻿using System;

namespace BrnMall.Core
{
    /// <summary>
    /// 关系数据库表
    /// </summary>
    public partial class RDBSFields
    {
        /// <summary>
        /// 广告位置表
        /// </summary>
        public const string ADVERT_POSITIONS = "[adposid],[displayorder],[title],[description]";

        /// <summary>
        /// 广告表
        /// </summary>
        public const string ADVERTS = "[adid],[clickcount],[adposid],[state],[starttime],[endtime],[displayorder],[title],[image],[url],[extfield1],[extfield2],[extfield3],[extfield4],[extfield5]";

        /// <summary>
        /// 属性分组表
        /// </summary>
        public const string ATTRIBUTE_GROUPS = "[attrgroupid],[cateid],[name],[displayorder]";

        /// <summary>
        /// 属性表
        /// </summary>
        public const string ATTRIBUTES = "[attrid],[name],[cateid],[attrgroupid],[showtype],[isfilter],[displayorder]";

        /// <summary>
        /// 属性值表
        /// </summary>
        public const string ATTRIBUTE_VALUES = "[attrvalueid],[attrvalue],[isinput],[attrname],[attrdisplayorder],[attrshowtype],[attrvaluedisplayorder],[attrgroupid],[attrgroupname],[attrgroupdisplayorder],[attrid]";

        /// <summary>
        /// 被禁止的ip表
        /// </summary>
        public const string BANNEDIPS = "[id],[ip],[liftbantime]";

        /// <summary>
        /// 品牌表
        /// </summary>
        public const string BRANDS = "[brandid],[displayorder],[name],[logo]";

        /// <summary>
        /// 浏览历史表
        /// </summary>
        public const string BROWSEHISTORIES = "[recordid],[uid],[pid],[times],[updatetime]";

        /// <summary>
        /// 买送促销商品表
        /// </summary>
        public const string BUYSEND_PRODUCTS = "[recordid],[pmid],[pid]";

        /// <summary>
        /// 买送促销表
        /// </summary>
        public const string BUYSEND_PROMOTIONS = "[pmid],[storeid],[starttime],[endtime],[userranklower],[state],[name],[type],[buycount],[sendcount]";

        /// <summary>
        /// 分类表
        /// </summary>
        public const string CATEGORIES = "[cateid],[displayorder],[name],[pricerange],[parentid],[layer],[haschild],[path]";

        /// <summary>
        /// 优惠劵商品表
        /// </summary>
        public const string COUPON_PRODUCTS = "[recordid],[coupontypeid],[pid]";

        /// <summary>
        /// 优惠劵表
        /// </summary>
        public const string COUPONS = "[couponid],[couponsn],[uid],[coupontypeid],[storeid],[oid],[usetime],[useip],[money],[activatetime],[activateip],[createuid],[createoid],[createtime],[createip]";

        /// <summary>
        /// 优惠劵类型表
        /// </summary>
        public const string COUPON_TYPES = "[coupontypeid],[storeid],[state],[name],[money],[count],[sendmode],[getmode],[usemode],[userranklower],[orderamountlower],[limitstorecid],[limitproduct],[sendstarttime],[sendendtime],[useexpiretime],[usestarttime],[useendtime]";

        /// <summary>
        /// 积分日志表
        /// </summary>
        public const string CREDITLOGS = "[logid],[uid],[paycredits],[rankcredits],[action],[actioncode],[actiontime],[actiondes],[operator]";

        /// <summary>
        /// 事件日志表
        /// </summary>
        public const string EVENTLOGS = "[id],[key],[title],[server],[executetime]";

        /// <summary>
        /// 商品收藏夹表
        /// </summary>
        public const string FAVORITEPRODUCTS = "[recordid],[uid],[pid],[state],[addtime]";

        /// <summary>
        /// 店铺收藏夹表
        /// </summary>
        public const string FAVORITESTORES = "[recordid],[uid],[storeid],[addtime]";

        /// <summary>
        /// 筛选词表
        /// </summary>
        public const string FILTERWORDS = "[id],[match],[replace]";

        /// <summary>
        /// 友情链接表
        /// </summary>
        public const string FRIENDLINKS = "[id],[name],[title],[logo],[url],[target],[displayorder]";

        /// <summary>
        /// 满减促销商品表
        /// </summary>
        public const string FULLCUT_PRODUCTS = "[recordid],[pmid],[pid]";

        /// <summary>
        /// 满减促销表
        /// </summary>
        public const string FULLCUT_PROMOTIONS = "[pmid],[storeid],[type],[starttime],[endtime],[userranklower],[state],[name],[limitmoney1],[cutmoney1],[limitmoney2],[cutmoney2],[limitmoney3],[cutmoney3]";

        /// <summary>
        /// 满赠促销商品表
        /// </summary>
        public const string FULLSEND_PRODUCTS = "[recordid],[pmid],[pid],[type]";

        /// <summary>
        /// 满赠促销表
        /// </summary>
        public const string FULLSEND_PROMOTIONS = "[pmid],[storeid],[starttime],[endtime],[userranklower],[state],[name],[limitmoney],[addmoney]";

        /// <summary>
        /// 赠品表
        /// </summary>
        public const string GIFTS = "[recordid],[pmid],[giftid],[number],[pid]";

        /// <summary>
        /// 赠品促销表
        /// </summary>
        public const string GIFT_PROMOTIONS = "[pmid],[pid],[storeid],[starttime1],[endtime1],[starttime2],[endtime2],[starttime3],[endtime3],[userranklower],[state],[name],[quotaupper]";

        /// <summary>
        /// 商城帮助表
        /// </summary>
        public const string HELPS = "[id],[pid],[title],[url],[description],[displayorder]";

        /// <summary>
        /// 登陆失败日志表
        /// </summary>
        public const string LOGINFAILLOGS = "[id],[loginip],[failtimes],[lastlogintime]";

        /// <summary>
        /// 商城后台操作表
        /// </summary>
        public const string MALL_ADMINACTIONS = "[aid],[title],[action],[parentid],[displayorder]";

        /// <summary>
        /// 商城管理员组表
        /// </summary>
        public const string MALL_ADMINGROUPS = "[mallagid],[title],[actionlist]";

        /// <summary>
        /// 商城管理日志表
        /// </summary>
        public const string MALL_ADMINLOGS = "[logid],[uid],[nickname],[mallagid],[mallagtitle],[operation],[description],[ip],[operatetime]";

        /// <summary>
        /// 导航栏表
        /// </summary>
        public const string NAVS = "[id],[pid],[layer],[name],[title],[url],[target],[displayorder]";

        /// <summary>
        /// 新闻表
        /// </summary>
        public const string NEWS = "[newsid],[newstypeid],[isshow],[istop],[ishome],[displayorder],[addtime],[title],[url],[body]";

        /// <summary>
        /// 新闻类型表
        /// </summary>
        public const string NEWS_TYPES = "[newstypeid],[name],[displayorder]";

        /// <summary>
        /// 开放授权表
        /// </summary>
        public const string OAUTH = "[id],[uid],[openid],[server]";

        /// <summary>
        /// 用户在线时间表
        /// </summary>
        public const string ONLINE_TIME = "[uid],[total],[year],[month],[week],[day],[updatetime]";

        /// <summary>
        /// 在线用户表
        /// </summary>
        public const string ONLINE_USERS = "[olid],[uid],[sid],[nickname],[ip],[regionid],[updatetime]";

        /// <summary>
        /// 订单处理表
        /// </summary>
        public const string ORDER_ACTIONS = "[aid],[oid],[uid],[realname],[actiontype],[actiontime],[actiondes]";

        /// <summary>
        /// 订单退款表
        /// </summary>
        public const string ORDER_REFUNDS = "[refundid],[storeid],[storename],[oid],[osn],[uid],[state],[applytime],[paymoney],[refundmoney],[refundsn],[refundsystemname],[refundfriendname],[refundtime],[paysn],[paysystemname],[payfriendname]";

        /// <summary>
        /// 订单表
        /// </summary>
        public const string ORDERS = "[oid],[osn],[uid],[orderstate],[productamount],[orderamount],[surplusmoney],[parentid],[isreview],[addtime],[storeid],[storename],[shipsn],[shipcoid],[shipconame],[shiptime],[paysn],[paysystemname],[payfriendname],[paymode],[paytime],[regionid],[consignee],[mobile],[phone],[email],[zipcode],[address],[besttime],[shipfee],[payfee],[fullcut],[discount],[paycreditcount],[paycreditmoney],[couponmoney],[weight],[buyerremark],[ip]";

        /// <summary>
        /// 商品属性表
        /// </summary>
        public const string PRODUCT_ATTRIBUTES = "[recordid],[pid],[attrid],[attrvalueid],[inputvalue]";

        /// <summary>
        /// 商品咨询表
        /// </summary>
        public const string PRODUCT_CONSULTS = "[consultid],[pid],[consulttypeid],[state],[consultuid],[replyuid],[storeid],[consulttime],[replytime],[consultmessage],[replymessage],[consultnickname],[replynickname],[pname],[pshowimg],[consultip],[replyip]";

        /// <summary>
        /// 商品咨询类型表
        /// </summary>
        public const string PRODUCT_CONSULTTYPES = "[consulttypeid],[title],[displayorder]";

        /// <summary>
        /// 商品图片表
        /// </summary>
        public const string PRODUCT_IMAGES = "[pimgid],[pid],[showimg],[ismain],[displayorder],[storeid]";

        /// <summary>
        /// 商品评价质量表
        /// </summary>
        public const string PRODUCT_REVIEWQUALITY = "[id],[reviewid],[uid],[votetime]";

        /// <summary>
        /// 商品评价表
        /// </summary>
        public const string PRODUCT_REVIEWS = "[reviewid],[pid],[uid],[oprid],[oid],[parentid],[state],[storeid],[star],[quality],[message],[reviewtime],[paycredits],[pname],[pshowimg],[buytime],[ip]";

        /// <summary>
        /// 商品表
        /// </summary>
        public const string PRODUCTS = "[pid],[psn],[cateid],[brandid],[storeid],[storecid],[storestid],[skugid],[name],[shopprice],[marketprice],[costprice],[state],[isbest],[ishot],[isnew],[displayorder],[weight],[showimg],[salecount],[visitcount],[reviewcount],[star1],[star2],[star3],[star4],[star5],[addtime],[description]";

        /// <summary>
        /// 商品部分表
        /// </summary>
        public const string PART_PRODUCTS = "[pid],[psn],[cateid],[brandid],[storeid],[storecid],[storestid],[skugid],[name],[shopprice],[marketprice],[costprice],[state],[isbest],[ishot],[isnew],[displayorder],[weight],[showimg],[salecount],[visitcount],[reviewcount],[star1],[star2],[star3],[star4],[star5],[addtime]";

        /// <summary>
        /// 商品SKU表
        /// </summary>
        public const string PRODUCT_SKUS = "[recordid],[skugid],[pid],[attrid],[attrvalueid],[inputvalue]";

        /// <summary>
        /// 商品统计表
        /// </summary>
        public const string PRODUCT_STATS = "[recordid],[pid],[category],[value],[count]";

        /// <summary>
        /// 商品库存表
        /// </summary>
        public const string PRODUCT_STOCKS = "[pid],[number],[limit]";

        /// <summary>
        /// PV统计表
        /// </summary>
        public const string PVSTATS = "[recordid],[storeid],[category],[value],[count]";

        /// <summary>
        /// 全国行政区域表
        /// </summary>
        public const string REGIONS = "[regionid],[name],[spell],[shortspell],[displayorder],[parentid],[layer],[provinceid],[provincename],[cityid],[cityname]";

        /// <summary>
        /// 关联商品表
        /// </summary>
        public const string RELATEPRODUCTS = "[recordid],[pid],[relatepid]";

        /// <summary>
        /// 搜索历史表
        /// </summary>
        public const string SEARCHHISTORIES = "[recordid],[uid],[keyword],[times],[updatetime]";

        /// <summary>
        /// 用户配送地址表
        /// </summary>
        public const string SHIPADDRESSES = "[said],[uid],[regionid],[isdefault],[consignee],[mobile],[phone],[email],[zipcode],[address]";

        /// <summary>
        /// 配送公司表
        /// </summary>
        public const string SHIPCOMPANIES = "[shipcoid],[name],[displayorder]";

        /// <summary>
        /// 单品促销表
        /// </summary>
        public const string SINGLE_PROMOTIONS = "[pmid],[pid],[storeid],[starttime1],[endtime1],[starttime2],[endtime2],[starttime3],[endtime3],[userranklower],[state],[name],[slogan],[discounttype],[discountvalue],[coupontypeid],[paycredits],[isstock],[stock],[quotalower],[quotaupper],[allowbuycount]";

        /// <summary>
        /// 店铺管理日志表
        /// </summary>
        public const string STORE_ADMINLOGS = "[logid],[uid],[nickname],[storeid],[storename],[operation],[description],[ip],[operatetime]";

        /// <summary>
        /// 店铺分类表
        /// </summary>
        public const string STORE_CLASSES = "[storecid],[storeid],[displayorder],[name],[parentid],[layer],[haschild],[path]";

        /// <summary>
        /// 店铺行业表
        /// </summary>
        public const string STORE_INDUSTRIES = "[storeiid],[title],[displayorder]";

        /// <summary>
        /// 店长表
        /// </summary>
        public const string STORE_KEEPERS = "[storeid],[type],[name],[idcard],[address]";

        /// <summary>
        /// 店铺等级表
        /// </summary>
        public const string STORE_RANKS = "[storerid],[title],[avatar],[honestieslower],[honestiesupper],[productcount]";

        /// <summary>
        /// 店铺评价表
        /// </summary>
        public const string STORE_REVIEWS = "[reviewid],[oid],[storeid],[descriptionstar],[servicestar],[shipstar],[uid],[reviewtime],[ip]";

        /// <summary>
        /// 店铺表
        /// </summary>
        public const string STORES = "[storeid],[state],[name],[regionid],[storerid],[storeiid],[logo],[createtime],[mobile],[phone],[qq],[ww],[depoint],[sepoint],[shpoint],[honesties],[stateendtime],[theme],[banner],[announcement],[description]";

        /// <summary>
        /// 店铺配送费用表
        /// </summary>
        public const string STORE_SHIPFEES = "[recordid],[storestid],[regionid],[startvalue],[startfee],[addvalue],[addfee]";

        /// <summary>
        /// 店铺配送模板表
        /// </summary>
        public const string STORE_SHIPTEMPLATES = "[storestid],[storeid],[free],[type],[title]";

        /// <summary>
        /// 套装商品表
        /// </summary>
        public const string SUIT_PRODUCTS = "[recordid],[pmid],[pid],[discount],[number]";

        /// <summary>
        /// 套装促销活动表
        /// </summary>
        public const string SUIT_PROMOTIONS = "[pmid],[storeid],[starttime1],[endtime1],[starttime2],[endtime2],[starttime3],[endtime3],[userranklower],[state],[name],[quotaupper],[onlyonce]";

        /// <summary>
        /// 定时商品表
        /// </summary>
        public const string TIMEPRODUCTS = "[recordid],[pid],[storeid],[onsalestate],[outsalestate],[onsaletime],[outsaletime]";

        /// <summary>
        /// 活动专题表
        /// </summary>
        public const string TOPICS = "[topicid],[starttime],[endtime],[isshow],[sn],[title],[headhtml],[bodyhtml]";

        /// <summary>
        /// 部分用户表
        /// </summary>
        public const string PARTUSERS = "[uid],[username],[email],[mobile],[password],[userrid],[storeid],[mallagid],[nickname],[avatar],[paycredits],[rankcredits],[verifyemail],[verifymobile],[liftbantime],[salt]";

        /// <summary>
        /// 用户细节表
        /// </summary>
        public const string USERDETAILS = "[uid],[lastvisittime],[lastvisitip],[lastvisitrgid],[registertime],[registerip],[registerrgid],[gender],[realname],[bday],[idcard],[regionid],[address],[bio]";

        /// <summary>
        /// 用户等级表
        /// </summary>
        public const string USER_RANKS = "[userrid],[system],[title],[avatar],[creditslower],[creditsupper],[limitdays]";
    }
}
