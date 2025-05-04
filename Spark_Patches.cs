using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace MoreSparks;

internal class Spark_Patches
{
    private const Int32 NEW_MAX_CURRENCY = 99999;

    [HarmonyPatch(typeof(EPCDKLCABNC), nameof(EPCDKLCABNC.PGIEGPGNBHJ))]    // Progress.AddCurrency
    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.BKHKFPANINH))]    // GameStatesGameResult.OpenResults
    [HarmonyPatch(typeof(CPNJEILDILH), nameof(CPNJEILDILH.GNEJOIFKGIC), MethodType.Enumerator)] // ScreenGameResults.CShowResult
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.SearchForward(iL => iL.opcode == OpCodes.Ldc_I4 && (Int32)iL.operand == 9999)
            .ThrowIfNotMatch("Didn't find value 9999 in method")
            .SetOperandAndAdvance(NEW_MAX_CURRENCY);

        return cm.InstructionEnumeration();
    }
}