using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ImageShare.Data;
using ImageShare.Web.Models;
using Newtonsoft.Json;

namespace ImageShare.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            @"Data Source=.\sqlexpress;Initial Catalog=ImageShare;Integrated Security=true;";

        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile myfile, string password)
        {
            Guid guid = Guid.NewGuid();
            string actualFileName = $"{guid}-{myfile.FileName}";
            string finalFileName = Path.Combine(_environment.WebRootPath, "uploads", actualFileName);
            using var fs = new FileStream(finalFileName, FileMode.CreateNew);
            myfile.CopyTo(fs);

            ImageDb db = new(_connectionString);
            int newId = db.AddImage(actualFileName, password);
            ImageUploadedViewModel vm = new()
            {
                ImageId = newId,
                Password = password
            };

            return View(vm);
        }
        public IActionResult ViewImage(int imageId, bool falsePassword)
        {
            ImageDb db = new(_connectionString);
            var ids = HttpContext.Session.Get<List<int>>("Ids");
            Image image = db.GetImage(imageId);
            ViewImageViewModel vm = new()
            {
                Image = image,
                PasswordEntered = ids==null ? false : ids.Contains(imageId),
                FalsePassword = falsePassword
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult SubmitPassword(int imageId, string password)
        {
            ImageDb db = new(_connectionString);
            bool isCorrect = (password == db.GetImage(imageId).Password);
            if (isCorrect)
            {
                var ids = HttpContext.Session.Get<List<int>>("Ids");
                if (ids == null)
                {
                    HttpContext.Session.Set("Ids", new List<int>() { imageId });
                }
                else
                {
                    ids.Add(imageId);
                    HttpContext.Session.Set("Ids", ids);
                }
            }
            return Redirect($"/home/viewimage?imageId={imageId}$falsePassword=true");
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
