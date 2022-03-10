using System.Collections.Generic;
using RecipeShuffler.Cache;
using Terraria.ModLoader;

namespace RecipeShuffler
{
	public class RecipeShuffler : Mod
	{
		/// <summary>
		///		Represents the Recipe cache in its vanilla state. Should be references as the "natural" state of recipes.
		/// </summary>
		/// <remarks>
		///		Useful for re-shuffling recipes between worlds.
		/// </remarks>
		public RecipeCache VanillaCache { get; protected set; } = null!; // Never null when used.

		public readonly Dictionary<int, RecipeCache> Caches = new();

		public override void PostAddRecipes()
		{
			base.PostAddRecipes();

			VanillaCache = new VanillaRecipeCache();
		}
	}
}