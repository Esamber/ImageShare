using System;

namespace ImageShare.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
        public int Views { get; set; }
    }
}
