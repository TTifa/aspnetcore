using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int State { get; set; }
        public int AreaLevel { get; set; }
        public string Alpha { get; set; }
        public int OrderNo { get; set; }
    }
}
