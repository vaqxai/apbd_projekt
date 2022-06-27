using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd_projekt.Shared
{
    public class Article
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }

        public Article(string title, string content, string link)
        {
            this.Title = title;
            this.Content = content;
            this.Link = link;
        }
    }
}
