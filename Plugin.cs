using BepInEx;
using HarmonyLib;
using System.Reflection;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace FasterPharaoh
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Patches : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            MethodInfo original = AccessTools.Method(typeof(TimeManager), "SetGameSpeed");
            MethodInfo patch = AccessTools.Method(typeof(Patches), "SetGameSpeed_Patch");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        private static bool SetGameSpeed_Patch(TimeManager.GameSpeed newGameSpeed,ref TimeManager __instance, ref float ____currentSpeed, ref TimeManager.GameSpeed ___CurrentGameSpeed, bool force = false )
        {
            if (___CurrentGameSpeed == newGameSpeed && !force)
            {
                return false;
            }
            ___CurrentGameSpeed = newGameSpeed;
            switch (newGameSpeed)
            {
                case TimeManager.GameSpeed.Low:
                    Traverse.Create(__instance).Property("SpeedFactor").SetValue(0.5f);
                    break;
                case TimeManager.GameSpeed.Normal:
                    Traverse.Create(__instance).Property("SpeedFactor").SetValue(1f);
                    break;
                case TimeManager.GameSpeed.Fast:
                    Traverse.Create(__instance).Property("SpeedFactor").SetValue(5f);
                    break;
                case TimeManager.GameSpeed.VeryFast:
                    Traverse.Create(__instance).Property("SpeedFactor").SetValue(15f);
                    break;
            }
            ____currentSpeed = (float)Traverse.Create(__instance).Property("SpeedFactor").GetValue() * (__instance.IsPaused ? 0f : 1f);
            __instance._uiManager.DisplayGameSpeed(___CurrentGameSpeed);
            return false;
        }


    }
}
