using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        public int Uid { get; set; }
        public DateTime PayDate { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public DateTime? LogTime { get; set; }
    }
}
