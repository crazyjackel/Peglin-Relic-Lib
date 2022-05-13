using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.IO;
using UnityEngine;
using Relics;
using I2.Loc;
using PeglinRelicLib.Model;
using PeglinRelicLib.Register;
using BepInEx.Configuration;

namespace PeglinRelicLib
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "io.github.crazyjackel.RelicLib";
        public const string Name = "Relic Lib";
        public const string Version = "1.0.3";

        static string m_path;
        static Plugin m_plugin;
        public static Plugin s_Plugin => m_plugin;
        public static string s_Path => m_path;

        public static ConfigEntry<bool> enableTestItem;
        internal static ConfigEntry<int> debugLog;
        internal static ConfigEntry<string> reserveInfo;

        static Plugin()
        {
            //Calculate out a BasePath
            var assembly = typeof(Plugin).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            m_path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }

        public static void Log(LogType type, string arg)
        {
            //Get Int from BitShift
            int selector = debugLog.Value;
            int type_int = 1 << (int)type;
            //Check if DebugLog has flag
            //Example:
            // 10101 selector
            // 00100 type_int
            // 00100 result
            // true
            //
            // 10101 selector
            // 00010 type_int
            // 00000 result
            // false
            // If and operator does not change, then we have flag or flag combo.
            if((selector & type_int) == type_int)
            {
                Debug.unityLogger.Log(type, arg);
            }
        }

        void Awake()
        {
            if (m_plugin != null) Destroy(this);
            m_plugin = this;

            enableTestItem = Config.Bind("EnableTestItem", "Have Test Item for Relic Lib", false);
            debugLog = Config.Bind("Config Output", "Flag for Output of Logs, (0 - No Logging, 1 - Only Errors, 4 - Only Warnings, 5 - Errors and Warnings, 31 - All Logs)", 1);
            reserveInfo = Config.Bind("Reserve Data", "Do not Touch", "");

            RelicRegister.LoadFromConfig();

            Harmony patcher = new Harmony(GUID);
            patcher.PatchAll();


            if (enableTestItem.Value)
            {
                RelicDataModel model = new RelicDataModel("io.github.crazyjackel.criticalknife")
                {
                    Rarity = RelicRarity.COMMON,
                    /*
                    AssemblyPath = m_path,
                    BundlePath = "relic",
                    SpriteName = "knife",
                    */
                    LocalKey = "knifeCrit",
                    DescriptionKey = "1"
                };

                RelicRegister.RegisterRelic(model);

                LocalizationRegister.ImportTerm(new TermDataModel(model.NameTerm)
                {
                    English = "Critical Knife",
                    French = "Dague Pointue"
                });

                LocalizationRegister.ImportTerm(new TermDataModel(model.DescriptionTerm)
                {
                    English = "All attacks get <style=dmg_bonus>+0 /+2</style>.",
                    French = "Toutes les attaques gagnent <style=dmg_bonus>+0 /+2</style>."
                });
            }
        }
    }
}
