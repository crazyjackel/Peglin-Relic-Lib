using BepInEx;
using Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinRelicLib.Model
{
    public class RelicDataModel
    {

        public string GUID { get; set; }
        public string AssemblyPath { get; set; }
        public Sprite RelicSprite { get; set; }
        public AudioClip RelicAudio { get; set; }
        public string BundlePath { get; set; }
        public string SpriteName { get; set; }
        public string AudioName { get; set; }
        public RelicRarity Rarity { get; set; }
        public bool AddToPool { get; set; } = true;
        public string LocalKey { get; set; }
        public string DescriptionKey { get; set; }


        public string FullPath => Path.Combine(AssemblyPath, BundlePath);
        public string NameTerm => "Relics/" + LocalKey + "_name";
        public string DescriptionTerm => "Relics/" + LocalKey + "_desc" + DescriptionKey;

        public RelicDataModel(string GUID)
        {
            this.GUID = GUID;
        }
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
    }
}
