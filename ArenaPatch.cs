using BepInEx;
using HarmonyLib;
using LovenseBLE;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This is for patches done to the Arena class
/// </summary>

namespace LovenseLushController
{
    [HarmonyPatch(typeof(Arena), "CrownGrab")]
    public class ArenaCrownGrabPatch
    {
        /// <summary>
        /// This is currently working as intended
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(Arena __instance)
        {
            if (LovenseSettings.Enabled)
            {
                if (LovenseSettings.OnCrownGrabVibrate)
                {
                    Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Convert.ToInt32(LovenseSettings.CrownGrabVibrateIntensity));
                    Plugin.VibrateStop();
                }
            }
        }
    }
}
