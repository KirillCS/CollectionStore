using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class EditingCollectionViewModelBase
    {
        public int CollectionId { get; set; }

        public string ReturnUrl { get; set; }
    }
}
