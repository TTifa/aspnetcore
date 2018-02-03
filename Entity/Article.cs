using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Author { get; set; }
        public int AuthorUid { get; set; }
    }
}
