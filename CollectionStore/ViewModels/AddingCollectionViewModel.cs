using CollectionStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class AddingCollectionViewModel
    {
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public List<CollectionTheme> Themes { get; set; }

        public SelectListItem SelectedTheme { get; set; }

        public IFormFile File { get; set; }

        public List<FieldType> Types { get; set; }

        public string ReturnUrl { get; set; }

        public AddingCollectionViewModel()
        {
            Themes = new List<CollectionTheme>();
        }
    }
}
