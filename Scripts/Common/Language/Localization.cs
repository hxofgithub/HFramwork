using System.Collections.Generic;
using UnityEngine;

namespace HFramework.ILocalization
{
    using Core.Delegates;

    public static class Localization
    {
        public static DelegateAction OnLocalization;

        public static void SetDefaultLanguage()
        {
            MarkLanguageDirty(PlayerPrefs.GetString("Localization", "Chinese"));
        }

        public static void SetLanguage(string language)
        {       
            MarkLanguageDirty(language);
        }

        public static string Get(string key, bool warningIfMiss = true)
        {
            if (_languageDict.ContainsKey(key))
                return _languageDict[key];

            #if UNITY_EDITOR
            if (warningIfMiss)
                Log.Error("[Localization]Error Key:{0}", key);
            #endif
            return key;
        }

        private static void MarkLanguageDirty(string newLanguage)
        {
            if (mLanguage.Equals(newLanguage))
                return;
            TextAsset asset = Resources.Load<TextAsset>(newLanguage); 
            Set(newLanguage, ParseTextToDict(asset.text));
        }

        private static void Set(string language, Dictionary<string,string> dict)
        {
            mLanguage = language;
            _languageDict = dict;
            PlayerPrefs.SetString("Localization", language);
            PlayerPrefs.Save();
            if (OnLocalization != null)
                OnLocalization();
        }


        private static Dictionary<string, string> ParseTextToDict(string content)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            char[] lineSeparator = new char[]{ '\r' };
            string[] lines = content.Replace("\n", string.Empty).Split(lineSeparator);

            char[] separator = new char[] { '=' };
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("//"))
                    continue;

                string[] colums = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (colums.Length == 2)
                    dict[colums[0].Trim()] = colums[1].Trim().Replace("\\n", "\n");                    
            }

            return dict;
        }

        private static string mLanguage = "";
        private static Dictionary<string, string> _languageDict = new Dictionary<string, string>();
    }
}