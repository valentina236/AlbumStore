using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class AlbumFormat
    {
        public int CodFormat { get; set; }
        public string FormatName { get; set; }

        public override string ToString() => FormatName;
    }
}
