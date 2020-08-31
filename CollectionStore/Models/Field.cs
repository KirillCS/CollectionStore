using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TypeId { get; set; }
        public FieldType Type { get; set; }

        public int CollectionId { get; set; }
        public Collection Collection { get; set; }

        public List<FieldValue> FieldValues { get; set; }

        public Field()
        {
            FieldValues = new List<FieldValue>();
        }
    }
}
