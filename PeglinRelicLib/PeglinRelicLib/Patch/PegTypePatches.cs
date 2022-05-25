using Battle;
using HarmonyLib;
using PeglinRelicLib.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Peg;

namespace PeglinRelicLib.Patch
{
    [HarmonyPatch(typeof(PegManager), "GetPegCount")]
    public class GetPegCountPatch
    {
        static void Postfix(PegManager __instance, Peg.PegType type, ref int __result)
        {
            if (!PegTypeRegister.IsCustomPegType(type)) return;

            __result = PegTypeRegister.GetPegCount(__instance, type);
        }
    }

    [HarmonyPatch(typeof(PegManager), "ShuffleSpecialPegs")]
    public class OnRefreshPegPatch
    {
        static void Postfix(PegManager __instance)
        {
            foreach (PegType peg in PegTypeRegister.GetPegTypesHit())
            {
                PegTypeRegister.ShufflePegTypes(__instance, peg);
            }
        }
    }

    [HarmonyPatch]
    public class ConvertToPegTypePatch
    {
        [HarmonyTargetMethods]
        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in PegTypeRegister.GetTypesOfPegs())
            {
                var method = type.GetMethod("ConvertPegToType");
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RemoveWarnings(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(AccessTools.Method(typeof(Debug), "LogWarning", new Type[] { typeof(object) }), AccessTools.Method(typeof(ConvertToPegTypePatch), "DoNothing"));
        }
        public static void DoNothing(object message) { }
        public static void Postfix(Peg __instance, Peg.PegType type)
        {
            MethodInfo method = typeof(PegTypeRegister).GetMethod(nameof(PegTypeRegister.OnConvertToPegType), BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo generic = null;
            try
            {
                generic = method.MakeGenericMethod(__instance.GetType());
                generic.Invoke(null, new object[] { __instance, type });
            }
            catch (Exception e)
            {
                Plugin.Log(BepInEx.Logging.LogLevel.Error, $"PegTypePatches: {e.Message}");
            }
            __instance.pegType = type;
        }
    }
    [HarmonyPatch]
    public class SupportsPegTypePatch
    {
        [HarmonyTargetMethods]
        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in PegTypeRegister.GetTypesOfPegs())
            {
                var method = type.GetMethod("SupportsPegType");
                if (method != null)
                {
                    yield return method;
                }
            }
        }
        static void Postfix(Peg __instance, Peg.PegType pType, ref bool __result)
        {
            if (__result == false)
            {
                __result = PegTypeRegister.Supports(__instance.GetType(), pType);
            }
        }
    }
}
