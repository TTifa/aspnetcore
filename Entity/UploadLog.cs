using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class UploadLog
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string URL { get; set; }
        public DateTime LogTime { get; set; }
        public string Remark { get; set; }
    }
}
