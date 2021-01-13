﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Data;
using Smartstore.Data.Batching;
using Smartstore.Data.Hooks;

namespace Smartstore.Core.Catalog.Categories
{
    [Important]
    public class CategoryHook : AsyncDbSaveHook<Category>
    {
        private readonly SmartDbContext _db;

        public CategoryHook(SmartDbContext db)
        {
            _db = db;
        }

        protected override Task<HookResult> OnInsertedAsync(Category entity, IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Ok);

        protected override Task<HookResult> OnUpdatedAsync(Category entity, IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Ok);

        public override async Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            // Update HasDiscountsApplied property.
            var categories = entries
                .Select(x => x.Entity)
                .OfType<Category>()
                .ToList();

            foreach (var categoriesChunk in categories.Slice(100))
            {
                var categoryIdsChunk = categoriesChunk
                    .Select(x => x.Id)
                    .ToArray();

                var appliedCategoryIds = await _db.Discounts
                    .SelectMany(x => x.AppliedToCategories)
                    .Where(x => categoryIdsChunk.Contains(x.Id))
                    .Select(x => x.Id)
                    .Distinct()
                    .ToListAsync(cancelToken);

                categoriesChunk.Each(x => x.HasDiscountsApplied = appliedCategoryIds.Contains(x.Id));
            }

            await _db.SaveChangesAsync(cancelToken);

            // Validate category hierarchy.
            var invalidCategoryIds = new HashSet<int>();
            var modifiedCategories = entries
                .Where(x => x.InitialState == Smartstore.Data.EntityState.Modified)
                .Select(x => x.Entity)
                .OfType<Category>()
                .ToList();

            foreach (var category in modifiedCategories)
            {
                var valid = await IsValidCategoryHierarchy(category.Id, category.ParentCategoryId);
                if (!valid)
                {
                    invalidCategoryIds.Add(category.Id);
                }
            }

            if (invalidCategoryIds.Any())
            {
                var num = await _db.Categories
                    .Where(x => invalidCategoryIds.Contains(x.Id))
                    .BatchUpdateAsync(x => new Category { ParentCategoryId = 0 }, cancelToken);
            }
        }

        private async Task<bool> IsValidCategoryHierarchy(int categoryId, int parentCategoryId)
        {
            var parent = await _db.Categories
                .Where(x => x.Id == parentCategoryId)
                .Select(x => new { x.Id, x.ParentCategoryId })
                .FirstOrDefaultAsync();

            while (parent?.Id > 0)
            {
                if (categoryId == parent.Id)
                {
                    // Same ID > invalid.
                    return false;
                }

                if (parent.ParentCategoryId == 0)
                {
                    break;
                }

                parent = await _db.Categories
                    .Where(x => x.Id == parent.ParentCategoryId)
                    .Select(x => new { x.Id, x.ParentCategoryId })
                    .FirstOrDefaultAsync();
            }

            return true;
        }
    }
}
