using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class PlaceOrder
    {
        public int userId { get; set; }
        public Dictionary<string, int> goods { get; set; }

        public string address { get; set; }
    }
}
