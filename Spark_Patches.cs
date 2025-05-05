using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace MoreSparks;

internal class Spark_Patches
{
    private const Int32 NEW_MAX_CURRENCY = 999999;

    [HarmonyPatch(typeof(EPCDKLCABNC), nameof(EPCDKLCABNC.PGIEGPGNBHJ))]    // Progress.AddCurrency
    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.BKHKFPANINH))]    // GameStatesGameResult.OpenResults
    [HarmonyPatch(typeof(CPNJEILDILH), nameof(CPNJEILDILH.GNEJOIFKGIC), MethodType.Enumerator)] // ScreenGameResults.CShowResult
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        CodeMatcher cm = new CodeMatcher(instructions);

        cm.SearchForward(iL => iL.opcode == OpCodes.Ldc_I4 && (Int32)iL.operand == 9999)
            .ThrowIfNotMatch("Didn't find value 9999 in method")
            .SetOperandAndAdvance(NEW_MAX_CURRENCY);

        return cm.InstructionEnumeration();
    }

    [HarmonyPatch(typeof(PostScreen), nameof(PostScreen.OnOpen))]
    [HarmonyPostfix]
    private static void AdjustCurrencyIcon(PostScreen __instance)
    {
        // Moves icon left a bit due to the increased currency amount.
        __instance.lbCurrency.transform.GetChild(0)
            .localPosition = new Vector3(-30, 1, 0);
    }
}