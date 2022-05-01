using HarmonyLib;
using PeglinRelicLib.Register;
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
        public static void Prefix(RelicManager ____relicManager)
        {
            RelicRegister.InitializeRelicsIntoPool(____relicManager);
        }
        public static void Postfix(RelicManager ____relicManager)
        {
            if (!Plugin.enableTestItem.Value) return;
            Relic relic = RelicRegister.GetCustomRelic(RelicRegister.GetCustomRelicEffect("io.github.crazyjackel.criticalknife"));

            if (relic == null) return;
            ____relicManager.AddRelic(relic);
        }
    }
}
