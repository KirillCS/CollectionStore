using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CollectionStore.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext context;

        public StatisticsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new StatisticsViewModel();
            SetTopUsersByCollections(viewModel);
            SetTopUsersByCommenting(viewModel);
            SetTopTags(viewModel);
            SetThemesData(viewModel);

            return View(viewModel);
        }

        private void SetTopUsersByCollections(StatisticsViewModel viewModel)
        {
            var grouppedCollections = context.Collections.GroupBy(c => c.UserId)
                                                         .Select(g => new { context.Users.FirstOrDefault(u => u.Id == g.Key).UserName, Count = g.Count() })
                                                         .ToList();
            viewModel.MaxCollectionsCount = grouppedCollections.IsNullOrEmpty() ? 0 : grouppedCollections.Max(g => g.Count);
            viewModel.TopUsersByCollections = grouppedCollections.Where(g => g.Count == viewModel.MaxCollectionsCount)
                                                                 .Select(g => g.UserName);
        }

        private void SetTopUsersByCommenting(StatisticsViewModel viewModel)
        {
            var grouppedComments = context.Comments.GroupBy(c => c.UserId)
                                                   .Select(g => new { context.Users.FirstOrDefault(u => u.Id == g.Key).UserName, Count = g.Count() })
                                                   .AsEnumerable();
            viewModel.MaxCommentsCount = grouppedComments.IsNullOrEmpty() ? 0 : grouppedComments.Max(g => g.Count);
            viewModel.TopUsersByCommenting = grouppedComments.Where(g => g.Count == viewModel.MaxCommentsCount)
                                                             .Select(g => g.UserName);
        }

        private void SetTopTags(StatisticsViewModel viewModel)
        {
            var grouppedTags = context.ItemTags.GroupBy(it => it.TagId)
                                               .Select(g => new { context.Tags.FirstOrDefault(t => t.Id == g.Key).Content, Count = g.Count() })
                                               .AsEnumerable();
            viewModel.MaxTagsFrequencyOfUse = grouppedTags.IsNullOrEmpty() ? 0 : grouppedTags.Max(g => g.Count);
            viewModel.TopTags = grouppedTags.Where(g => g.Count == viewModel.MaxTagsFrequencyOfUse)
                                            .Select(g => g.Content);
        }

        private void SetThemesData(StatisticsViewModel viewModel)
        {
            var grouppedCollections = context.Collections.GroupBy(c => c.ThemeId)
                                                         .Select(g => new { context.CollectionThemes.FirstOrDefault(ct => ct.Id == g.Key).Name, Count = g.Count() })
                                                         .OrderBy(g => g.Count);
            viewModel.ThemesData = new Dictionary<string, int>();
            foreach (var group in grouppedCollections)
            {
                viewModel.ThemesData.Add(group.Name, group.Count);
            }
        }
    }
}
