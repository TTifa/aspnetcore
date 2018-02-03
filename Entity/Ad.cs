using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Ad
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Images { get; set; }
        public string LinkUrl { get; set; }
        public int OrderNo { get; set; }
        public DateTime CreateTime { get; set; }
        public string Match { get; set; }
        public AdState State { get; set; }
    }

    public enum AdState
    {
        disable,
        enable
    }
}
