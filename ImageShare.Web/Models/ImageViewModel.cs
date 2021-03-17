using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageShare.Data;

namespace ImageShare.Web.Models
{
    public class ImageViewModel
    {
    }
    public class ImageUploadedViewModel
    {
        public int ImageId { get; set; }
        public string Password { get; set; }
    }
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool PasswordEntered { get; set; }
        public bool FalsePassword { get; set; }
    }
}
