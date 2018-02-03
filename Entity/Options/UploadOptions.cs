using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity
{
    public class UploadOptions
    {
        /// <summary>
        /// 图片上传路径
        /// </summary>
        public string ImgDir { get; set; }

        public string QiniuAK { get; set; }
        public string QiniuSK { get; set; }
        public string QiniuBucket { get; set; }
        public string QiniuDomain { get; set; }
    }
}
