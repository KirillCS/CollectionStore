using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class CollectionViewModel
    {
        public Collection Collection { get; set; }

        public List<CollectionTheme> Themes { get; set; }

        public string ReturnUrl { get; set; }

        public CollectionViewModel()
        {
            Themes = new List<CollectionTheme>();
        }
    }
}
