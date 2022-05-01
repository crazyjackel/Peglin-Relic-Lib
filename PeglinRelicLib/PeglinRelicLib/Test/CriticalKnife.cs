using Battle;
using HarmonyLib;
using PeglinRelicLib.Register;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinRelicLib.Test
{
    [HarmonyPatch(typeof(Attack), "GetModifiedDamagePerPeg")]
    public class CriticalKnifeEffect
    {
        public static void Postfix(Attack __instance, int critCount, ref float __result)
        {
            if (!Plugin.enableTestItem.Value) return;
            RelicManager relicManager = (RelicManager)AccessTools.Field(typeof(Attack), "_relicManager").GetValue(__instance);

            if (relicManager.RelicEffectActive(RelicRegister.GetCustomRelicEffect("io.github.crazyjackel.criticalknife")) && critCount > 0)
            {
                __result += 2f;
            }
        }
    }
}
