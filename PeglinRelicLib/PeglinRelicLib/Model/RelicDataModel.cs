using BepInEx;
using Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Model
{
    public class RelicDataModel
    {
        /// <summary>
        /// Function to Assist in Setting Assembly Path
        /// </summary>
        /// <param name="plugin"></param>
        public void SetAssemblyPath(BaseUnityPlugin plugin)
        {
            var assembly = plugin.GetType().Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            AssemblyPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
        public string GUID { get; set; }
        public string AssemblyPath { get; set; }
        public string BundlePath { get; set; }
        public string SpriteName { get; set; }
        public string AudioName { get; set; }
        public string FullPath => Path.Combine(AssemblyPath, BundlePath);

        public RelicRarity Rarity { get; set; }
        public bool AddToPool { get; set; } = true;

        public string LocalKey { get; set; }
        public string DescriptionKey { get; set; }
    }
}
