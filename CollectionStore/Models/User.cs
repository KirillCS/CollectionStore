﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class User : IdentityUser
    {
        public bool IsBlocked { get; set; }

        public List<Collection> Collections { get; set; }

        public User()
        {
            Collections = new List<Collection>();
        }
    }
}
