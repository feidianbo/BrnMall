using System;

namespace BrnMall.Core
{
    /// <summary>
    /// 订单处理类型
    /// </summary>
    public enum OrderActionType
    {
        /// <summary>
        /// 提交
        /// </summary>
        Submit = 0,
        /// <summary>
        /// 支付
        /// </summary>
        Pay = 1,
        /// <summary>
        /// 确认
        /// </summary>
        Confirm = 2,
        /// <summary>
        /// 备货
        /// </summary>
        PreProduct = 3,
        /// <summary>
        /// 发货
        /// </summary>
        Send = 4,
        /// <summary>
        /// 完成
        /// </summary>
        Complete = 5,
        /// <summary>
        /// 退货
        /// </summary>
        Return = 6,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 7,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel = 8,
        /// <summary>
        /// 更新配送费用
        /// </summary>
        UpdateShipFee = 101,
        /// <summary>
        /// 更新折扣
        /// </summary>
        UpdateDiscount = 102
    }
}
