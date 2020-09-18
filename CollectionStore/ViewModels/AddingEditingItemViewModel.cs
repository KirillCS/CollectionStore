using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class AddingEditingItemViewModel
    {
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        public List<string> TagNames { get; set; }

        public List<string> Values { get; set; }

        public List<int> FieldIds { get; set; }

        public int CollectionId { get; set; }

        public Collection Collection { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsEditing { get; set; }

        public AddingEditingItemViewModel()
        {
            Values = new List<string>();
            FieldIds = new List<int>();
            TagNames = new List<string>();
        }
    }
}
