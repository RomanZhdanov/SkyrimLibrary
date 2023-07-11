using HtmlAgilityPack;

namespace SkyrimLibrary.WebAPI.Utils
{
    public static class HtmlUtils
    {
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
                    string innerText = anchorNode.InnerHtml; // Extract inner text
                    anchorNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(innerText), anchorNode); // Replace anchor tag with inner text
                }
            }

            HtmlNodeCollection imageNodes = document.DocumentNode.SelectNodes("//img");
            if (imageNodes != null)
            {
                foreach (HtmlNode iamgeNode in imageNodes)
                {
                    string innerText = iamgeNode.GetAttributeValue("title", ""); // Extract inner text
                    if (!string.IsNullOrEmpty(innerText))
                    {
                        iamgeNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(innerText), iamgeNode); // Replace anchor tag with inner text
                    }
                }
            }

            return document.DocumentNode.OuterHtml;
        }
    }
}
