using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class JurnalUceta
    {
        public string MonthName { get; set; }
        public Album Album { get; set; }
        public int NumbeOfDeliveredAlbums { get; set; }
        public int NumberOfAlbumsSold { get; set; }
    }
}