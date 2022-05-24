using PeglinRelicLib.Interfaces;
using PeglinRelicLib.Utility;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeglinRelicLib.Register
{
    public abstract class Register<TSelf,TEnum> : IRegister<TEnum> where TSelf : Register<TSelf, TEnum>, new() where TEnum : struct, Enum
    {
        protected static TSelf Instance { get; } = new TSelf();
        public abstract string SaveID { get; }
        public List<TEnum> Values => m_values;

        protected readonly Dictionary<string, TEnum> m_registered;
        protected SerializableDictionary<string, TEnum> m_reserved;
        protected readonly List<TEnum> m_default;
        protected readonly List<TEnum> m_values;

        protected Register() 
        {
            m_registered = new Dictionary<string, TEnum>();
            m_reserved = new SerializableDictionary<string, TEnum>();
            m_default = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            m_values = new List<TEnum>(m_default);
        }

        public bool TryGetRegisteredValue(string GUID, out TEnum @enum)
        {
            if (m_registered.TryGetValue(GUID, out TEnum value))
            {
                @enum = value;
                return true;
            }
            @enum = default;
            return false;
        }
        public bool RegisterValue(IModel<TEnum> model, out TEnum @enum)
        {
            if (m_registered.ContainsKey(model.GUID)) throw new ArgumentException($"Model GUID ({model.GUID}) already registered.");

            if (!LoadModel(model, out object args))
            {
                @enum = default;
                return false;
            }
            TEnum calcEnum = CalculateEnum(model.GUID); 
            m_registered[model.GUID] = calcEnum;
            m_values.Add(calcEnum);
            FinalizeModel(model, calcEnum, args);

            @enum = calcEnum;
            return true;
        }
        public void ReleaseValue(string GUID)
        {
            if (m_registered.TryGetValue(GUID, out TEnum @enum))
            {
                UnloadEnum(@enum);
                m_registered.Remove(GUID);
                m_reserved.Remove(GUID);
            }
        }

        protected abstract TEnum CalculateEnum(string GUID);
        protected abstract bool LoadModel(IModel<TEnum> model, out object args);
        protected virtual void UnloadEnum(TEnum @enum) { }
        protected virtual void FinalizeModel(IModel<TEnum> model, TEnum @enum, object args) { }


        public void Save()
        {
            ModdedDataSerializer.Save(SaveID, m_reserved);
        }
        public void Load()
        {
            var reserved = ModdedDataSerializer.Load<SerializableDictionary<string, TEnum>>(SaveID);
            if (reserved != null) m_reserved = reserved;
        }
        public void Reset()
        {
            m_reserved.Clear();
            foreach (var pair in m_registered)
            {
                m_reserved.Add(pair.Key, pair.Value);
            }
        }

    }
}
