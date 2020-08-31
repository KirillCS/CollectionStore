using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public List<Collection> Collections { get; set; }
    }
}
