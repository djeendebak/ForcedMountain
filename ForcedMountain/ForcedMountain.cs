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
        public const string PluginVersion = "1.0.3";

        private int mountainShrineCount = 0;

        public BepInEx.Configuration.ConfigEntry<bool> AutoActivate { get; set; }

        public void Awake()
        {
            Log.Init(Logger);

            AutoActivate = base.Config.Bind<bool>("Toggles", "Enable auto activation of shrines", false, "Set to true or false , if set to true , will activate the shrines automatically. Normal: false");
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

                if (AutoActivate.Value)
                {
                    TeleporterInteraction.instance.AddShrineStack();
                }
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

