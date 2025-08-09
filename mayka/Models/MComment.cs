using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Img { get; set; }
        public bool Public { get; set; }
    }
}
