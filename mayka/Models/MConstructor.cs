using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MConstructor
    {
        public int Id { get; set; }

        public byte[] FullImg { get; set; }

        public byte[] FullImgBack { get; set; }

        public string Font { get; set; }

        public virtual List<MPhoto> Photos { get; set; }

        public MConstructor()
        {
            Photos = new List<MPhoto>();
        }
    }
}
