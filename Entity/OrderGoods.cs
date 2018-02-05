using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class OrderGoods
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int GoodsId { get; set; }
        public int Quantity { get; set; }
        public string GoodsName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public string Avatar { get; set; }
    }
}
