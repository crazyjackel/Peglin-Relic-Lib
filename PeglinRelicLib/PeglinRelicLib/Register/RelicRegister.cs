using HarmonyLib;
using I2.Loc;
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
    public static class RelicRegister
    {
        public static List<RelicEffect> RegisteredRelicEffects => m_registeredEffects.Values.Concat(m_default.Cast<RelicEffect>()).ToList();

        public const string DefaultBundleName = "relic";
        public const string DefaultSpriteName = "knife";
        static AssetBundle m_defaultBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.s_Path, DefaultBundleName));
        static Sprite m_defaultSprite = m_defaultBundle.LoadAsset<Sprite>(DefaultSpriteName);

        static Array m_default = Enum.GetValues(typeof(RelicEffect));
        static int m_pointer = m_default.Cast<int>().Min() - 1;
        static RelicManager m_relicManager;
        static Queue<RelicPoolData> m_relicPool = new Queue<RelicPoolData>();
        static Dictionary<RelicEffect, Relic> m_customRelics = new Dictionary<RelicEffect, Relic>();
        static Dictionary<string, RelicEffect> m_registeredEffects = new Dictionary<string, RelicEffect>();
        static Dictionary<string, RelicEffect> m_reservedEffects = new Dictionary<string, RelicEffect>();

        public static RelicEffect GetCustomRelicEffect(string GUID)
        {
            if (m_registeredEffects.TryGetValue(GUID, out RelicEffect value))
            {
                return value;
            }
            else
            {
                try
                {
                    return (RelicEffect)Enum.Parse(typeof(RelicEffect), GUID);
                }
                catch (Exception)
                {

                }
            }
            return RelicEffect.NONE;
        }

        public static Relic GetCustomRelic(RelicEffect effect)
        {
            return m_customRelics[effect];
        }

        internal static void LoadFromConfig()
        {
            string reserved = Plugin.reserveInfo.Value;
            m_reservedEffects = JsonUtility.FromJson<Dictionary<string, RelicEffect>>(reserved);
        }

        internal static void SaveToConfig()
        {
            string reserve = JsonUtility.ToJson(m_reservedEffects);
            Plugin.reserveInfo.Value = reserve;
        }
        internal static void InitializeRelicsIntoPool(RelicManager relicManager)
        {
            m_relicManager = relicManager;
            while (m_relicPool.Count > 0)
            {
                RelicPoolData data = m_relicPool.Dequeue();
                AddRelicToPool(data, true);
            }
        }
        public static RelicEffect RegisterRelic(RelicDataModel relicData)
        {
            Relic relic = ScriptableObject.CreateInstance<Relic>();
            relic.locKey = relicData.LocalKey;
            relic.descMod = relicData.DescriptionKey;

            LoadRelicAssets(relic, relicData);
            AddRelicToPool(new RelicPoolData()
            {
                rarity = relicData.Rarity,
                relic = relic,
                AddToPool = relicData.AddToPool
            });

            //Man, I do not get to use Do Whiles, like ever.
            do
            {
                relic.effect = (RelicEffect)m_pointer--;
            } while (!m_reservedEffects.Values.Contains(relic.effect));

            if (relic.effect > RelicEffect.NONE) Debug.LogWarning("Relic Value Overflow");
            Debug.Log($"Registering {relicData.GUID} to {(int)relic.effect}");
            m_customRelics.Add(relic.effect, relic);
            m_registeredEffects[relicData.GUID] = relic.effect;
            return relic.effect;
        }
        public static void UnregisterRelic(string GUID)
        {
            if(m_registeredEffects.TryGetValue(GUID, out RelicEffect effect))
            {
                while (m_relicManager?.RelicEffectActive(effect) ?? false)
                {
                    m_relicManager?.RemoveRelic(effect);
                }
                m_registeredEffects.Remove(GUID);
                m_reservedEffects.Remove(GUID);
                m_customRelics.Remove(effect);
            }
        }
        private static void LoadRelicAssets(Relic relic, RelicDataModel data)
        {
            relic.sprite = data.RelicSprite ?? m_defaultSprite;
            relic.useSfx = data.RelicAudio;

            if (data.FullPath == null || data.FullPath == "") return;

            AssetBundle bundle = AssetBundle.LoadFromFile(data.FullPath);
            if (bundle == null)
            {
                Debug.LogWarning("Bundle Not Found");
                return;
            }

            if(data.SpriteName != null)
            {
                Sprite spr = bundle.LoadAsset<Sprite>(data.SpriteName);
                if (spr != null) relic.sprite = spr;
            }
            if(data.AudioName != null)
            {
                AudioClip clip = bundle.LoadAsset<AudioClip>(data.AudioName);
                if (clip != null) relic.useSfx = clip;
            }
            bundle.Unload(false);
        }

        private static void AddRelicToPool(RelicPoolData data, bool noOverflow = false)
        {
            if (m_relicManager == null && !noOverflow)
            {
                m_relicPool.Enqueue(data);
                return;
            }

            RelicSet set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_rareScenarioRelics").GetValue(m_relicManager);
            if (data.AddToPool)
            {
                switch (data.rarity)
                {
                    case RelicRarity.COMMON:
                        set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_commonRelicPool").GetValue(m_relicManager);
                        break;
                    case RelicRarity.RARE:
                        set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_rareRelicPool").GetValue(m_relicManager);
                        break;
                    case RelicRarity.BOSS:
                        set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_bossRelicPool").GetValue(m_relicManager);
                        break;
                    case RelicRarity.NONE:
                    default:
                        break;
                }
            }

            List<Relic> relics = (List<Relic>)AccessTools.Field(typeof(RelicSet), "_relics").GetValue(set);
            relics.Add(data.relic);
        }

        private class RelicPoolData
        {
            public RelicRarity rarity { get; set; }
            public Relic relic { get; set; }
            public bool AddToPool { get; set; }
        }
    }
}
