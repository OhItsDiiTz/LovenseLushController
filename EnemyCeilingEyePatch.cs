using BepInEx;
using HarmonyLib;
using LovenseBLE;
using System;
using System.Reflection;
using UnityEngine;

namespace LovenseLushController
{
    [HarmonyPatch(typeof(EnemyCeilingEye), "UpdateState")]
    public class EnemyCeilingEyeUpdateStatePatch
    {
        [HarmonyPrefix]
        private static void Prefix(EnemyCeilingEye __instance, EnemyCeilingEye.State _state)
        {
            if (_state == EnemyCeilingEye.State.HasTarget)
            {
                if (LovenseSettings.Enabled)
                {
                    if (LovenseSettings.OnEyeLockonVibrate)
                    {
                        Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Convert.ToInt32(LovenseSettings.EyeLockonIntensity));
                    }
                }
            }
            if (_state == EnemyCeilingEye.State.TargetLost)
            {
                if (LovenseSettings.Enabled)
                {
                    if (LovenseSettings.OnEyeLockonVibrate)
                    {
                        Plugin.VibrateStop();
                    }
                }
            }
        }
    }
}
