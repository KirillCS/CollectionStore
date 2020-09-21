using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Helpers
{
    public enum SortBy
    {
        None,
        AlphabetUp,
        AlphabetDown,
        DateUp,
        DateDown,
        LikeUp,
        LikeDown
    }
    public static class Sorting
    {
        public static List<Item> SortItemsByParameter(List<Item> items, SortBy sortBy)
        {
            switch (sortBy)
            {
                case SortBy.None:
                    return items.OrderByDescending(i => i.Id).ToList();
                case SortBy.AlphabetUp:
                    return items.OrderBy(i => i.Name).ToList();
                case SortBy.AlphabetDown:
                    return items.OrderByDescending(i => i.Name).ToList();
                case SortBy.DateUp:
                    return items.OrderByDescending(i => i.Id).ToList();
                case SortBy.DateDown:
                    return items.OrderBy(i => i.Id).ToList();
                case SortBy.LikeUp:
                    return items.OrderByDescending(i => i.Likes.Count).ToList();
                case SortBy.LikeDown:
                    return items.OrderBy(i => i.Likes.Count).ToList();
                default:
                    return items.OrderByDescending(i => i.Id).ToList();
            }
        }
    }
}
