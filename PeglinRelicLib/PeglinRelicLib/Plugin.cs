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

namespace PeglinRelicLib
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "io.github.crazyjackel.RelicLib";
        public const string Name = "crazyjackel";
        public const string Version = "1.0.0";

        static string m_path;
        static int m_rg = Enum.GetValues(typeof(RelicEffect)).Cast<int>().Max();
        static Plugin m_lib;
        static RelicManager m_relicManager;
        public static Plugin RelicLib => m_lib;
        public static string BasePath => m_path;
        static Plugin()
        {
            //Calculate out a BasePath
            var assembly = typeof(Plugin).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            m_path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
        void Awake()
        {
            if (m_lib != null) Destroy(this);

            m_lib = this;
            m_relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().First();

            if (m_relicManager == null)
            {
                Debug.Log("Relic Manager Not Found");
                return;
            }


            Harmony patcher =  new Harmony(GUID);
            patcher.PatchAll();
        }


        public RelicEffect RegisterRelic(RelicDataModel relicData)
        {
            Relic r = ScriptableObject.CreateInstance<Relic>();
            r.locKey = relicData.LocalKey;
            r.descMod = relicData.DescriptionKey;

            return (RelicEffect)m_rg++;
        }
    }
}
