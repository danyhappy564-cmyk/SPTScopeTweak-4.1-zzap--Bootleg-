using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Diz.Jobs;
using EFT.CameraControl;
using SPTScopeTweaks.Data;
using SPTScopeTweaks.Helpers;
using SPTScopeTweaks.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace SPTScopeTweaks
{
    [BepInPlugin("com.pein.scopetweak", "Eye Relief Tweak", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static ConfigEntry<float> EyeReliefMultiplier { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;

            new OpticSightAwakePatch().Enable();
            // OnGameEndedPatch existed but was never enabled, so OpticMaterialData kept
            // every OpticSight the session had ever seen - destroyed Unity objects
            // included - and the SettingChanged handler below walked the whole pile on
            // each config change. Clearing it at raid end is what the patch is for.
            new OnGameEndedPatch().Enable();

            EyeReliefMultiplier = Config.Bind(
                "General",
                "Eye Relief Multiplier",
                1.4f,
                new ConfigDescription(
                    "Multiplies the eye relief size for all optics.",
                    new AcceptableValueRange<float>(0.1f, 5f)
                )
            );

            EyeReliefMultiplier.SettingChanged += (sender, args) =>
            {
                foreach (KeyValuePair<OpticSight, ScopeMaterialData> kvp in ScopeMaterialData.OpticMaterialData)
                {
                    OpticSight opticSight = kvp.Key;
                    ScopeMaterialData scopeMaterialData = kvp.Value;

                    if (opticSight != null && scopeMaterialData != null)
                    {
                        ScopeHelper.UpdateScopeEyeRelief(opticSight, EyeReliefMultiplier.Value);
                    }
                }
            };
        }
    }
}
