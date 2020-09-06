using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class AddingItemViewModel
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        public List<int?> FieldIds { get; set; }

        public List<string> Values { get; set; }

        public int? CollectionId { get; set; }

        public string CollectionName { get; set; }

        public List<Field> Fields { get; set; }

        public string ReturnUrl { get; set; }
    }
}
