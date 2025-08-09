using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class MPhoto
    {
        public int Id { get; set; }

        public byte[] Img { get; set; }

        public int ConstructorId { get; set; }

        [ForeignKey(nameof(ConstructorId))]
        public MConstructor Constructor { get; set; }

    }
}
