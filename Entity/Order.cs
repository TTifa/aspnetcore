using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
    public class Order
    {
        [Key]
        public string OrderId { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }
        /// <summary>
        /// 送达时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        public OrderStatus State { get; set; }
    }

    public enum OrderStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        created,
        /// <summary>
        /// 已付款
        /// </summary>
        paid,
        /// <summary>
        /// 已发货
        /// </summary>
        deliveried,
        /// <summary>
        /// 已收货
        /// </summary>
        received,
        /// <summary>
        /// 完成
        /// </summary>
        done
    }
}
