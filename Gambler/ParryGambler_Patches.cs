using CodeStage.AntiCheat.ObscuredTypes;
using GameplayEntities;
using HarmonyLib;
using LLGUI;
using LLHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

#if DEBUG
namespace MoreSparks.Gambler;

internal class ParryGambler_Patches
{
    [HarmonyPatch(typeof(World), nameof(World.InitHandlers))]
    [HarmonyPostfix]
    private static void Initilise_GrabGambleHandler(World __instance)
    {
        __instance.gameObject.AddComponent<GrabGamblerHandler>();
    }

    [HarmonyPatch(typeof(GetHitPlayerEntity), nameof(GetHitPlayerEntity.FailGrab))]
    [HarmonyPostfix]
    private static void LoseSparks_OnFailedGrab(GetHitPlayerEntity __instance)
    {
        GrabGamblerHandler.instance.CounterFail++;

        //EPCDKLCABNC.ICHBCIDNDJA(650);
        __instance.PlaySfx(LLHandlers.Sfx.POWERUP_WARP);
    }

    [HarmonyPatch(typeof(GetHitPlayerEntity), nameof(GetHitPlayerEntity.CounterParry))]
    [HarmonyPostfix]
    private static void GainSparks_CounterGrab(GetHitPlayerEntity __instance)
    {
        GrabGamblerHandler.instance.CounterSuccess++;

        //EPCDKLCABNC.PGIEGPGNBHJ(500);
        __instance.PlaySfx(LLHandlers.Sfx.RESULT_CURRENCY_TOTAL);
    }

    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.BKHKFPANINH))]    // GameStatesGameResult.OpenResults
    [HarmonyTranspiler]
    [HarmonyDebug]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.SearchForward(il => il.opcode == OpCodes.Call && (il.operand as MethodBase).Name == nameof(EPCDKLCABNC.PGIEGPGNBHJ))
            .ThrowIfNotMatch("Didn't find 'EPCDKLCABNC.PGIEGPGNBHJ' aka 'Progress.AddCurrency'");

        // Changes the method 'Progress.AddCurrency' to 'ParryGambler_Patches.ChangeCurrency'
        cm.SetOperandAndAdvance(SymbolExtensions.GetMethodInfo(() => ChangeCurrency(default)))
            .Insert(
            new CodeInstruction(OpCodes.Call, typeof(CodeStage.AntiCheat.ObscuredTypes.ObscuredInt).GetMethod("op_Implicit", new Type[] { typeof(Int32) })),
            new CodeInstruction(OpCodes.Stloc, (SByte)11)
            );

        // Moves back and changes the IF condition from a 'value > 0' to 'value != 0'
        cm.Advance(-5)
            .RemoveInstruction()
            .SetOpcodeAndAdvance(OpCodes.Brfalse_S);

        return cm.InstructionEnumeration();
    }

    private static Int32 ChangeCurrency(Int32 currencyGain)
    {
        int amount = currencyGain + GrabGamblerHandler.instance.GetAmount();
        if (amount > 0)
        {
            EPCDKLCABNC.PGIEGPGNBHJ(amount); //  Progress.AddCurrency
        }
        else
        {
            EPCDKLCABNC.ICHBCIDNDJA(amount); //  Progress.SubtractCurrency
        }

        MainMoreSparks.Logger.LogWarning($"{amount} = {currencyGain} + {GrabGamblerHandler.instance.GetAmount()}");
        return amount;
    }

    [HarmonyPatch(typeof(CPNJEILDILH), nameof(CPNJEILDILH.PEEORANRAKAK), MethodType.Enumerator)]
    [HarmonyTranspiler]
    [HarmonyDebug]
    private static IEnumerable<CodeInstruction> ModifyResultScreenCurrencyAnim(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);

        FieldInfo field = cm.SearchForward(il => il.opcode == OpCodes.Ldfld && (il.operand as FieldInfo).Name == "IBNCFEOHBEP")
            .ThrowIfNotMatch("Didn't find 'IBNCFEOHBEP'").Operand as FieldInfo;

        cm.SearchForward(il => il.opcode == OpCodes.Ldstr && il.operand as string == "+")
            .ThrowIfNotMatch("Didn't find '+'");

        cm.SetInstruction(
            Transpilers.EmitDelegate(delegate (Int32 currencyGain)
            {
                return currencyGain > 0 ? "+" : string.Empty;
            }))
            .Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, field),
            new CodeInstruction(OpCodes.Ldfld, field.FieldType.GetField("currencyGain")),
            new CodeInstruction(OpCodes.Call, typeof(ObscuredInt).GetMethod("op_Implicit", new Type[] { typeof(ObscuredInt) }))
            );

        cm.SearchBack(il => il.opcode == OpCodes.Ble)
            .Advance(-1)
            .RemoveInstruction()
            .SetOpcodeAndAdvance(OpCodes.Brfalse_S);

        cm.SearchForward(il => il.opcode == OpCodes.Call && (il.operand as MethodBase).Name == nameof(CPNJEILDILH.PEORFKFKGGGG))
            .ThrowIfNotMatch("Didn't find 'CAddCurrency'"); // Breaks here not sure why??

        cm.Set(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => CChangeCurrency(null, default, default, default, default)));

        cm.SearchBack(il => il.opcode == OpCodes.Ble)
            .SearchBack(il => il.opcode == OpCodes.Ldfld && (il.operand as FieldInfo).Name == nameof(CPNJEILDILH.currencyGain))
            .ThrowIfNotMatchBack("Didn't find 'currencyGain'");

        cm.Advance(2)
            .RemoveInstruction()
            .SetOpcodeAndAdvance(OpCodes.Brfalse_S);

        cm.Advance(1)
            .RemoveInstructions(2);

        return cm.InstructionEnumeration();
    }

    private const float CURRENCY_DURATION = 250;

    private static IEnumerator CChangeCurrency(PostScreen postScreen, bool skipWin, int currencyPrev, int gainPrev, bool pLevelUp)
    {
        TMP_Text lbLvlUpBonus = postScreen.lbLvlUpBonus;
        TMP_Text lbCurrency = postScreen.lbCurrency;
        TMP_Text lbCurrencyGain = postScreen.lbCurrencyGain;
        if (gainPrev < 0)
            lbCurrencyGain.color = Color.cyan;
        yield return new WaitForSeconds(0.2f);
        bool skipped = UIInput.HasClicked(Controller.all, false, false);
        yield return new WaitForSeconds(1f);

        MainMoreSparks.Logger.LogWarning($"\t[{Time.realtimeSinceStartup}] PrevCurrency: {currencyPrev} : {gainPrev}");
        int gain = gainPrev;
        int currency = currencyPrev;

        #region LevelUpBonus

        if (pLevelUp)
        {
            int g = 0;
            for (int lvlBonus = 80; lvlBonus >= 0; lvlBonus--)
            {
                lbLvlUpBonus.text = TextHandler.Get("RESULT_LEVELUP_BONUS", new string[0]) + (lvlBonus);
                lbCurrency.text = (g + currency).ToString();
                skipped = skipped || UIInput.HasClicked(Controller.all, false, false);

                if (lvlBonus <= 0)
                {
                    lbLvlUpBonus.gameObject.SetActive(false);

                    if (skipped == false)
                        yield return new WaitForSeconds(0.5f);
                }

                if (skipped == false)
                {
                    AudioHandler.PlaySfx(Sfx.RESULT_CURRENCY_GAIN);
                    yield return new WaitForSeconds(0.009f);
                }
                g++;
            }
            gain -= 80;
        }

        #endregion LevelUpBonus

        if (gainPrev > 0)
        {
            MainMoreSparks.Logger.LogWarning($"ADD: {currencyPrev} <= {currencyPrev + gainPrev}");
            for (; currency <= currencyPrev + gainPrev; currency++)
            {
                if (currency % 64 == 0 || currency == currencyPrev + gainPrev)
                {
                    if (gain > 0)
                    {
                        lbCurrencyGain.text = "+" + gain.ToString();
                    }
                    else
                    {
                        lbCurrencyGain.gameObject.SetActive(false);
                    }

                    lbCurrency.text = currency.ToString();
                    skipped = skipped || UIInput.HasClicked(Controller.all, false, false);

                    if (skipped == false)
                    {
                        AudioHandler.PlaySfx(Sfx.RESULT_CURRENCY_GAIN);
                        yield return new WaitForSeconds(0.009f);
                    }
                    MainMoreSparks.Logger.LogWarning($"Cur: {currency}");
                }
                gain--;
            }
        }
        else if (gainPrev < 0)
        {
            // SubtractCurrency()
            MainMoreSparks.Logger.LogWarning($"SUBTRACT: {currencyPrev} >= {currencyPrev + gainPrev}");
            for (; currency >= gainPrev + currencyPrev; currency--)
            {
                if (currency % 64 == 0 || currency == gainPrev + currencyPrev)
                {
                    if (gain < 0)
                    {
                        lbCurrencyGain.text = gain.ToString();
                    }
                    else
                    {
                        lbCurrencyGain.gameObject.SetActive(false);
                    }

                    lbCurrency.text = currency.ToString();
                    skipped = skipped || UIInput.HasClicked(Controller.all, false, false);

                    if (skipped == false)
                    {
                        AudioHandler.PlaySfx(Sfx.RESULT_CURRENCY_GAIN);
                        yield return new WaitForSeconds(0.009f);
                    }
                }
                gain++;
            }
        }

        AudioHandler.PlaySfx(Sfx.RESULT_CURRENCY_TOTAL);
        lbLvlUpBonus.gameObject.SetActive(false);
        lbCurrencyGain.gameObject.SetActive(false);
        MainMoreSparks.Logger.LogWarning($"\t[{Time.realtimeSinceStartup}]");
        yield break;
    }
}

#endif