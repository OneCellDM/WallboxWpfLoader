using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
namespace WallBox
{
    public static  class Parser
    {
       public static string DownloadUrlParser(string Data)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Data);
            return   htmlDocument.GetElementbyId("image-full").Attributes["src"].Value;
           
        }

        public static String LoadPageParser(string Data)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Data);
            var node=htmlDocument.DocumentNode.SelectSingleNode("//a[@class='ca_download']");
            return node.Attributes["href"].Value;
        }
        public static List<int> PageNumbersParser(string Data)
        {
            List<int> pages = new List<int>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Data);
            var paginationLiList = htmlDocument.DocumentNode.SelectSingleNode("//ul[@class='pagination']").SelectNodes("li");

            for (int i = 1; i < paginationLiList.Count - 1; i++)
            {
                try
                {
                    pages.Add(int.Parse(paginationLiList[i].InnerText));
                }
                catch (Exception EX) { }

            }
            return pages;
        }
            
      
        public static (List<DataModel.ImageModel>,List<int>) PageParser(string Data)
        {
            var TupleData = (new List<DataModel.ImageModel>(), new List<int>());
            TupleData.Item1.AddRange(PreviewParser(Data));
            TupleData.Item2.AddRange(PageNumbersParser(Data));
            return TupleData;

        }
       
        public static List<DataModel.ImageModel> PreviewParser(string Data)
        {
            List<DataModel.ImageModel> imageModels = new List<DataModel.ImageModel>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Data);
            try
            {
                
                
                var  divD = htmlDocument.DocumentNode.SelectNodes("//div[@class='col-md-4 category-list__item']");

                if (divD!=null)
                foreach (var ampnode in divD)
                {
                    try
                    {
                            if (ampnode.FirstChild.Attributes.Count>0)
                            {
                                string url = "";
                                string alt = "";
                                string Date = "";
                               Date= ampnode.SelectSingleNode("div[@class='row']").LastChild.InnerText.Trim().Replace("\n","").Replace("\r","");
                                if (ampnode.FirstChild.SelectSingleNode("img") != null)
                                {
                                    url = ampnode.FirstChild.SelectSingleNode("img").Attributes["src"].Value;
                                    alt = ampnode.FirstChild.SelectSingleNode("img").Attributes["alt"].Value;
                                    
                                }
                                if(ampnode.FirstChild.SelectSingleNode("amp-img")!=null)
                                {
                                    url = ampnode.FirstChild.SelectSingleNode("amp-img").Attributes["src"].Value;
                                    alt = ampnode.FirstChild.SelectSingleNode("amp-img").Attributes["alt"].Value;
                                }
                                if(url!=null)
                                imageModels.Add(new DataModel.ImageModel()
                                {
                                    LoadPageUrl = ampnode.FirstChild.Attributes["href"].Value,

                                    Url = url,
                                    Alt = alt,
                                    Date=Date,
                                }
                                ); ;
                            }
                    }
                    catch (Exception EX) { }
                }
                if (imageModels[0].Url.Contains("yandex.ru"))
                    imageModels.RemoveAt(0);
            }
            catch(Exception EX) { }
            return imageModels;

            
        }
        public static List<DataModel.CategoryModel> CategoriesParser(String Data)
        {
            List<DataModel.CategoryModel> categoryModels = new List<DataModel.CategoryModel>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Data);
            var categoriesLiNodes = htmlDocument.GetElementbyId("w5").SelectNodes("li");
            foreach (var categoryData in categoriesLiNodes)

                categoryModels.Add(new DataModel.CategoryModel { 
                    Title = categoryData.FirstChild.InnerText,
                    Url = categoryData.FirstChild.Attributes["href"].Value
                });


            return categoryModels;
        }
    }
}
