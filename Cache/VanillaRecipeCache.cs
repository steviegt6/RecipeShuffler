using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace RecipeShuffler.Cache
{
    public class VanillaRecipeCache : RecipeCache
    {
        public VanillaRecipeCache()
        {
            foreach (Recipe recipe in Main.recipe.Where(
                         x => x?.createItem != null && x.createItem.type != ItemID.None)
                    ) Recipes.Add(recipe);
        }

        public override void ShuffleRecipes(int seed) =>
            throw new InvalidOperationException("Cannot shuffle a vanilla recipe cache.");

        /// <summary>
        ///     Instead of updating <see cref="RecipeCache.Recipes"/>, the vanilla recipe array is updated.
        /// </summary>
        public override void SetRecipes(IEnumerable<Recipe> recipes)
        {
            Recipe.numRecipes = 0;
            Recipe.maxRecipes = 50;
            Main.recipe = new Recipe[Recipe.maxRecipes];
            Main.availableRecipe = new int[Recipe.maxRecipes];
            Main.availableRecipeY = new float[Recipe.maxRecipes];

            List<Recipe> list = recipes.ToList();

            foreach (Recipe recipe in list)
                recipe.Register();
        }
    }
}