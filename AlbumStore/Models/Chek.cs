using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class Chek
    {
        public int CheckNumber { get; set; }
        public DateTime DateOfSale { get; set; }
        public TimeSpan SaleTime { get; set; }
        public Album Album { get; set; }
        public int NumberOfAlbums { get; set; }
    }
}