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
using PeglinRelicLib.Utility;
using BepInEx.Logging;

namespace PeglinRelicLib
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "io.github.crazyjackel.RelicLib";
        public const string Name = "Relic Lib";
        public const string Version = "2.0.0";

        static string m_path;
        static Plugin m_plugin;
        public static Plugin s_Plugin => m_plugin;
        public static string s_Path => m_path;

        public static ConfigEntry<bool> enableTestItem;
        internal static ConfigEntry<LogLevel> debugLog;

        static Plugin()
        {
            //Calculate out a BasePath
            var assembly = typeof(Plugin).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            m_path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }

        public static void Log(LogLevel type, string arg)
        {
            //Get Int from BitShift
            LogLevel selector = debugLog.Value;
            //Check if DebugLog has flag
            if(selector.HasFlag(type))
            {
                m_plugin.Logger.Log(type, arg);
            }
        }

        void Awake()
        {
            if (m_plugin != null) Destroy(this);
            m_plugin = this;

            //Get Config
            enableTestItem = Config.Bind("EnableTestItem", "Have Test Item for Relic Lib", false);
            debugLog = Config.Bind("Config Output", "Flag for Output of Logs", LogLevel.Error | LogLevel.Fatal);

            //Setup Data
            ModdedDataSerializer.Setup();
            
            //Load Data from Config
            RelicRegister.LoadConfig();
            PegTypeRegister.LoadConfig();

            //Do Patches
            Harmony patcher = new Harmony(GUID);
            patcher.PatchAll();

            //Do Subscription Test Item.
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

                RelicRegister.RegisterRelic(model, out _);

                LocalizationHelper.ImportTerm(new TermDataModel(model.NameTerm)
                {
                    English = "Critical Knife",
                    French = "Dague Pointue"
                });

                LocalizationHelper.ImportTerm(new TermDataModel(model.DescriptionTerm)
                {
                    English = "All attacks get <style=dmg_bonus>+0 /+2</style>.",
                    French = "Toutes les attaques gagnent <style=dmg_bonus>+0 /+2</style>."
                });
            }
        }
    }
}
