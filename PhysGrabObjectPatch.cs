using BepInEx;
using HarmonyLib;
using LovenseBLE;
using System.Reflection;
using UnityEngine;

namespace LovenseLushController
{
    [HarmonyPatch(typeof(PhysGrabObject), "GrabStarted")]
    public class PhysGrabObjectGrabStartedPatch
    {
        /// <summary>
        /// This is currently working but not as intended
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(PhysGrabObject __instance, PhysGrabber player)
        {
            if (LovenseSettings.Enabled)
            {
                if (LovenseSettings.OnGrabStartedVibrate)
                {
                    if (!LovenseSettings.isItemGrabbed)
                    {
                        Plugin.Logger.LogInfo("PhysGrabObject GrabStarted");
                        Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : System.Convert.ToInt32(LovenseSettings.GrabStartedVibrateIntensity));
                        LovenseSettings.isItemGrabbed = true;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PhysGrabObject), "GrabEnded")]
    public class PhysGrabObjectGrabEndedPatch
    {
        /// <summary>
        /// This is currently working but not as intended
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(PhysGrabObject __instance, PhysGrabber player)
        {
            if (LovenseSettings.Enabled)
            {
                if (LovenseSettings.OnGrabStartedVibrate)
                {
                    if (LovenseSettings.isItemGrabbed)
                    {
                        Plugin.Logger.LogInfo("PhysGrabObject GrabEnded");
                        Plugin.VibrateStop();
                        LovenseSettings.isItemGrabbed = false;
                    }
                }
            }
        }
    }
}
