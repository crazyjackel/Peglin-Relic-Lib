using I2.Loc;
using PeglinRelicLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinRelicLib.Utility
{
    public static class LocalizationHelper
    {
        public static void ImportTerm(params TermDataModel[] models)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Key,Type,Desc,English,Français [fr],Español [es],Deutsch [de],Nederlands [nl],Italiano [it],Português do Brasil [pt-BR],Русский [ru],简体中文 [zh-CN],繁体中文 [zh-TW],日本語 [ja],한국어 [ko],Svenska [sv],Polski [pl],Türkçe [tr]\n");

            int appendcount = 0;
            foreach (TermDataModel model in models)
            {
                if (string.IsNullOrEmpty(model.Key)) continue;
                if (!LocalizationManager.TryGetTranslation(model.Key, out _))
                {
                    Plugin.Log(BepInEx.Logging.LogLevel.Info, $"Adding Translation for {model.Key}");
                    builder.Append(model.GetCSVLine());
                    appendcount++;
                }
            }
            if (appendcount == 0) return;

            List<string> categories = LocalizationManager.Sources[0].GetCategories(true);
            foreach (string category in categories)
            {
                LocalizationManager.Sources[0].Import_CSV(category, builder.ToString(), eSpreadsheetUpdateMode.AddNewTerms, ',');
            }
            LocalizationManager.LocalizeAll();
        }
    }
}
