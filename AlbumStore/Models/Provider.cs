using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class Provider
    {
        public int ProviderCod { get; set; }
        public string ProviderName { get; set; }
        public string Address { get; set; }
        public string PhoneFax { get; set; }
        public string RascetScet { get; set; }

        public override string ToString() => ProviderName;
    }
}
