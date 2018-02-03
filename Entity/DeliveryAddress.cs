using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class DeliveryAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AreaCode { get; set; }
        public string AreaText { get; set; }
        public string Detail { get; set; }
        public string Contact { get; set; }
        public string Telphone { get; set; }
        public DateTime CreateTime { get; set; }
        public AddressState State { get; set; }
        public string Remark { get; set; }
    }

    public enum AddressState
    {
        normal,
        isDefault
    }
}
