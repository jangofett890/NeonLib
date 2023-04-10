using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace NeonLib.ResourceManagement {
    public static class NeonResources {

        private static BinaryFormatter m_binaryFormatter;

        public static BinaryFormatter BinaryFormatter { get { if (m_binaryFormatter == null) { m_binaryFormatter = new BinaryFormatter(); } return m_binaryFormatter; } }

        public static void SaveObjectToFilePath(object O, string filePath) {
            File.WriteAllBytes(filePath, SerializeObject(O));
        }

        public static void SaveTextToFilePath(string O, string filePath) {
            File.WriteAllText(filePath, O);
        }

        private static byte[] SerializeObject(object o) {
            if (o == null) { return null; }
            MemoryStream ms = new MemoryStream();
            BinaryFormatter.Serialize(ms, o);
            return ms.ToArray();
        }

        public static T LoadObjectFromFilePath<T>(string filePath) {
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            T O = (T)BinaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return O;
        }

        public static string LoadTextFromFile(string filePath) {
            return File.ReadAllText(filePath);
        }

        public static void SaveGameSaveToSaveDataPath(byte[] data, string fileName) {
            string path = Application.dataPath + "/Resources/" + "SaveData";
            string filePath = path + fileName + ".data";

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                FileStream newFile = File.Create(filePath);
                newFile.Close();
            }

            // Create the file.
            File.WriteAllBytes(filePath, data);
        }

        public static byte[] LoadGameSaveFromSaveDataPath(string fileName) {
            string path = Application.dataPath + "/Resources/" + "SaveData";
            string filePath = path + fileName + ".data";
            if (!Directory.Exists(path)) {
                return null;
            }

            return File.ReadAllBytes(filePath);
        }

        public static byte[] Serialize(object obj) {
            byte[] data = null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter.Serialize(ms,obj);
            data = ms.GetBuffer();
            ms.Flush();
            ms.Position = 0;
            ms.Close();
            return data;
        }

        public static object Deserialize(byte[] data) {
            MemoryStream ms = new MemoryStream(data);
            object o = BinaryFormatter.Deserialize(ms);
            ms.Flush();
            ms.Position = 0;
            ms.Close();
            return o;
        }
    }
}