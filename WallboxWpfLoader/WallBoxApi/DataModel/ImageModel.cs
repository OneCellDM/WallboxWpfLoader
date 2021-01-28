using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WallBox.DataModel
{
   public class ImageModel
    {
        public string LoadPageUrl { get; set; }
        
        public string Date { get; set; }
        public string Alt { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Url { get; set; }

       
    }
}
