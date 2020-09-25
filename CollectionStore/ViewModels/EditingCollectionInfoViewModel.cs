using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class EditingCollectionInfoViewModel
    {
        public int CollectionId { get; set; }

        [Required]
        public string Name { get; set; }

        public int ThemeId { get; set; }

        public string Description { get; set; }

        public string ReturnUrl { get; set; }
    }
}
