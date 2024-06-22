using System.Collections.Generic;
using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
// using UnityEngine.AddressableAssets;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine.Networking;

namespace ForcedMountain
{
    public class BasePlugin : NetworkBehaviour
    {
        private bool allShrinesActivated = true;

        public delegate void orig_OnInteractionBegin(GenericInteraction self, Interactor activator);
        public Hook hook_OnInteractionBegin;

        public delegate Interactability orig_GetInteractability(GenericInteraction self, Interactor activator);
        public Hook hook_GetInteractability;

        public void Awake()
        {

            On.RoR2.TeleporterInteraction.GetInteractability += TeleporterInteraction_GetInteractability;
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;

            hook_GetInteractability = new Hook(typeof(GenericInteraction).GetMethod("RoR2.IInteractable.GetInteractability", BindingFlags.NonPublic | BindingFlags.Instance), typeof(ForcedMountain).GetMethod("GenericInteraction_GetInteractability"), this, new HookConfig());
            hook_OnInteractionBegin = new Hook(typeof(GenericInteraction).GetMethod("RoR2.IInteractable.OnInteractionBegin", BindingFlags.NonPublic | BindingFlags.Instance), typeof(ForcedMountain).GetMethod("GenericInteraction_OnInteractionBegin"), this, new HookConfig());
        }

        public void OnDestroy()
        {
            On.RoR2.TeleporterInteraction.GetInteractability -= TeleporterInteraction_GetInteractability;
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= TeleporterInteraction_OnInteractionBegin;
            On.RoR2.Run.OnServerSceneChanged -= Run_OnServerSceneChanged;

            hook_GetInteractability.Dispose();
            hook_OnInteractionBegin.Dispose();

        }

        private Interactability TeleporterInteraction_GetInteractability(On.RoR2.TeleporterInteraction.orig_GetInteractability orig, TeleporterInteraction self, Interactor activator) 
        {
            if  (allShrinesActivated)
            {
                return orig(self, activator);
            }

            return Interactability.ConditionsNotMet;
        }

        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, RoR2.TeleporterInteraction self, RoR2.Interactor activator)
        {
            if (allShrinesActivated)
            {
                orig(self, activator);
            }
        }

        public Interactability GenericInteraction_GetInteractability(On.RoR2.TeleporterInteraction.orig_GetInteractability orig, TeleporterInteraction self, Interactor activator) 
        {

            if (!self.name.ToLower().Contains("portal"))
                orig(self, activator);

            if  (allShrinesActivated)
            {
                return orig(self, activator);
            }

            return Interactability.ConditionsNotMet;
        }

        public void GenericInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, RoR2.TeleporterInteraction self, RoR2.Interactor activator)
        {

            if (!self.name.ToLower().Contains("portal"))
                orig(self, activator);
            
            if (allShrinesActivated)
            {
                orig(self, activator);
            }
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            GameObject preplaced = GameObject.Find("HOLDER: Preplaced Goodies");
            List<GameObject> mountainShrines = [preplaced.transform.Find("iscShrineBoss").gameObject];
            Log.Info($"Found shrines {mountainShrines}");
        }
    }
}
