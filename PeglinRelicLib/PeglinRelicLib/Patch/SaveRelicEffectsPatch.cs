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
    [HarmonyPatch(typeof(RelicManager),"SaveRelicData")]
    class SaveRelicEffectsPatch
    {
        public static void Prefix()
        {
            //We Save Our Config Whenever the Game Saves our Relic Data
            RelicRegister.SaveToConfig();
        }
    }

    [HarmonyPatch(typeof(GameInit), "Start")]
    class SaveAtGameInit
    {
        public static void Prefix(LoadMapData ___LoadData)
        {
            if (___LoadData.NewGame)
            {
                //If it is a New Game Clear out our buffer.
                //This means we will only expect additional relics needing to be reserved
                RelicRegister.ResetReserveEffects();
            }
            //We Save our Config whenever a New Game Begins
            RelicRegister.SaveToConfig();
        }
    }
}
