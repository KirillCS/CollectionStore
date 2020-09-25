using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class EditingCollectionFieldsViewModel : EditingCollectionViewModelBase
    {
        public List<string> FieldNames { get; set; }

        public List<int> FieldTypesIds { get; set; }

        public EditingCollectionFieldsViewModel()
        {
            FieldNames = new List<string>();
            FieldTypesIds = new List<int>();
        }
    }
}
