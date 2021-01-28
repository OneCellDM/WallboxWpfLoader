using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using System.Diagnostics;

namespace WallBox
{
    public static  class Tools
    {
        public static  Task<String> Request(String url)
        {
            return  Task.Run(() =>
            {
                WebRequest webRequest = WebRequest.Create(url);

                return new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
            });
        }
      
       
       
    }
}
