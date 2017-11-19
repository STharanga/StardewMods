using System;
using System.Collections.Generic;
using System.Linq;

using SObject = StardewValley.Object;

namespace DynamicDungeons
{
    class LootHandler
    {
        private Dictionary<double, List<object>> _LootTable = new Dictionary<double, List<object>>();
        private Random _Random;
        public LootHandler(int seed, Dictionary<double, List<object>> lootTable = null)
        {
            this._Random = new Random(seed);
            this._LootTable = lootTable?.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<double, List<object>>();
        }
        public void Add(double chancePercentage, SObject itemLoot)
        {
            if (!this._LootTable.ContainsKey(chancePercentage))
                this._LootTable.Add(chancePercentage, new List<object>() { itemLoot});
            this._LootTable=this._LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }
        public void Add(double chancePercentage, Func<SObject> itemLootCallback)
        {
            if (!this._LootTable.ContainsKey(chancePercentage))
                this._LootTable.Add(chancePercentage, new List<object>() { itemLootCallback });
            this._LootTable = this._LootTable.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);
        }
        public SObject[] GetDrops(int count, double chanceModifier)
        {
            var drops = new SObject[count];
            for (int c = 0; c < count; c++)
                drops[c] = this.GetDrop(chanceModifier);
            return drops;
        }
        public SObject GetDrop(double chanceModifier)
        {
            foreach(var option in this._LootTable)
            {
                double chance = chanceModifier + option.Key;
                foreach (object item in option.Value)
                    if (this._Random.NextDouble() < chance)
                        return (item as SObject) ?? (item as Func<SObject>)() ?? GetDrop(chanceModifier);
            }
            return new SObject(93, 1);
        }
    }
}