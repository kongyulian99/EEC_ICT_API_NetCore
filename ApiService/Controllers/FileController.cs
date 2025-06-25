using ApiService.Business;
using ApiService.Common;
using ApiService.Core;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private AuthService _authSv;
        private readonly IMemoryCache _memoryCache;
        //private readonly MinioService _minioService;

        public FileController(
            //DI default service
            ILogger<FileController> logger
            , AppSetting appSettingInfo
            //DI services
            , AuthService authSv
            , IMemoryCache memoryCache
         //, MinioService minioService
         )
        {
            _authSv = authSv;
            _memoryCache = memoryCache;
            //_minioService = minioService;
        }

        [HttpPost]
        [Route("ckeditor-images")]
        public async Task<object> ImageUpload()
        {
            try
            {
                var file = Request.Form.Files[0];

                // Tạo thư mục lưu trữ ảnh
                string folderPath = Path.Combine("Uploads", "Images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                int total;
                try
                {
                    total = Request.Form.Files.Count;
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new { error = new { message = "Lỗi upload!" } });
                }
                if (total == 0)
                {
                    return await Task.FromResult(new { error = new { message = "Không tồn tại file tải lên!" } });
                }
                string fileName = file.FileName;
                if (fileName == "")
                {
                    return await Task.FromResult(new { error = new { message = "Không tồn tại file tải lên!" } });
                }
                
                // Tạo tên file mới để tránh trùng lặp
                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                string newPath = Path.Combine(folderPath, newFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo URL tương đối cho CKEditor sử dụng
                // URL sẽ trỏ đến endpoint get-image
                string relativeUrl = $"/api/File/get-image/{newFileName}";
                
                // Trả về url cho CKEditor
                return await Task.FromResult(new { url = relativeUrl });
            }
            catch (Exception exception)
            {
                return await Task.FromResult(new { error = new { message = exception.Message } });
            }
        }

        //[HttpPost("upload")]
        //public async Task<ReturnBaseInfo<object>> UploadFile(string folder)
        //{
        //    var retval = new ReturnBaseInfo<object>();
        //    retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
        //    retval.ReturnData = null;

        //    try
        //    {
        //        string path = Path.Combine("Uploads", "Documents", folder);
        //        // create document and images folders in Upload folder
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        var file = Request.Form.Files[0];
        //        if (file == null || file.Length == 0)
        //        {
        //            retval.ReturnStatus.Message = "Chưa chọn file. Vui lòng kiểm tra lại";
        //            retval.ReturnStatus.Code = 0;
        //        }

        //        var uniqueFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Util.GeneateCharacterRandomString(4) + Path.GetExtension(file.FileName);
        //        // Tạo đường dẫn đầy đủ để lưu file
        //        var filePath = Path.Combine(path, uniqueFileName);

        //        // Ghi file lên server
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        retval.ReturnStatus.Message = "Thành công";
        //        retval.ReturnStatus.Code = 0;
        //        retval.ReturnData = uniqueFileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        retval.ReturnStatus.Message = ex.Message;
        //        retval.ReturnStatus.Code = -1;
        //    }

        //    return retval;
        //}

        //[HttpPost("upload-absolute-path")]
        //public async Task<ReturnBaseInfo<object>> UploadFileAndReturnAbsolutePath(string folder)
        //{
        //    var retval = new ReturnBaseInfo<object>();
        //    retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
        //    retval.ReturnData = null;

        //    try
        //    {
        //        string path = Path.Combine("Uploads", "Documents", folder);
        //        // create document and images folders in Upload folder
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        var file = Request.Form.Files[0];
        //        if (file == null || file.Length == 0)
        //        {
        //            retval.ReturnStatus.Message = "Chưa chọn file. Vui lòng kiểm tra lại";
        //            retval.ReturnStatus.Code = 0;
        //        }

        //        var uniqueFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Util.GeneateCharacterRandomString(4) + Path.GetExtension(file.FileName);
        //        // Tạo đường dẫn đầy đủ để lưu file
        //        var filePath = Path.Combine(path, uniqueFileName);

        //        // Ghi file lên server
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        retval.ReturnStatus.Message = "Thành công";
        //        retval.ReturnStatus.Code = 0;
        //        retval.ReturnData = new { filePath, file.FileName };
        //    }
        //    catch (Exception ex)
        //    {
        //        retval.ReturnStatus.Message = ex.Message;
        //        retval.ReturnStatus.Code = -1;
        //    }

        //    return retval;
        //}

        //[HttpPost]
        //[Route("uploadchunk")]
        //public async Task<ReturnBaseInfo<object>> UploadChunkAsync(string folder, string fileType = "NOT_VIDEO")
        //{
        //    var retval = new ReturnBaseInfo<object>();
        //    retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
        //    retval.ReturnData = null;
        //    FileStream outputStream = null;

        //    // create document and images folders in Upload folder
        //    string path = Path.Combine("Uploads", "Documents", folder);
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    try
        //    {
        //        string fileId = Request.Form["fileId"];
        //        string nameFile = Path.GetFileNameWithoutExtension(fileId);
        //        string fileExtension = Path.GetExtension(fileId);

        //        int chunkNumber = Convert.ToInt32(Request.Form["chunkNumber"]);
        //        int totalChunks = Convert.ToInt32(Request.Form["totalChunks"]);
        //        var chunk = Request.Form.Files["chunk"];

        //        string tempPath = Path.Combine(path, nameFile);
        //        Directory.CreateDirectory(tempPath);
        //        string chunkFilePath = Path.Combine(tempPath, chunkNumber.ToString());

        //        using (var stream = new FileStream(chunkFilePath, FileMode.Create))
        //        {
        //            await chunk.CopyToAsync(stream);
        //        }

        //        if (Directory.GetFiles(tempPath).Length == totalChunks)
        //        {
        //            string fileNameWoExtension = Guid.NewGuid().ToString();
        //            string finalFileName = fileNameWoExtension + fileExtension;
        //            string finalFilePath = Path.Combine(path, fileNameWoExtension, finalFileName);
        //            if (!Directory.Exists(Path.Combine(path, fileNameWoExtension)))
        //            {
        //                Directory.CreateDirectory(Path.Combine(path, fileNameWoExtension));
        //            }

        //            try
        //            {
        //                outputStream = new FileStream(finalFilePath, FileMode.Create);

        //                for (int i = 0; i < totalChunks; i++)
        //                {
        //                    chunkFilePath = Path.Combine(tempPath, i.ToString());
        //                    FileStream readChunkStream = null;

        //                    try
        //                    {
        //                        readChunkStream = new FileStream(chunkFilePath, FileMode.Open);
        //                        readChunkStream.CopyTo(outputStream);
        //                    }
        //                    finally
        //                    {
        //                        if (readChunkStream != null)
        //                        {
        //                            readChunkStream.Close();
        //                            readChunkStream.Dispose();
        //                        }
        //                    }

        //                    // Xóa chunk file sau khi đã đọc
        //                    try
        //                    {
        //                        System.IO.File.Delete(chunkFilePath);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // Log lỗi nếu không xóa được chunk file
        //                        Console.WriteLine($"Không thể xóa chunk file: {ex.Message}");
        //                    }
        //                }
        //            }
        //            finally
        //            {
        //                if (outputStream != null)
        //                {
        //                    outputStream.Flush();
        //                    outputStream.Close();
        //                    outputStream.Dispose();
        //                }
        //            }

        //            retval.ReturnData = finalFileName;

        //            try
        //            {
        //                // Xóa thư mục tạm
        //                Directory.Delete(tempPath, true);
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log lỗi nếu không xóa được thư mục tạm
        //                Console.WriteLine($"Không thể xóa thư mục tạm: {ex.Message}");
        //            }

        //            //// convert to HLS                    
        //            //if (fileType == "VIDEO")
        //            //{
        //            //    // Thực hiện convert video trong một task riêng biệt
        //            //    _ = Task.Run(async () =>
        //            //    {
        //            //        try
        //            //        {
        //            //            string outputDirectory = Path.Combine(path, fileNameWoExtension, "hls");
        //            //            if (!Directory.Exists(outputDirectory))
        //            //                Directory.CreateDirectory(outputDirectory);

        //            //            FFmpeg.SetExecutablesPath(Path.Combine("ffmpeg", "bin"));

        //            //            var outputPath = Path.Combine(outputDirectory, fileNameWoExtension);
        //            //            var arguments = $"-i \"{finalFilePath}\" -c:v libx264 -b:v {ConfigurationHelper.ffmpegBitrate} -hls_time {ConfigurationHelper.segmentTime} -hls_list_size 0 -hls_segment_filename \"{outputPath}%04d.ts\" \"{outputPath}.m3u8\"";

        //            //            var conversion = FFmpeg.Conversions.New()
        //            //                .AddParameter(arguments, ParameterPosition.PreInput);

        //            //            await conversion.Start();

        //            //            // Giải phóng tài nguyên sau khi convert xong
        //            //            GC.Collect();
        //            //        }
        //            //        catch (Exception ex)
        //            //        {
        //            //            // Log lỗi convert video
        //            //            Console.WriteLine($"Lỗi convert video: {ex.Message}");
        //            //        }
        //            //    });
        //            //}
        //        }

        //        retval.ReturnStatus.Message = "Thành công";
        //        retval.ReturnStatus.Code = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        retval.ReturnStatus.Message = ex.Message;
        //        retval.ReturnStatus.Code = -1;
        //    }
        //    finally
        //    {
        //        // Đảm bảo giải phóng bộ nhớ sau khi xử lý xong
        //        GC.Collect();
        //    }

        //    return retval;
        //}

        //[HttpPost]
        //[Route("convert-video")]
        //public Task<ReturnBaseInfo<object>> ConvertVideo(string folder, string fileName, Guid videoId)
        //{
        //    var retval = new ReturnBaseInfo<object>();
        //    retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
        //    retval.ReturnData = null;

        //    // create document and images folders in Upload folder
        //    string path = Path.Combine("Uploads", "Documents", folder);
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    try
        //    {
        //        // Chuyển đổi video trong một task riêng biệt để không chặn request
        //        _ = Task.Run(async () =>
        //        {
        //            try
        //            {
        //                string fileNameWoExtension = Path.GetFileNameWithoutExtension(fileName);
        //                string outputDirectory = Path.Combine(path, fileNameWoExtension, "hls");
        //                string filePath = Path.Combine(path, fileNameWoExtension, fileName);

        //                if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

        //                FFmpeg.SetExecutablesPath(Path.Combine("ffmpeg", "bin"));

        //                var outputPath = Path.Combine(outputDirectory, fileNameWoExtension);
        //                var arguments = $"-i \"{filePath}\" -c:v libx264 -b:v {ConfigurationHelper.ffmpegBitrate} -hls_time {ConfigurationHelper.segmentTime} -hls_list_size 0 -hls_segment_filename \"{outputPath}%04d.ts\" \"{outputPath}.m3u8\"";

        //                using (var cts = new System.Threading.CancellationTokenSource())
        //                {
        //                    try
        //                    {
        //                        var conversion = FFmpeg.Conversions.New()
        //                            .AddParameter(arguments, ParameterPosition.PreInput);

        //                        await conversion.Start();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Lỗi khi chuyển đổi video: {ex.Message}");
        //                    }
        //                }

        //                // cập nhật trạng thái video
        //                var videoInfo = ServiceFactory.QLTrangTin_DuLieu_Video.GetInfo(videoId).Result;
        //                if (videoInfo != null)
        //                {
        //                    videoInfo.DangXuLy = false;
        //                    _ = ServiceFactory.QLTrangTin_DuLieu_Video.Update(videoInfo);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Cập nhật trạng thái video khi có lỗi
        //                try
        //                {
        //                    var videoInfo = ServiceFactory.QLTrangTin_DuLieu_Video.GetInfo(videoId).Result;
        //                    if (videoInfo != null)
        //                    {
        //                        videoInfo.DangXuLy = false;
        //                        _ = ServiceFactory.QLTrangTin_DuLieu_Video.Update(videoInfo);
        //                    }
        //                }
        //                catch { }

        //                // Log lỗi
        //                Console.WriteLine($"Lỗi convert video: {ex.Message}");
        //            }
        //            finally
        //            {
        //                // Giải phóng bộ nhớ sau khi xử lý
        //                GC.Collect();
        //            }
        //        });

        //        retval.ReturnStatus.Message = "Thành công";
        //        retval.ReturnStatus.Code = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        retval.ReturnStatus.Message = ex.Message;
        //        retval.ReturnStatus.Code = -1;
        //    }

        //    return Task.FromResult(retval);
        //}

        ////[HttpPost]
        ////[Route("uploadchunk")]
        ////public ReturnBaseInfo<object> UploadChunk(string folder)
        ////{
        ////    var retval = new ReturnBaseInfo<object>();
        ////    retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
        ////    retval.ReturnData = null;

        ////    // create document and images folders in Upload folder

        ////    string path = Path.Combine("Uploads", "Documents", folder);
        ////    // create document and images folders in Upload folder
        ////    if (!Directory.Exists(path))
        ////    {
        ////        Directory.CreateDirectory(path);
        ////    }

        ////    try
        ////    {

        ////        string fileId = Request.Form["fileId"];

        ////        string nameFile = Path.GetFileNameWithoutExtension(fileId);
        ////        string extensionFile = Path.GetExtension(fileId);

        ////        int chunkNumber = Convert.ToInt32(Request.Form["chunkNumber"]);
        ////        int totalChunks = Convert.ToInt32(Request.Form["totalChunks"]);
        ////        var chunk = Request.Form.Files["chunk"];

        ////        string tempPath = Path.Combine(path, nameFile);
        ////        Directory.CreateDirectory(tempPath);
        ////        string chunkFilePath = Path.Combine(tempPath, chunkNumber.ToString());

        ////        using (var stream = new FileStream(chunkFilePath, FileMode.Create))
        ////        {
        ////            chunk.CopyTo(stream);
        ////        }

        ////        string finalFilePath = Path.Combine(path, nameFile + DateTime.Now.ToString("yyyyMMddhhmmss") + extensionFile);
        ////        if (chunkNumber == totalChunks - 1)
        ////        {
        ////            using (var outputStream = new FileStream(finalFilePath, FileMode.Create))
        ////            {
        ////                for (int i = 0; i < totalChunks; i++)
        ////                {
        ////                    chunkFilePath = Path.Combine(tempPath, i.ToString());
        ////                    using (var readChunkStream = new FileStream(chunkFilePath, FileMode.Open))
        ////                    {
        ////                        readChunkStream.CopyTo(outputStream);
        ////                    }
        ////                }
        ////            }
        ////            Directory.Delete(tempPath, true);
        ////        }

        ////        retval.ReturnData = finalFilePath;
        ////        retval.ReturnStatus.Message = "Thành công";
        ////        retval.ReturnStatus.Code = 0;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        retval.ReturnStatus.Message = ex.Message;
        ////        retval.ReturnStatus.Code = -1;
        ////    }

        ////    return retval;
        ////}

        ////[HttpGet]
        ////[Route("video")]
        ////public IActionResult PlayVideo(string folder, string fileName)
        ////{
        ////    //Build the File Path.  
        ////    string path = Path.Combine("Uploads", "Documents", folder, fileName);  // the video file is in the wwwroot/files folder

        ////    if (!System.IO.File.Exists(path))
        ////    {
        ////        return NotFound();
        ////    }

        ////    //var filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        ////    //return File(filestream, "application/octet-stream", true);
        ////    using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        ////    {
        ////        fileStream.Seek(rangeStart, SeekOrigin.Begin);
        ////        await fileStream.CopyToAsync(response.Body);
        ////    }
        ////    return new EmptyResult();
        ////}

        //[HttpGet("video")]
        //public async Task<IActionResult> GetVideo(string folder, string fileName)
        //{
        //    //Build the File Path.  
        //    string path = Path.Combine("Uploads", "Documents", folder, fileName);  // the video file is in the wwwroot/files folder

        //    // Kiểm tra tồn tại file
        //    if (!System.IO.File.Exists(path))
        //    {
        //        return NotFound();
        //    }

        //    var file = new FileInfo(path);
        //    var fileLength = file.Length;

        //    var range = Request.Headers["Range"].ToString();
        //    var response = HttpContext.Response;
        //    FileStream fileStream = null;

        //    try
        //    {
        //        if (range != null)
        //        {
        //            var rangeStart = long.Parse(range.Replace("bytes=", "").Split('-')[0]);
        //            var rangeEnd = Math.Min(rangeStart + 1024 * 1024 - 1, fileLength - 1); // 1 MB chunk
        //            response.Headers.Add("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{fileLength}");
        //            response.StatusCode = 206;
        //            response.ContentLength = rangeEnd - rangeStart + 1;

        //            try
        //            {
        //                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //                fileStream.Seek(rangeStart, SeekOrigin.Begin);
        //                await fileStream.CopyToAsync(response.Body);
        //            }
        //            finally
        //            {
        //                if (fileStream != null)
        //                {
        //                    fileStream.Close();
        //                    fileStream.Dispose();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            response.ContentType = "video/*";
        //            response.ContentLength = fileLength;

        //            try
        //            {
        //                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //                await fileStream.CopyToAsync(response.Body);
        //            }
        //            finally
        //            {
        //                if (fileStream != null)
        //                {
        //                    fileStream.Close();
        //                    fileStream.Dispose();
        //                }
        //            }
        //        }

        //        return new EmptyResult();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log lỗi 
        //        Console.WriteLine($"Lỗi streaming video: {ex.Message}");
        //        return StatusCode(500, "Lỗi khi xử lý video");
        //    }
        //    finally
        //    {
        //        // Đảm bảo giải phóng bộ nhớ
        //        GC.Collect();
        //    }
        //}

        //[HttpGet]
        //[Route("downloadreport")]
        //public async Task<IActionResult> DownloadReport(string fileName)
        //{
        //    //var fileName = Path.GetFileName();
        //    var pathToFile = Path.Combine("Reports", fileName);
        //    var content = await System.IO.File.ReadAllBytesAsync(pathToFile);


        //    if (!new FileExtensionContentTypeProvider().TryGetContentType(pathToFile,
        //    out var ContentType))
        //    {
        //        ContentType = "application/octet-stream";
        //    }

        //    return File(content, ContentType, fileName);
        //}

        //// use Minio
        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is empty");

        //    using var stream = file.OpenReadStream();
        //    await _minioService.UploadFileAsync(file.FileName, stream, file.Length, file.ContentType);

        //    return Ok("Uploaded successfully");
        //}

        [HttpGet]
        [Route("get-image/{*imagePath}")]
        public IActionResult GetImage(string imagePath)
        {
            try
            {
                // Xử lý đường dẫn để đảm bảo an toàn
                if (string.IsNullOrEmpty(imagePath))
                {
                    return NotFound();
                }

                // Ngăn chặn path traversal attacks
                imagePath = imagePath.Replace("..", "");
                
                // Tìm kiếm file trong thư mục Uploads và Uploads/Images
                string filePath;
                if (System.IO.File.Exists(Path.Combine("Uploads", "Images", imagePath)))
                {
                    filePath = Path.Combine("Uploads", "Images", imagePath);
                }
                else if (System.IO.File.Exists(Path.Combine("Uploads", imagePath)))
                {
                    filePath = Path.Combine("Uploads", imagePath);
                }
                else
                {
                    return NotFound();
                }

                // Xác định loại nội dung (MIME type)
                string contentType;
                if (!new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                // Đọc file và trả về
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}