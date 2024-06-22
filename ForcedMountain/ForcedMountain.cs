using BepInEx;
using RoR2;
using UnityEngine;

namespace ForcedMountain
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class ForcedMountain : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "FakeDuck";
        public const string PluginName = "ForcedMountain";
        public const string PluginVersion = "1.0.2";

        private int mountainShrineCount = 0;

        public void Awake()
        {
            Log.Init(Logger);

            On.RoR2.TeleporterInteraction.GetInteractability += TeleporterInteraction_GetInteractability;
            SceneDirector.onGenerateInteractableCardSelection += ResetShrineCount;
            On.RoR2.DirectorCore.TrySpawnObject += CountShrines;
        }

        private GameObject CountShrines(On.RoR2.DirectorCore.orig_TrySpawnObject orig, DirectorCore self, DirectorSpawnRequest directorSpawnRequest)
        {
            var card = directorSpawnRequest.spawnCard;
            if (card.name.Contains("iscShrineBoss"))
            {
                mountainShrineCount += 1;
                Log.Info($"Found one shrine, current shrines = {mountainShrineCount}");
            }
            return orig(self, directorSpawnRequest);
        }

        private void ResetShrineCount(SceneDirector director, DirectorCardCategorySelection selection)
        {
            mountainShrineCount = 0;
            // Log.Info($"ForcedMountain ResetShrine Called, current shrines = {mountainShrineCount}");
        }

        private Interactability TeleporterInteraction_GetInteractability(On.RoR2.TeleporterInteraction.orig_GetInteractability orig, TeleporterInteraction self, Interactor activator)
        {
            if (TeleporterInteraction.instance.shrineBonusStacks == mountainShrineCount) 
            {
                return orig(self, activator);
            }
            return Interactability.ConditionsNotMet;
        }


    }
}

