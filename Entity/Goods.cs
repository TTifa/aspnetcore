using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Goods
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Avatar { get; set; }
        public string Images { get; set; }
        public string Detail { get; set; }
        public int SalesVolume { get; set; }
        public decimal PraiseRate { get; set; }
        public bool Hot { get; set; }
        public int OrderNo { get; set; }
    }
}
