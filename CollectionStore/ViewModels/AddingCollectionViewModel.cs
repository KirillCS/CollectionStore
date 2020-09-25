using CollectionStore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollectionStore.ViewModels
{
    public class AddingCollectionViewModel
    {
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public List<CollectionTheme> Themes { get; set; }

        public int SelectedThemeId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public IFormFile File { get; set; }

        public List<FieldType> Types { get; set; }

        public List<string> FieldNames { get; set; }

        public List<int> FieldTypesIds { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string ReturnUrl { get; set; }
    }
}
