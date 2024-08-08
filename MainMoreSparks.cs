using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace MoreSparks
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MainMoreSparks : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(EPCDKLCABNC), "PGIEGPGNBHJ")] // Progress.AddCurrency
    public class ProgressNewMaxCurrencyPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4 && (Int32)codes[i].operand == 9999)
                {
                    MainMoreSparks.Logger.LogInfo("Progress PATCHED");
                    codes[i].operand = 99999;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
    [HarmonyPatch(typeof(OEAINNHEMKA), "BKHKFPANINH")] // GameStatesGameResult.OpenResults
    public class GameResultsNewMaxCurrencyPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4 && (Int32)codes[i].operand == 9999)
                {
                    MainMoreSparks.Logger.LogInfo("OpenResults PATCHED");
                    codes[i].operand = 99999;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(CPNJEILDILH), "GNEJOIFKGIC", MethodType.Enumerator)] // ScreenGameResults.MoveNext
    public class ScreenGameResultsNewMaxCurrencyPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4 && (Int32)codes[i].operand == 9999)
                {
                    MainMoreSparks.Logger.LogInfo("ScreenGameResults PATCHED");
                    codes[i].operand = 99999;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
