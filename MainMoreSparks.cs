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

#if DEBUG

using MoreSparks.Gambler;

#endif

namespace MoreSparks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class MainMoreSparks : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    internal Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    internal static MainMoreSparks Instance { get; private set; }

    private void Awake()
    {
        // Plugin startup logic
        Instance = this;
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        try
        {
            harmony.PatchAll(typeof(Spark_Patches));
#if DEBUG
            harmony.PatchAll(typeof(ParryGambler_Patches));
            DebugSettings.instance.gainXpOffline = true;
#endif
        }
        catch (Exception)
        {
            MainMoreSparks.Logger.LogFatal($"Failed to Patch '{MyPluginInfo.PLUGIN_NAME}' UnPatching Self");
            MainMoreSparks.Instance.harmony.UnpatchSelf();
            throw;
        }
    }

#if DEBUG

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.U))
        {
            Logger.LogWarning("-100 xp");
            BDCINPKBMBL.xp -= 100;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Y))
        {
            Logger.LogWarning("+100 xp");
            BDCINPKBMBL.xp += 100;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            Logger.LogWarning("Add coin 10000");
            EPCDKLCABNC.PGIEGPGNBHJ(10000);
        }
    }

#endif
}