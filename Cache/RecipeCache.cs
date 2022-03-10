using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace RecipeShuffler.Cache
{
    /// <summary>
    ///     Object representing a shuffled recipe state.
    /// </summary>
    public class RecipeCache
    {
        /// <summary>
        ///     A collection of cached recipes.
        /// </summary>
        protected readonly List<Recipe> Recipes = new();

        /// <summary>
        ///     A readonly collection of cached recipes.
        /// </summary>
        public ReadOnlyCollection<Recipe> ReadonlyRecipes => Recipes.AsReadOnly();
        
        /// <summary>
        ///     Verifies the integrity of a recipe cache against another recipe cache.
        /// </summary>
        public virtual bool VerifyIntegrity(RecipeCache cache) => Recipes.Count == cache.ReadonlyRecipes.Count;

        /// <summary>
        ///     Shuffles the <see cref="Recipes"/> collection, given the seed.
        /// </summary>
        public virtual void ShuffleRecipes(int seed)
        {
            Random rand = new(seed);

            Item[] results = Recipes
                // .Where(x => x.createItem is not null && x.createItem.type != ItemID.None)
                .Select(x => x.createItem)
                .OrderBy(x => rand.Next())
                .ToArray();

            FieldInfo setupRecipes = typeof(RecipeLoader).GetField(
                "setupRecipes",
                BindingFlags.Static | BindingFlags.NonPublic
            )!;
            MethodInfo memberwiseClone = typeof(Recipe).GetMethod(
                "MemberwiseClone",
                BindingFlags.Instance | BindingFlags.NonPublic
            )!;
            
            setupRecipes.SetValue(null, true);
            
            for (int i = 0; i < Recipes.Count; i++)
            {
                Recipe recipe = (Recipe) memberwiseClone.Invoke(Recipes[i], null)!;
                recipe.createItem = results[i];
                Recipes[i] = recipe;
            }
            
            setupRecipes.SetValue(null, false);
        }

        /// <summary>
        ///     Updates a <see cref="Recipes"/> collection externally.
        /// </summary>
        /// <param name="recipes"></param>
        public virtual void SetRecipes(IEnumerable<Recipe> recipes)
        {
            Recipes.Clear();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Recipes.AddRange(recipes.Where(x => x.createItem != null && x.createItem.type != ItemID.None));
        }
    }
}