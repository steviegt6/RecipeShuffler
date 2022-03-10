using RecipeShuffler.Cache;
using Terraria;
using Terraria.ModLoader;

namespace RecipeShuffler
{
    public class WorldRecipeRandomizer : ModPlayer
    {
        private RecipeShuffler Shuffler => (RecipeShuffler) Mod;

        public override void OnEnterWorld(Player player)
        {
            void Log(string msg) => Main.NewText($"[{Mod.Name}] {msg}");

            if (!int.TryParse(WorldGen.currentWorldSeed, out int seed))
            {
                Log($"Could not parse world seed \"{seed}\", aborting recipe shuffling.");
                return;
            }
            
            Log($"Loading recipe cache for world seed: {seed}");

            if (Shuffler.Caches.ContainsKey(seed))
            {
                if (Shuffler.Caches[seed].VerifyIntegrity(Shuffler.VanillaCache))
                {
                    Log("Failed to verify pre-existing recipe cache for world, going to re-shuffle.");
                    goto IntegFail;
                }
                
                Log("Using verified pre-existing recipe cache for world.");
                
                return;
            }
            
            IntegFail:
            RecipeCache cache = new();
            cache.SetRecipes(Shuffler.VanillaCache.ReadonlyRecipes);
            cache.ShuffleRecipes(seed);

            if (!cache.VerifyIntegrity(Shuffler.VanillaCache))
            {
                Log("Couldn't verify integrity of brand-new recipe cache, panicking!");
                return;
            }
            
            Log("Successfully shuffled and verified newly-created recipe cache.");
            
            Shuffler.VanillaCache.SetRecipes(cache.ReadonlyRecipes);
            Shuffler.Caches[seed] = cache;
            
            Log("Applied new recipe cache to the current world.");
        }
    }
}