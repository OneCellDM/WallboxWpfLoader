using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WallBox
{
    public static  class WallBoxApi
    {
       

        public static async Task<string> GetImageUrlAsync(DataModel.ImageModel imageModel)
        {

           return "https://wallbox.ru"+ Parser.DownloadUrlParser(
                await Tools.Request("https://wallbox.ru" + Parser.LoadPageParser(
                            await Tools.Request("https://wallbox.ru" + imageModel.LoadPageUrl)
                    )
                )
              );
            
        }
        public static async Task<(List<DataModel.ImageModel>, List<int>)> GetCategoryPageData(string Url)
        {
            var url = Url.Replace("?amp=1","");
            if (Url.Contains("wallbox.ru"))
                return Parser.PageParser(await Tools.Request(url));

            return Parser.PageParser(await Tools.Request("https://wallbox.ru/" + url));
        }
        public static async Task<(List<DataModel.ImageModel>, List<int>)> GetCategoryPageData(string categoryUrl,int page)=>
              await GetCategoryPageData(categoryUrl + "/page-" + page);
        /// <summary>
        /// Старый метод который не будет использован
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        /*
        public static async  Task <List<DataModel.ImageModel>> GetPreviews(String Url)
        {
           
            if (Url.Contains("wallbox.ru"))
              return Parser.PreviewParser(await Tools.Request(Url));
                  
            return Parser.PreviewParser(await Tools.Request("https://wallbox.ru/"+Url));
                    
        }
        */
        public static async Task<List<DataModel.CategoryModel>> GetCategoriesAsync()=>
                    Parser.CategoriesParser(await Tools.Request("https://wallbox.ru"));
          
    }
}
