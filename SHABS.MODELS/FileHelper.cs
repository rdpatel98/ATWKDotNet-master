using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHABS.MODELS
{
    public class FileHelper
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}