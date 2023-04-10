using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeonLib.ResourceManagement;
using System;
using UnityEngine.Networking;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System.Linq;

namespace NeonLib.Localization {
    public static class LocalizationManager {
        private static bool _initialized = false;
        private static LocaleAsset LoadedLocale;
        private static string _localeName;
        public static string LoadedLocaleName { 
            get {
                if (!_initialized) Init();
                    return _localeName;
            }  
        }

        private static Dictionary<string, string> _localizedStrings = new Dictionary<string, string>();
        private static Dictionary<string, AudioClip> _localizedSounds = new Dictionary<string, AudioClip>();

        public static AudioClip GetClip(string identifier) {
            if(!_initialized) Init();
            _localizedSounds.TryGetValue(identifier, out var clip);
            return clip;
        }

        public static string GetString(string identifier) {
            if (!_initialized) Init();
            _localizedStrings.TryGetValue(identifier, out var name);
            return name;
        }

        public static void LoadLocale(string localeName) {
            string applicationDatapath = Application.persistentDataPath;
            string applicationAssetsPath = Application.dataPath;

            LoadedLocale = JsonConvert.DeserializeObject<LocaleAsset>(NeonResources.LoadTextFromFile(applicationDatapath + "/Resources/Locales/" + localeName + ".json"));

            if (LoadedLocale == null)
                return;

            _localizedStrings = LoadedLocale.localizedStrings;
            _localizedSounds = new Dictionary<string, AudioClip>();

            foreach (KeyValuePair<string, string> kvp in LoadedLocale.localizedClipPaths) {
                UnityWebRequest clip = UnityWebRequestMultimedia.GetAudioClip("file:///" + applicationAssetsPath + "/Sounds/" + kvp.Value, GetAudioTypeFromFilePath(kvp.Value));
                UnityWebRequestAsyncOperation uwrao = clip.SendWebRequest();
                uwrao.completed += (AsyncOperation) =>
                {
                    if (clip.result == UnityWebRequest.Result.ConnectionError) {
                        Debug.LogError(clip.error);
                    }
                    else {
                        _localizedSounds.Add(kvp.Key, DownloadHandlerAudioClip.GetContent(clip));
                    }
                };
            }

            PlayerPrefs.SetString("Language", localeName);
        }
        
        private static AudioType GetAudioTypeFromFilePath(string value) {
            AudioType audioType = AudioType.UNKNOWN;
            string filetype = Path.GetExtension(value);
            switch (filetype) {
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
                case ".mp3":
                    audioType = AudioType.MPEG;
                    break;
                case ".aiff":
                    audioType = AudioType.AIFF;
                    break;
                case ".it":
                    audioType = AudioType.IT;
                    break;
                case ".xm":
                    audioType = AudioType.XM;
                    break;
                case ".xma":
                    audioType = AudioType.XMA;
                    break;
                case ".mod":
                    audioType = AudioType.MOD;
                    break;
                case ".s3m":
                    audioType = AudioType.S3M;
                    break;
                case ".vag":
                    audioType = AudioType.VAG;
                    break;
            }

            return audioType;
        }

        public static void Init() {
            string savedLocale = PlayerPrefs.GetString("Language", "_DeveloperLang");
            if (savedLocale == "_DeveloperLang") {
                GenerateDefaultLocaleAsset();
            }
            LoadLocale(savedLocale);
        }

        //Creates a new instance of LocaleAsset, by using Resources.LoadAll<LocalizedString>("Strings") and Resouces.LoadAll<LocalizedAudioClip>("AudioClips") and then saves it to a json file
        private static void GenerateDefaultLocaleAsset() {
            string applicationDatapath = Application.persistentDataPath;
            string applicationAssetsPath = Application.dataPath;

            LocaleAsset localeAsset = LocaleAsset.CreateInstance<LocaleAsset>();

            localeAsset.name = "_DeveloperLang";
            localeAsset.localizedStrings = new Dictionary<string, string>();
            localeAsset.localizedClipPaths = new Dictionary<string, string>();

            var strings = Resources.LoadAll<LocalizedString>("Strings");
            var clips = Resources.LoadAll<LocalizedAudioClip>("AudioClips");

            foreach (LocalizedString stringAsset in strings) {
                localeAsset.localizedStrings.Add(stringAsset.Identifier, stringAsset.DefaultValue);
            }

            foreach (LocalizedAudioClip audioAsset in clips) {
                string fileType = ".test";
                string[] o = Directory.GetFiles(applicationAssetsPath + "/Sounds/", audioAsset.DefaultValue.name + "*");
                foreach(string oo in o) {
                    if(oo != ".meta") {
                        fileType = Path.GetExtension(oo);
                        break;
                    }
                }
                localeAsset.localizedClipPaths.Add(audioAsset.Identifier, audioAsset.DefaultValue.name + fileType);
            }

            string json = JsonConvert.SerializeObject(localeAsset, Formatting.Indented);
            if(!Directory.Exists(applicationDatapath + "/Resources/Locales")){
                Directory.CreateDirectory(applicationDatapath + "/Resources/Locales");
            }
            NeonResources.SaveTextToFilePath(json, applicationDatapath + "/Resources/Locales/" + localeAsset.name + ".json");
        }
    }
}


