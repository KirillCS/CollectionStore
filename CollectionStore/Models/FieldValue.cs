using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class FieldValue
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }

        public int? ItemId { get; set; }
        public Item Item { get; set; }
    }
}
