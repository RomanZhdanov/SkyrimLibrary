using HtmlAgilityPack;

namespace SkyrimLibrary.WebAPI.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToStripHtml(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(text);

            return htmlDoc.DocumentNode.InnerText;
        }

        public static string RemoveLinks(this string html, string picturesUrl)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection anchorNodes = document.DocumentNode.SelectNodes("//a");
            if (anchorNodes != null)
            {
                foreach (HtmlNode anchorNode in anchorNodes)
                {
                    string innerText = anchorNode.InnerHtml; // Extract inner text
                    anchorNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(innerText), anchorNode); // Replace anchor tag with inner text
                }
            }

            HtmlNodeCollection imageNodes = document.DocumentNode.SelectNodes("//img");
            if (imageNodes != null)
            {
                foreach (HtmlNode iamgeNode in imageNodes)
                {
                    //string innerText = iamgeNode.GetAttributeValue("title", ""); // Extract inner text
                    //if (!string.IsNullOrEmpty(innerText))
                    //{
                    //    iamgeNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(innerText), iamgeNode); // Replace anchor tag with inner text
                    //}
                    string picture = iamgeNode.GetAttributeValue("src", "");
                    iamgeNode.SetAttributeValue("src", picturesUrl + picture);
                }
            }

            return document.DocumentNode.OuterHtml;
        }
    }
}
