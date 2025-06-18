using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class AlbumGenre
    {
        public int CodGenre { get; set; }
        public string GenreName { get; set; }

        public override string ToString() => GenreName;
    }
}
