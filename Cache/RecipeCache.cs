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

            Item[] results = Recipes.Select(x => x.createItem).OrderBy(x => rand.Next()).ToArray();

            Type recipeLoader = typeof(RecipeLoader);
            FieldInfo setupRecipes = recipeLoader.GetField(
                "setupRecipes",
                BindingFlags.Static | BindingFlags.NonPublic
            )!;
            
            setupRecipes.SetValue(null, true);
            
            for (int i = 0; i < Recipes.Count; i++)
            {
                // De-referencing the Recipe object as to not interfere with our Vanilla cache!
                Recipe res = Recipes[i];

                Recipe recipe = ModContent.GetInstance<RecipeShuffler>().CreateRecipe(
                    results[i].type,
                    results[i].stack
                ).AddCondition(res.Conditions);

                // Recipe *groups* will be added as ingredients in the acceptedIngredients step.
                // This just ensures we register the groups here for handling.
                // AddIngredient actually handles the iconic item and item stack; recipe group
                // iconic items are registered to Recipe.RequiredItem.
                foreach (int acceptedGroup in res.acceptedGroups)
                    recipe.acceptedGroups.Add(acceptedGroup);

                foreach (Item requiredItem in res.requiredItem)
                    recipe.AddIngredient(requiredItem.type, requiredItem.stack);

                foreach (int requiredTile in res.requiredTile)
                    recipe.AddTile(requiredTile);
                
                // Scary!!!
                if (recipe.createItem is null || recipe.createItem.type == ItemID.None)
                {
                    Item dirt = new();
                    dirt.SetDefaults(ItemID.DirtBlock);
                    recipe.createItem = dirt;
                }
                
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
            Recipes.AddRange(recipes);
        }
    }
}