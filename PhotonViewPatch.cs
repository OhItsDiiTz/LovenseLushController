using BepInEx;
using HarmonyLib;
using LovenseBLE;
using Photon.Pun;
using System;
using System.Reflection;
using UnityEngine;

namespace LovenseLushController
{
    [HarmonyPatch]
    public class PhotonViewRPCPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(PhotonView),               // <-- Change to actual class
                "RPC",
                new System.Type[] { typeof(string), typeof(RpcTarget), typeof(object[]) }
            );
        }

        //[HarmonyPrefix]
        private static void Prefix(PhotonView __instance, string methodName, RpcTarget target, params object[] parameters)
        {
            //Plugin.Logger.LogInfo($"RPC({methodName}) with {parameters.Length} args");
            //if (LovenseSettings.Enabled)
            //{
            //    if (LovenseSettings.OnGlobalImpactVibrate)
            //    {
            //        if (methodName.Contains("Impact"))
            //        {
            //            Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Convert.ToInt32(LovenseSettings.GlobalImpactVibrateIntensity));
            //            Plugin.VibrateStop();
            //        }
            //    }
            //}
        }
    }
}
