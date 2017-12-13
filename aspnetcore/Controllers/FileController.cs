using Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.Controllers
{
    public class FileController : Controller
    {
        private readonly UploadConfig _uploadConfig;
        private readonly IHostingEnvironment _env;

        public FileController(IOptions<UploadConfig> config, IHostingEnvironment env)
        {
            this._uploadConfig = config.Value;
            _env = env;
        }

        public ApiResult Get()
        {
            var pth = Path.Combine(_env.ContentRootPath, _uploadConfig.ImgDir);

            return new ApiResult(data: pth);
        }

        [HttpPost]
        [Route("api/File")]
        public async Task<ApiResult> Upload()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            //size > 10MB refuse upload !
            if (size > 10485760)
            {
                return new ApiResult(ApiStatus.Illegal, "files total size > 10MB , server refused!");
            }

            // full path to file in temp location
            //var filePath = Path.GetTempFileName();
            var filePath = Path.Combine(_env.ContentRootPath, _uploadConfig.ImgDir);
            foreach (var formFile in files)
            {
                filePath = Path.Combine(filePath, formFile.FileName);

                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return new ApiResult();
        }

        [HttpPost]
        [Route("api/File/Upload2Qiniu")]
        public async Task<ApiResult> Upload2Qiniu()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            if (size > 10485760)
            {
                return new ApiResult(ApiStatus.Illegal, "files total size > 10MB , server refused!");
            }

            var domain = _uploadConfig.qiniuDomain;
            var urls = new List<string>();
            var token = GetQiniuToken();
            var fu = new FormUploader();
            foreach (var file in files)
            {
                var filename = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
                var fileStream = file.OpenReadStream();
                byte[] byteData = new byte[fileStream.Length];
                fileStream.Read(byteData, 0, byteData.Length);
                var upResult = await fu.UploadDataAsync(byteData, filename, token);

                if (upResult.Code != 200)
                    return new ApiResult(ApiStatus.Fail, upResult.Text);

                urls.Add($"{domain}/{filename}");
            }

            return new ApiResult(data: urls);
        }

        private string GetQiniuToken(string filekey = "")
        {
            var ak = _uploadConfig.QiniuAK;
            var sk = _uploadConfig.QiniuSK;
            var bucket = _uploadConfig.QiniuBucket;
            var mac = new Mac(ak, sk);
            var putPolicy = new PutPolicy();
            // "覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + filekey;
            putPolicy.Scope = bucket;
            putPolicy.SetExpires(3600);
            //自动删除
            //putPolicy.DeleteAfterDays = 1;
            return Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        }
    }
}