using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class ShoppingCart
    {
        public int goodsId { get; set; }
        public string avatar { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public bool selected { get; set; }
        public int quantity { get; set; }
    }
}
