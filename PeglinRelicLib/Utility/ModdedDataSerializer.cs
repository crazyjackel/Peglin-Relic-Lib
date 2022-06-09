using HarmonyLib;
using Peglin.OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBox.Serialization;
using UnityEngine;

namespace PeglinRelicLib.Utility
{
	/// <summary>
	/// Port of DataSerializer from base Game. Used for dealing with Steam Auto-Saves and Maintaining File Security
	/// </summary>
    public static class ModdedDataSerializer
	{
		private static Dictionary<string, ISerializable> _data;
		private static bool _isSetup = false;

		internal static void Setup()
		{
			if (_isSetup) return;
			LoadFile(0);
			_isSetup = true;
			Application.quitting += () => SaveFile((int)(AccessTools.Field(typeof(DataSerializer), "_currentProfileIndex").GetValue(null) ?? 0));
		}

		public static void Save<T>(string key, T dataToSave)
		{
			ISerializable serializable;
			if (_data.TryGetValue(key, out serializable))
			{
				((Item<T>)serializable).Value = dataToSave;
				return;
			}
			Item<T> value = new Item<T>
			{
				Value = dataToSave
			};
			_data.Add(key, value);
		}
		public static void SaveFile(int profileIndex)
		{
			string filePath = GetFilePath(profileIndex);
			byte[] bytes = SerializationUtility.SerializeValue(_data, DataFormat.Binary, null);
			File.WriteAllBytes(filePath, bytes);
		}

		public static T Load<T>(string key)
		{
			ISerializable serializable;
            _data.TryGetValue(key, out serializable);
			Item<T> item = (Item<T>)serializable;
			if (item != null)
			{
				return item.Value;
			}
			return default(T);
		}
		internal static void LoadFile(int profile)
		{
			string filePath = GetFilePath(profile);
			if (!File.Exists(filePath))
			{
				FileStream fileStream = File.Create(filePath);
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			byte[] bytes = File.ReadAllBytes(filePath);
			try
			{
				_data = SerializationUtility.DeserializeValue<Dictionary<string, ISerializable>>(bytes, DataFormat.Binary, null);
				if (_data == null)
				{
					_data = new Dictionary<string, ISerializable>(64);
				}
			}
			catch (InvalidOperationException ex)
			{
				Plugin.Log(BepInEx.Logging.LogLevel.Warning, ex.Message);
				string destFileName = filePath + "_bak" + DateTime.Now.ToString("yyyyMMddHHmmss");
				File.Move(filePath, destFileName);
			}
		}
		public static bool HasKey(string key)
		{
			return !string.IsNullOrEmpty(key) && _data.ContainsKey(key);
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00018704 File Offset: 0x00016904
		public static void DeleteKey(string key)
		{
			if (DataSerializer.HasKey(key))
			{
				_data.Remove(key);
			}
		}
		private static string GetFilePath(int profileIndex)
		{
			return Path.Combine(Application.persistentDataPath, string.Format("{0}_{1}.txt", "ModdedSave", profileIndex));
		}
	}
}
