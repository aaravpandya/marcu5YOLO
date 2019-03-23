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
            ViewBag.SelectedNav = "Home";
            return View();
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> imageUploadAsync(FormViewModel viewModel)
        {
            var uploadedImage = viewModel.imageUpload;
            var ms = new MemoryStream();
            await uploadedImage.CopyToAsync(ms);
            byte[] imageContents = ms.ToArray();
            //string filepath = null;
            //if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
            //{
            //    //    var root = he.WebRootPath;
            //    //    root = root + "\\SubmittedInitiativeImg";
            //    ////same file name problems
            //    //var filename = Path.Combine(he.WebRootPath, Path.GetFileName(uploadedImage.FileName));
            //    var name = Guid.NewGuid() + Path.GetFileName(uploadedImage.FileName);
            //    var filename = Path.Combine(_he.WebRootPath, name);

            //    uploadedImage.CopyTo(new FileStream(filename, FileMode.Create));
            //    filepath = name;
            //}
            //FileInfo fi = new FileInfo(filepath);
            //string fileName = fi.Name;
            //byte[] fileContents = await System.IO.File.ReadAllBytesAsync(Path.Combine(_he.WebRootPath, filepath));

            Uri webService = new Uri(@"https://marcu5yolo19.azurewebsites.net/prediction/image");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;
            MultipartFormDataContent form = new MultipartFormDataContent
            {
                { new ByteArrayContent(imageContents, 0, imageContents.Length), "file", "pic.jpeg" }
            };

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.PostAsync(webService.AbsoluteUri, form);
            byte[] imageResponse = await result.Content.ReadAsByteArrayAsync();
            //ViewBag._image = File(result.Result.Content.ReadAsByteArrayAsync().Result, result.Result.GetType().ToString());
            ViewBag.imageUrl = "data:image; base64," + Convert.ToBase64String(imageResponse);
            ViewBag.SelectedNav = "Home";
            return View();
        }

        [Route("[controller]/[action]")]
        public IActionResult About()
        {
            ViewBag.SelectedNav = "About";
            return View();
        }
    }
}
