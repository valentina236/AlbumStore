﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumStore.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string RoleName { get; set; }
    }
}
