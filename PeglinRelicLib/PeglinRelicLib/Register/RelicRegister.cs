using HarmonyLib;
using I2.Loc;
using PeglinRelicLib.Interfaces;
using PeglinRelicLib.Model;
using PeglinRelicLib.Utility;
using Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.Serialization;
using UnityEngine;

namespace PeglinRelicLib.Register
{
    public class RelicRegister : Register<RelicRegister, RelicEffect>
    {
        public override string SaveID => "io.github.crazyjackel.relicRegister";

        public const string DefaultBundleName = "relic";
        public const string DefaultSpriteName = "knife";

        /// <summary>
        /// Default Bundle Location for Loading Default Assets
        /// </summary>
        readonly AssetBundle m_defaultBundle;
        /// <summary>
        /// Default Sprite for Relics without a set Sprite
        /// </summary>
        readonly Sprite m_defaultSprite;
        /// <summary>
        /// Queue For Data being added into the Pools
        /// </summary>
        readonly Queue<RelicPoolData> m_relicPool;
        /// <summary>
        /// Relic Manager Instance, We only Access it once the Game Starts. Use to Add Relics into Pools
        /// </summary>
        RelicManager m_relicManager;
        /// <summary>
        /// A Dictionary for Accessing Relics from their Effect
        /// </summary>
        readonly Dictionary<RelicEffect, Relic> m_customRelics;

        public int m_pointer;

        public RelicRegister()
        {
            m_defaultBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.s_Path, DefaultBundleName));
            m_relicPool = new Queue<RelicPoolData>();
            m_defaultSprite = m_defaultBundle.LoadAsset<Sprite>(DefaultSpriteName);
            m_pointer = m_default.Min(x => (int)x) - 1; 
            m_customRelics = new Dictionary<RelicEffect, Relic>();
        }

        #region Front End
        public static bool TryGetCustomRelicEffect(string GUID, out RelicEffect effect)
        {
            if (Instance.TryGetRegisteredValue(GUID, out RelicEffect @enum))
            {
                effect = @enum;
                return true;
            }
            effect = default;
            return false;
        }
        [Obsolete("Use TryGetCustomRelicEffect Instead")]
        public static RelicEffect GetCustomRelicEffect(string GUID)
        {
            return Instance.m_registered[GUID];
        }
        public static bool TryGetCustomRelic(string GUID, out Relic relic)
        {
            relic = null;

            if (!TryGetCustomRelicEffect(GUID, out RelicEffect effect)) return false;
            
            if (!TryGetCustomRelic(effect, out Relic rel)) return false;
            
            relic = rel;
            return true;

        }
        public static bool TryGetCustomRelic(RelicEffect effect, out Relic relic)
        {
            if(Instance.m_customRelics.TryGetValue(effect, out Relic val))
            {
                relic = val;
                return true;
            }
            relic = null;
            return false;
        }
        [Obsolete("Use Alternative Register Relic")]
        public static RelicEffect RegisterRelic(RelicDataModel relicData)
        {
            if (Instance.RegisterValue(relicData, out RelicEffect effect))
            {
                return effect;
            }
            return default;
        }
        public static bool RegisterRelic(RelicDataModel relicData, out RelicEffect @enum)
        {
            if (Instance.RegisterValue(relicData, out RelicEffect effect))
            {
                @enum = effect;
                Plugin.Log(BepInEx.Logging.LogLevel.Info, $"Registering {relicData.GUID} to {(int)@enum}");
                return true;
            }
            @enum = default;
            return false;
        }
        internal static void SaveConfig()
        {
            Instance.Save();
        }
        internal static void LoadConfig()
        {
            Instance.Load();
        }
        internal static void ResetConfig()
        {
            Instance.Reset();
        }
        internal static void InitializeRelicsIntoPool(RelicManager relicManager)
        {
            Instance.m_relicManager = relicManager;
            while (Instance.m_relicPool.Count > 0)
            {
                RelicPoolData data = Instance.m_relicPool.Dequeue();
                Instance.AddRelicToPool(data, true);
            }
        }
        #endregion

        protected override RelicEffect CalculateEnum(string GUID)
        {
            if (m_reserved.ContainsKey(GUID))
            {
                return m_reserved[GUID];
            }

            if (m_registered.ContainsKey(GUID))
            {
                return m_registered[GUID];
            }

            //Find an Open Spot
            while (m_values.Contains((RelicEffect)m_pointer))
            {
                m_pointer--;
            }
            return (RelicEffect)m_pointer;
        }

        protected override bool LoadModel(IModel<RelicEffect> model, out object args)
        {
            if (model is RelicDataModel relicData)
            {
                Relic relic = ScriptableObject.CreateInstance<Relic>();
                relic.locKey = relicData.LocalKey;
                relic.descMod = relicData.DescriptionKey ?? "";
                relic.name = relicData.LocalKey;

                LoadRelicAssets(relic, relicData);
                AddRelicToPool(new RelicPoolData()
                {
                    Rarity = relicData.Rarity,
                    Relic = relic,
                    AddToPool = relicData.AddToPool
                });
                args = relic;
                return true;
            }
            args = default;
            return false;
        }
        private void LoadRelicAssets(Relic relic, RelicDataModel data)
        {
            relic.sprite = data.RelicSprite ?? m_defaultSprite;
            relic.useSfx = data.RelicAudio;

            if (data.AssemblyPath == null || data.FullPath == null || data.BundlePath == null) return;

            AssetBundle bundle = AssetBundle.LoadFromFile(data.FullPath);
            if (bundle == null)
            {
                Plugin.Log(BepInEx.Logging.LogLevel.Error, $"Bundle at ({data.FullPath}) was Not Found");
                return;
            }

            if (data.SpriteName != null)
            {
                Sprite spr = bundle.LoadAsset<Sprite>(data.SpriteName);
                if (spr != null) relic.sprite = spr;
            }
            if (data.AudioName != null)
            {
                AudioClip clip = bundle.LoadAsset<AudioClip>(data.AudioName);
                if (clip != null) relic.useSfx = clip;
            }
            bundle.Unload(false);
        }

        private void AddRelicToPool(RelicPoolData data, bool noOverflow = false)
        {
            if (m_relicManager == null && !noOverflow)
            {
                m_relicPool.Enqueue(data);
                return;
            }

            RelicSet set = (RelicSet)AccessTools.Field(typeof(RelicManager), "_rareScenarioRelics").GetValue(m_relicManager);
            if (data.AddToPool)
            {
                switch (data.Rarity)
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
            relics.Add(data.Relic);
        }
        protected override void FinalizeModel(IModel<RelicEffect> model, RelicEffect @enum, object args)
        {
            if(args is Relic relic)
            {
                relic.effect = @enum;
                m_customRelics.Add(@enum, relic);
            }
        }
        protected override void UnloadEnum(RelicEffect @enum)
        {
            while (m_relicManager?.RelicEffectActive(@enum) ?? false)
            {
                m_relicManager?.RemoveRelic(@enum);
            }
            m_customRelics.Remove(@enum);
        }

        private class RelicPoolData
        {
            public RelicRarity Rarity { get; set; }
            public Relic Relic { get; set; }
            public bool AddToPool { get; set; }
        }
    }
    /*
    public static class RelicRegister
    {
        #region Properties
        [Obsolete("Use AllRelicEffects Instead")]
        public static List<RelicEffect> RegisteredRelicEffects => m_registeredEffects.Values.Concat(m_default).ToList();
        /// <summary>
        /// A List of All Available Relic Effects, including Default Ones
        /// </summary>
        public static List<RelicEffect> AllRelicEffects => m_AllRelicEffects;
        #endregion

        #region Variables
        public const string DefaultBundleName = "relic";
        public const string DefaultSpriteName = "knife";

        /// <summary>
        /// Default Bundle Location for Loading Default Assets
        /// </summary>
        static readonly AssetBundle m_defaultBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.s_Path, DefaultBundleName));
        /// <summary>
        /// Default Sprite for Relics without a set Sprite
        /// </summary>
        static readonly Sprite m_defaultSprite = m_defaultBundle.LoadAsset<Sprite>(DefaultSpriteName);
        /// <summary>
        /// A Default List of Relics
        /// </summary>
        static readonly List<RelicEffect> m_default = Enum.GetValues(typeof(RelicEffect)).Cast<RelicEffect>().ToList();
        /// <summary>
        /// Queue For Data being added into the Pools
        /// </summary>
        static readonly Queue<RelicPoolData> m_relicPool = new Queue<RelicPoolData>();
        /// <summary>
        /// A Dictionary for Accessing Relics from their Effect
        /// </summary>
        static readonly Dictionary<RelicEffect, Relic> m_customRelics = new Dictionary<RelicEffect, Relic>();
        /// <summary>
        /// A Dictionary for Accessing a Relic Effect from a registration GUID
        /// </summary>
        static readonly Dictionary<string, RelicEffect> m_registeredEffects = new Dictionary<string, RelicEffect>();
        /// <summary>
        /// A List of All Available Relic Effects, including Default Ones
        /// </summary>
        static readonly List<RelicEffect> m_AllRelicEffects = new List<RelicEffect>(m_default);

        /// <summary>
        /// A Pointer set to the minimum value of registered Relics by default
        /// </summary>
        static int m_pointer = m_default.Min(x => (int)x);
        /// <summary>
        /// Relic Manager Instance, We only Access it once the Game Starts. Use to Add Relics into Pools
        /// </summary>
        static RelicManager m_relicManager;
        /// <summary>
        /// A Dictionary of Reserved Serialized Effects
        /// </summary>
        static SerializableDictionary<string, RelicEffect> m_reservedEffects = new SerializableDictionary<string, RelicEffect>();
        #endregion

        #region Functions
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
        public static RelicEffect RegisterRelic(RelicDataModel relicData)
        {
            Relic relic = ScriptableObject.CreateInstance<Relic>();
            relic.locKey = relicData.LocalKey;
            relic.descMod = relicData.DescriptionKey;

            LoadRelicAssets(relic, relicData);
            AddRelicToPool(new RelicPoolData()
            {
                Rarity = relicData.Rarity,
                Relic = relic,
                AddToPool = relicData.AddToPool
            });

            //Man, I do not get to use Do Whiles, like ever.
            if (m_reservedEffects.ContainsKey(relicData.GUID))
            {
                relic.effect = m_reservedEffects[relicData.GUID];
            }
            else
            {
                do
                {
                    m_pointer--;
                } while (m_reservedEffects.Values.Contains((RelicEffect)m_pointer));
                relic.effect = (RelicEffect)m_pointer;
                m_reservedEffects.Add(relicData.GUID, relic.effect);
            }


            if (relic.effect > RelicEffect.NONE) Plugin.Log(LogType.Error, "Relic Value Overflow, Things will Break.");
            Plugin.Log(LogType.Log, $"Registering {relicData.GUID} to {(int)relic.effect}");
            m_AllRelicEffects.Add(relic.effect);
            m_customRelics.Add(relic.effect, relic);
            m_registeredEffects[relicData.GUID] = relic.effect;
            return relic.effect;
        }
        public static void UnregisterRelic(string GUID)
        {
            if (m_registeredEffects.TryGetValue(GUID, out RelicEffect effect))
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

        internal static void SaveToConfig()
        {
            ModdedDataSerializer.Save("io.github.crazyjackel.reservedRelics", m_reservedEffects);
        }
        internal static void LoadFromConfig()
        {
            var reserved = ModdedDataSerializer.Load<SerializableDictionary<string, RelicEffect>>("io.github.crazyjackel.reservedRelics");
            if (reserved != null) m_reservedEffects = reserved;

            foreach (var relic in m_reservedEffects)
            {
                Plugin.Log(LogType.Log, $"Reserving {relic.Key} to {relic.Value}");
            }
        }
        internal static void ResetReserveEffects()
        {
            m_reservedEffects.Clear();
            foreach (var pair in m_registeredEffects)
            {
                m_reservedEffects.Add(pair.Key, pair.Value);
            }
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

        private static void LoadRelicAssets(Relic relic, RelicDataModel data)
        {
            relic.sprite = data.RelicSprite ?? m_defaultSprite;
            relic.useSfx = data.RelicAudio;

            if (data.AssemblyPath == null || data.FullPath == null || data.BundlePath == null) return;

            AssetBundle bundle = AssetBundle.LoadFromFile(data.FullPath);
            if (bundle == null)
            {
                Plugin.Log(LogType.Error, $"Bundle at ({data.FullPath}) was Not Found");
                return;
            }

            if (data.SpriteName != null)
            {
                Sprite spr = bundle.LoadAsset<Sprite>(data.SpriteName);
                if (spr != null) relic.sprite = spr;
            }
            if (data.AudioName != null)
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
                switch (data.Rarity)
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
            relics.Add(data.Relic);
        }
        #endregion

        private class RelicPoolData
        {
            public RelicRarity Rarity { get; set; }
            public Relic Relic { get; set; }
            public bool AddToPool { get; set; }
        }
    }*/
}
