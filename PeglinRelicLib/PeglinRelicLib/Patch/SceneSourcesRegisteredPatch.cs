using HarmonyLib;
using I2.Loc;
using PeglinRelicLib.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Patch
{
    [HarmonyPatch(typeof(LocalizationManager), "RegisterSceneSources")]
    public class SceneSourcesRegisteredPatch
    {
        public static void Postfix()
        {
        }
    }
}
