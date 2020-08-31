using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class FieldType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Field> Fields { get; set; }

        public FieldType()
        {
            Fields = new List<Field>();
        }
    }
}
