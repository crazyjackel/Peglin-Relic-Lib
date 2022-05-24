using HarmonyLib;
using PeglinRelicLib.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Patch
{
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
                Plugin.Log(UnityEngine.LogType.Error, $"PegTypePatches: {e.Message}");
            }
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
