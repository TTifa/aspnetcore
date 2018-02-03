using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int ParentId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int OrderNo { get; set; }

    }
}
