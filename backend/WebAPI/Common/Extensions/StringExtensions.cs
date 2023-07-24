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

        public static string UpdateImagesSrc(this string html, string imgUrl)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection imageNodes = document.DocumentNode.SelectNodes("//img");
            if (imageNodes != null)
            {
                foreach (HtmlNode iamgeNode in imageNodes)
                {
                    string picture = iamgeNode.GetAttributeValue("src", "");
                    iamgeNode.SetAttributeValue("src", imgUrl + picture);
                }
            }

            return document.DocumentNode.OuterHtml;
        }

        public static string RemoveLinks(this string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection anchorNodes = document.DocumentNode.SelectNodes("//a");
            if (anchorNodes != null)
            {
                foreach (HtmlNode anchorNode in anchorNodes)
                {
                    string innerText = $"<span>{anchorNode.InnerHtml}</span>"; // Extract inner text
                    anchorNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(innerText), anchorNode); // Replace anchor tag with inner text
                }
            }            

            return document.DocumentNode.OuterHtml;
        }
    }
}
