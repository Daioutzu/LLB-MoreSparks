using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LLScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;

namespace MoreSparks
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MainMoreSparks : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;
        private Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        public static TextMeshProUGUI currencyText;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            harmony.PatchAll(typeof(Spark_Patches));
        }
    }
}