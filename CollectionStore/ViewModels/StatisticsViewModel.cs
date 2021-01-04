using System.Collections.Generic;

namespace CollectionStore.ViewModels
{
    public class StatisticsViewModel
    {
        public IEnumerable<string> TopUsersByCollections { get; set; }
        public int MaxCollectionsCount { get; set; }

        public IEnumerable<string> TopUsersByCommenting { get; set; }
        public int MaxCommentsCount { get; set; }

        public IEnumerable<string> TopTags { get; set; }
        public int MaxTagsFrequencyOfUse { get; set; }

        public Dictionary<string, int> ThemesData { get; set; }
    }
}
