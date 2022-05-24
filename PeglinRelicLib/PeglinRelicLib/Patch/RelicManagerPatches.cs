using HarmonyLib;
using PeglinRelicLib.Register;
using PeglinRelicLib.Utility;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinRelicLib.Patch
{
    [HarmonyPatch(typeof(GameInit), "Start")]
    public class RelicManagerInstancePatch
    {
        public static void Postfix(RelicManager ____relicManager)
        {
            if (!Plugin.enableTestItem.Value) return;

            if (!RelicRegister.TryGetCustomRelic("io.github.crazyjackel.criticalknife", out Relic relic)) return;

            if (relic == null) return;

            ____relicManager.AddRelic(relic);
        }
    }
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(RelicManager), "Reset")]
    public class RelicManagerResetPatch
    {
        public static void Prefix(RelicManager __instance)
        {
            //Register Relics
            RelicRegister.InitializeRelicsIntoPool(__instance);
        }
    }
}
