using ReindexerClient.Attributes;
using SkyrimLibrary.WebAPI.Attributes;

namespace SkyrimLibrary.WebAPI.Models
{
    [SearchNamespace("books")]
    public class BookSearchItem
    {
        [SearchIndex("hash", isPk = true)]
        public string Id { get; set; }

        [SearchIndex(search = true, boost = 1.6, function = "highlight(<mark>,</mark>)")]
        public string Title { get; set; }

        [SearchIndex(search = true, boost = 0.5, function = "highlight(<mark>,</mark>)")]
        public string Description { get; set; }

        [StripHTML]
        [SearchIndex(search = true, boost = 1, function = "snippet(<mark>,</mark>,30,30, ...,... <br/>)")]
        public string Text { get; set; }

        [SearchIndex(search = true, boost = 1, function = "highlight(<mark>,</mark>)")]
        public string Author { get; set; }

        [SearchIndex]
        public string CoverImage { get; set; }
    }
}
