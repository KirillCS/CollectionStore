using CollectionStore.Models;
using System.Collections.Generic;

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
