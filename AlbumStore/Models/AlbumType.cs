using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class AlbumType
    {
        public int CodType { get; set; }
        public string TypeName { get; set; }

        public override string ToString() => TypeName;
    }
}
