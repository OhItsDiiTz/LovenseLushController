using HarmonyLib;
using LovenseBLE;
using Photon.Pun;
using System;
using UnityEngine;

namespace LovenseLushController
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "BreakRPC")]
    public class PhysGrabObjectImpactDetectorBreakRPCPatch
    {
        /// <summary>
        /// This is currently working as intended
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(PhysGrabObjectImpactDetector __instance, float valueLost, Vector3 _contactPoint, int breakLevel, bool _loseValue, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            if (LovenseSettings.Enabled)
            {
                ValuableObject valObj = __instance.GetComponent<ValuableObject>();
                if (valObj != null) {
                    var field = AccessTools.Field(typeof(ValuableObject), "dollarValueCurrent");
                    var value = field.GetValue(valObj);
                    int ival = Convert.ToInt32(value);
                    int ival1 = Convert.ToInt32(valueLost);

                    if (LovenseSettings.TestingMode)
                    {
                        field.SetValue(valObj, 1000000);
                    }

                    if (LovenseSettings.OnBreakVibrate)
                    {
                        Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Plugin.Map1to20(ival1, ival));
                        if (!LovenseSettings.isItemGrabbed)
                        {
                            Plugin.VibrateStop();
                        }
                    }
                    //Plugin.Logger.LogInfo($"{valObj.valuePreset.valueMin} {valObj.valuePreset.valueMax} {value} {Plugin.Map1to20(ival1, ival)}");
                }
            }
        }
    }
}
