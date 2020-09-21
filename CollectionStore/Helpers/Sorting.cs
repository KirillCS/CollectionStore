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
        public static void SortItemsInCollection(Collection collection, SortBy sortBy)
        {
            switch (sortBy)
            {
                case SortBy.None:
                    collection.Items = collection.Items.OrderByDescending(i => i.Id).ToList();
                    break;
                case SortBy.AlphabetUp:
                    collection.Items = collection.Items.OrderBy(i => i.Name).ToList();
                    break;
                case SortBy.AlphabetDown:
                    collection.Items = collection.Items.OrderByDescending(i => i.Name).ToList();
                    break;
                case SortBy.DateUp:
                    collection.Items = collection.Items.OrderByDescending(i => i.Id).ToList();
                    break;
                case SortBy.DateDown:
                    collection.Items = collection.Items.OrderBy(i => i.Id).ToList();
                    break;
                case SortBy.LikeUp:
                    collection.Items = collection.Items.OrderByDescending(i => i.Likes.Count).ToList();
                    break;
                case SortBy.LikeDown:
                    collection.Items = collection.Items.OrderBy(i => i.Likes.Count).ToList();
                    break;
                default:
                    collection.Items = collection.Items.OrderByDescending(i => i.Id).ToList();
                    break;
            }
        }
    }
}
