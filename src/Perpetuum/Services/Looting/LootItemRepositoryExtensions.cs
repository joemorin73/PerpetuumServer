using System;
using System.Collections.Generic;
using System.Linq;

namespace Perpetuum.Services.Looting
{
    public static class LootItemRepositoryExtensions
    {
        public static void AddMany(this ILootItemRepository repository, LootContainer container, IEnumerable<LootItem> lootItems)
        {
            foreach (var lootItem in lootItems)
            {
                repository.AddWithStack(container,lootItem);
            }
        }

        public static void AddWithStack(this ILootItemRepository repository, LootContainer container, LootItem lootItem)
        {
            var f = repository.GetByDefinition(container,lootItem.ItemInfo.Definition).FirstOrDefault(l => Math.Abs(l.ItemInfo.Health - lootItem.ItemInfo.Health) < double.Epsilon);
            // if the item has dynamic properties, do not stack it.
            if (f == null || f.ItemInfo.EntityDynamicProperties.Items.Count > 1)
            {
                repository.Add(container,lootItem);
                return;
            }

            f.Quantity += lootItem.Quantity;
            repository.Update(container,f);
        }
    }
}