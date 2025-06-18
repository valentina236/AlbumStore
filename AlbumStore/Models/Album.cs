using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class Album
    {
        public int AlbumCod { get; set; }
        public string AlbumName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ArtistName { get; set; }
        public string ManufacturerName { get; set; }
        public AlbumFormat Format { get; set; }
        public AlbumGenre Genre { get; set; }
        public AlbumType Type { get; set; }
        public decimal UnitPrice { get; set; }
        public string Photo {  get; set; }

        public override string ToString() => AlbumName;

        public string ImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(Photo))
                    return "/Images/album_placeholder.png";

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDirectory, "Images", Photo).Replace("\\", "/");
                return fullPath;
            }
        }
    }
}