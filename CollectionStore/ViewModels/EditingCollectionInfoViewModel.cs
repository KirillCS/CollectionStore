using System.ComponentModel.DataAnnotations;

namespace CollectionStore.ViewModels
{
    public class EditingCollectionInfoViewModel : EditingCollectionViewModelBase
    {
        [Required]
        public string Name { get; set; }

        public int ThemeId { get; set; }

        public string Description { get; set; }
    }
}
