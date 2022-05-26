using HarmonyLib;
using PeglinRelicLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.Serialization;

namespace PeglinRelicLib.Patch
{
    [HarmonyPatch(typeof(DataSerializer), "SaveFile")]
    public class ModdedSaveFilePatch
    {
        static void Postfix(int ____currentProfileIndex)
        {
            ModdedDataSerializer.SaveFile(____currentProfileIndex);
        }
    }

    [HarmonyPatch(typeof(DataSerializer), "LoadFile")]
    public class ModdedLoadFilePatch
    {
        static void Postfix(int ____currentProfileIndex)
        {
            ModdedDataSerializer.LoadFile(____currentProfileIndex);
        }
    }
}
