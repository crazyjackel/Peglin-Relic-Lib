using Battle;
using BepInEx.Bootstrap;
using HarmonyLib;
using PeglinRelicLib.Interfaces;
using PeglinRelicLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Peg;

namespace PeglinRelicLib.Register
{
    public class PegTypeRegister : Register<PegTypeRegister, PegType>
    {
        public override string SaveID => "io.github.crazyjackel.pegTypeRegister";
        public int m_pointer;

        private readonly HashSet<Type> PegTypes;
        public readonly Dictionary<Type, HashSet<PegType>> SupportedPairs;
        public readonly Dictionary<PegType, Dictionary<Type, object>> ConvertToPegActions;
        public readonly Dictionary<PegType, Func<PegManager, int>> GetPegCountActions;
        public readonly Dictionary<PegType, List<Peg>> PegsOfTypes;
        public PegTypeRegister()
        {
            PegTypes = new HashSet<Type>(FindPegTypesFromAssembly());
            SupportedPairs = new Dictionary<Type, HashSet<PegType>>();
            ConvertToPegActions = new Dictionary<PegType, Dictionary<Type, object>>();
            GetPegCountActions = new Dictionary<PegType, Func<PegManager, int>>();
            PegsOfTypes = new Dictionary<PegType, List<Peg>>();
            foreach (Type type in PegTypes)
            {
                SupportedPairs.Add(type, new HashSet<PegType>());
            }
        }

        #region Front End
        public static void ShufflePegTypes(PegManager pegManager, PegType type)
        {
            if (!Instance.PegsOfTypes.ContainsKey(type)) Instance.PegsOfTypes.Add(type, new List<Peg>());
            AccessTools.Method(typeof(PegManager), "ShufflePegs").Invoke(pegManager, new object[] { Instance.PegsOfTypes[type], type });
        }

        internal static List<PegType> GetPegTypesHit()
        {
            return Instance.GetPegCountActions.Keys.ToList();
        }
        internal static int GetPegHit(PegManager pegManager, PegType type)
        {
            if (Instance.GetPegCountActions.TryGetValue(type, out Func<PegManager, int> func))
            {
                return func(pegManager);
            }
            return 0;
        }
        internal static void OnConvertToPegType<T>(T peg, PegType type) where T : Peg
        {
            if (!Instance.ConvertToPegActions.TryGetValue(type, out Dictionary<Type, object> actions)) return;

            if (actions == null) return;

            if (!actions.TryGetValue(typeof(T), out object action)) return;

            if (action is Action<T> act)
            {
                act.Invoke(peg);
            }
        }
        public static bool IsCustomPegType(PegType type)
        {
            return Instance.m_registered.Values.Contains(type);
        }
        /// <summary>
        /// Get All Implementations Types of Abstract Peg. 
        /// </summary>
        /// <returns></returns>
        public static HashSet<Type> GetTypesOfPegs()
        {
            return new HashSet<Type>(Instance.PegTypes);
        }
        public static bool Supports(Type pegType, PegType type)
        {
            if (!Instance.SupportedPairs.TryGetValue(pegType, out HashSet<PegType> hash)) return false;

            if (hash == null) return false;

            return hash.Contains(type);
        }
        public static bool RegisterPegType(PegTypeDataModel model, out PegType @enum)
        {
            if (Instance.RegisterValue(model, out PegType effect))
            {
                @enum = effect;
                return true;
            }
            @enum = default;
            return false;
        }
        public static bool TryGetCustomPegType(string GUID, out PegType effect)
        {
            if (Instance.TryGetRegisteredValue(GUID, out PegType @enum))
            {
                effect = @enum;
                return true;
            }
            effect = default;
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
        #endregion

        protected override PegType CalculateEnum(string GUID)
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
            while (m_values.Contains((PegType)m_pointer))
            {
                m_pointer--;
            }
            return (PegType)m_pointer;
        }

        protected override bool LoadModel(IModel<PegType> model, out object args)
        {
            if (model is PegTypeDataModel datamodel)
            {
                args = null;
                return true;
            }
            args = null;
            return false;
        }
        protected override void FinalizeModel(IModel<PegType> model, PegType @enum, object args)
        {
            if (model is PegTypeDataModel dataModel)
            {
                foreach (Type type in dataModel.SupportTypes)
                {
                    if (!SupportedPairs.ContainsKey(type)) continue;

                    SupportedPairs[type]?.Add(@enum);
                }

                ConvertToPegActions.Add(@enum, dataModel.PegConversionAction);
                GetPegCountActions.Add(@enum, dataModel.GetPegCount);
            }
        }
        internal static IEnumerable<Type> FindPegTypesFromAssembly()
        {
            //Construct a List of Assemblies to Check for Pegs from.
            List<Assembly> checkAssemblies = new List<Assembly>() { typeof(Peg).Assembly };

            //Go through each Plugin to Check
            foreach (var pluginInfo in Chainloader.PluginInfos.Values)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(pluginInfo.Location);
                }
                catch (Exception)
                {
                    continue;
                }
                if (assembly == null) continue;
                checkAssemblies.Add(assembly);
            }

            //Go Through each Implementation and Decide to Patch that Function
            foreach (Assembly assemble in checkAssemblies)
            {
                if (assemble == null) continue;
                foreach (Type type in assemble.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Peg))))
                {
                    yield return type;
                }
            }
        }
    }
}
