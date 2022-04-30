﻿using BepInEx;
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
        public const string Name = "crazyjackel";
        public const string Version = "1.0.0";

        static string m_path;
        static Plugin m_plugin;
        public static Plugin s_Plugin => m_plugin;
        public static string s_Path => m_path;

        internal static ConfigEntry<bool> enableTestItem;

        static Plugin()
        {
            //Calculate out a BasePath
            var assembly = typeof(Plugin).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            m_path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
        void Awake()
        {
            if (m_plugin != null) Destroy(this);
            m_plugin = this;

            enableTestItem = Config.Bind("EnableTestItem", "Have Test Item for Relic Lib", false);

            Harmony patcher = new Harmony(GUID);
            patcher.PatchAll();

            if (enableTestItem.Value)
            {
                RelicEffect effect = RelicRegister.Instance.RegisterRelic(new RelicDataModel()
                {
                    Rarity = RelicRarity.COMMON,
                    AssemblyPath = m_path,
                    BundlePath = "relic",
                    SpriteName = "knife",
                    LocalKey = "knifeCrit",
                    GUID = "io.github.crazyjackel.criticalknife",
                });

                //The Effect is randomly Assigned, However GUID can be used to access effect after it has been registered.
                RelicEffect effect2 = RelicRegister.Instance["io.github.crazyjackel.criticalknife"];
            }
        }
    }
}