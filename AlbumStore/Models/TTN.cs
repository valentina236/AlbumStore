using System;

namespace AlbumStore.Models
{
    public class TTN
    {
        public int NumDoc { get; set; }
        public DateTime DatePost { get; set; }
        public Provider Provider { get; set; }
        public Album Album { get; set; }
        public int NumberOfAlbums { get; set; }
    }
}
