using BepInEx;
using HarmonyLib;
using LovenseBLE;
using Photon.Pun;
using System;
using System.Reflection;
using UnityEngine;

namespace LovenseLushController
{
    [HarmonyPatch(typeof(PlayerAvatar), "PlayerDeath")]
    public class PlayerAvatarPlayerDeathPatch
    {
        /// <summary>
        /// This is currently working as intended
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(PlayerAvatar __instance, int enemyIndex)
        {
            if (LovenseSettings.Enabled)
            {
                if (LovenseSettings.OnPlayerDeathVibrate)
                {
                    Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Convert.ToInt32(LovenseSettings.PlayerDeathIntensity));
                    Plugin.VibrateStop();
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerAvatar), "ReviveRPC")]
    public class PlayerAvatarReviveRPCPatch
    {
        [HarmonyPrefix]
        private static void Prefix(PlayerAvatar __instance, bool _revivedByTruck, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            Plugin.Logger.LogDebug("PlayerAvatar.ReviveRPC patch pre");
        }
    }

    /// <summary>
    /// This is currently working as intended
    /// </summary>
    [HarmonyPatch(typeof(PlayerAvatar), "ChatMessageSendRPC")]
    public class PlayerAvatarChatMessageSendRPCPatch
    {
        [HarmonyPrefix]
        private static void Prefix(PlayerAvatar __instance, string _message, bool crouching, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            Plugin.Logger.LogDebug("PlayerAvatar.ChatMessageSendRPC patch pre");

            if (LovenseSettings.Enabled)
            {
                if (LovenseSettings.AllowChatBoops)
                {
                    if (_message.ToLower() == "boop")
                    {
                        Plugin.VibrateToy(LovenseSettings.RandomizedIntensity ? (UnityEngine.Random.Range(1, 20)) : Convert.ToInt32(LovenseSettings.AllowChatBoopsIntensity));
                        Plugin.VibrateStop();
                    }
                }
            }
        }
    }
}
