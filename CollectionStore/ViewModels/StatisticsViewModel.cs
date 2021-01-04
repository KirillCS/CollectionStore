using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class StatisticsViewModel
    {
        public List<User> TopUsersByCollections { get; set; }
        public int CollectionsCount { get; set; }
    }
}
