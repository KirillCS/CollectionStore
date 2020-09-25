using Microsoft.AspNetCore.Http;

namespace CollectionStore.ViewModels
{
    public class EditingCollectionCoverViewModel : EditingCollectionViewModelBase
    {
        public IFormFile File { get; set; }
    }
}
