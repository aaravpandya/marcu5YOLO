using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using marcu5yolo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace marcu5yolo.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private IHostingEnvironment _he;

        public HomeController(IHostingEnvironment HE)
        {
            _he = HE;
        }
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("[controller]/[action]")]
        public IActionResult imageUpload(FormViewModel viewModel)
        {
            var uploadedImage = viewModel.imageUpload;
            string filepath = null;
            if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
            {
                //    var root = he.WebRootPath;
                //    root = root + "\\SubmittedInitiativeImg";
                ////same file name problems
                //var filename = Path.Combine(he.WebRootPath, Path.GetFileName(uploadedImage.FileName));
                var name = Guid.NewGuid() + Path.GetFileName(uploadedImage.FileName);
                var filename = Path.Combine(_he.WebRootPath, name);

                uploadedImage.CopyTo(new FileStream(filename, FileMode.Create));
                filepath = name;
            }
            FileInfo fi = new FileInfo(filepath);
            string fileName = fi.Name;
            byte[] fileContents = System.IO.File.ReadAllBytes(Path.Combine(_he.WebRootPath, filepath));

            Uri webService = new Uri(@"https://marcu5yolo.azurewebsites.net/prediction/image");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;
            MultipartFormDataContent form = new MultipartFormDataContent
            {
                { new ByteArrayContent(fileContents, 0, fileContents.Length), "file", "pic.jpeg" }
            };

            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> result = client.PostAsync(webService.AbsoluteUri, form);
            //ViewBag._image = File(result.Result.Content.ReadAsByteArrayAsync().Result, result.Result.GetType().ToString());
            ViewBag.imageUrl = "data:" + result.Result.GetType().ToString()+"; base64," + Convert.ToBase64String(result.Result.Content.ReadAsByteArrayAsync().Result);
            return View();
        }

        
    }
}
