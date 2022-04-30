using HarmonyLib;
using PeglinRelicLib.Model;
using Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinRelicLib.Register
{
    public class RelicRegister
    {
        public static RelicRegister Instance { get; } = new RelicRegister();
        public static List<RelicEffect> RegisteredRelicEffects => m_relicEffects.Values.Concat(m_default.Cast<RelicEffect>()).ToList();

        public const string DefaultBundleName = "relic";
        public const string DefaultSpriteName = "knife";

        static Array m_default = Enum.GetValues(typeof(RelicEffect));
        static int m_pointer = m_default.Cast<int>().Min() - 1;
        static RelicManager m_relicManager;
        static Queue<RelicPoolData> m_relicPool = new Queue<RelicPoolData>();
        static Dictionary<RelicEffect, Relic> m_customRelics = new Dictionary<RelicEffect, Relic>();
        static Dictionary<string, RelicEffect> m_relicEffects = new Dictionary<string, RelicEffect>();
        public RelicEffect this[string GUID]
        {
            get
            {
                return m_relicEffects[GUID];
            }
        }

        public Relic GetCustomRelic(RelicEffect effect)
        {
            return m_customRelics[effect];
        }

        internal void InitializeRelicsIntoPool(RelicManager relicManager)
        {
            m_relicManager = relicManager;
            while(m_relicPool.Count > 0)
            {
                RelicPoolData data = m_relicPool.Dequeue();
                AddRelicToPool(data, true);
            }
        }

        public RelicEffect RegisterRelic(RelicDataModel relicData)
        {
            Relic relic = ScriptableObject.CreateInstance<Relic>();
            relic.locKey = relicData.LocalKey;
            relic.descMod = relicData.DescriptionKey;

            LoadRelicAssets(relic, relicData);
            AddRelicToPool(new RelicPoolData() 
            { 
                rarity = relicData.Rarity,
                relic = relic
            });
            LoadTranslationData(relicData);

            relic.effect = (RelicEffect)m_pointer--;
            m_customRelics.Add(relic.effect, relic);
            m_relicEffects[relicData.GUID] = relic.effect;
            return relic.effect;
        }


        public void UnregisterRelic(RelicEffect effect)
        {
            //Todo:
            //Remove RelicEffect from RelicPool
        }


        private void LoadRelicAssets(Relic relic, RelicDataModel data)
        {
            if (data.FullPath == null || data.FullPath == "") return;

            AssetBundle bundle = AssetBundle.LoadFromFile(data.FullPath);
            if (bundle == null)
            {
                Debug.LogWarning("Bundle Not Found");
                return;
            }

            if(data.SpriteName != null) relic.sprite = bundle.LoadAsset<Sprite>(data.SpriteName) ?? AssetBundle.LoadFromFile(Path.Combine(Plugin.s_Path, DefaultBundleName)).LoadAsset<Sprite>(DefaultSpriteName);
            if(data.AudioName != null) relic.useSfx = bundle.LoadAsset<AudioClip>(data.AudioName);
        }

        private void LoadTranslationData(RelicDataModel data)
        {

        }

        private void AddRelicToPool(RelicPoolData data, bool noOverflow = false)
        {
            if(m_relicManager == null && !noOverflow)
            {
                m_relicPool.Enqueue(data);
                return;
            }

            RelicSet set;
            switch (data.rarity)
            {
                case RelicRarity.NONE:
                    return;
                case RelicRarity.COMMON:
                    set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_commonRelicPool").GetValue(m_relicManager);
                    break;
                case RelicRarity.RARE:
                    set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_rareRelicPool").GetValue(m_relicManager);
                    break;
                case RelicRarity.BOSS:
                    set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_bossRelicPool").GetValue(m_relicManager);
                    break;
                default:
                    return;
            }

            List<Relic> relics = (List<Relic>)AccessTools.Field(typeof(RelicSet), "_relics").GetValue(set);
            relics.Add(data.relic);
        }

        private class RelicPoolData
        {
            public RelicRarity rarity { get; set; }
            public Relic relic { get; set; }
        }
    }
}
