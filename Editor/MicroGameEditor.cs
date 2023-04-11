using NeonLib.Gameplay.MicroGames;
using NeonLib.States;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NeonLib.Editor {
    [CustomEditor(typeof(MicroGame), true)]
    public class MicroGameEditor : UnityEditor.Editor {



        [MenuItem("Assets/Create/NeonLib/MicroGames/MicroGame", priority = 0)]
        public static void CreateNewMicroGame() {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(assetPath)) {
                assetPath = "Assets";
            }
            else if (Path.GetExtension(assetPath) != "") {
                assetPath = assetPath.Replace(Path.GetFileName(assetPath), "");
            }

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/NewMicroGame.asset");

            // Create the MicroGame ScriptableObject
            MicroGame microGame = ScriptableObject.CreateInstance<MicroGame>();

            // Save the MicroGame asset
            AssetDatabase.CreateAsset(microGame, uniquePath);

            // Add the associated ScriptableObjects as sub-assets
            AssetDatabase.AddObjectToAsset(microGame.StateMachine, microGame);
            AssetDatabase.AddObjectToAsset(microGame.StartGameEvent, microGame);
            AssetDatabase.AddObjectToAsset(microGame.BeforeStartEvent, microGame);
            AssetDatabase.AddObjectToAsset(microGame.OnEndEvent, microGame);

            // Enumerate through each state in the MicroGame
            foreach (State state in microGame.States) {
                AssetDatabase.AddObjectToAsset(state, microGame);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(microGame);
        }

        public static void AddInputResponse(MicroGame microGame) {
            MicroGameInputResponse newInputResponse = ScriptableObject.CreateInstance<MicroGameInputResponse>();
            microGame.inputResponses.Add(newInputResponse);

            // Save the InputResponse as a sub-asset of the MicroGame
            AssetDatabase.AddObjectToAsset(newInputResponse, microGame);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveInputResponse(MicroGame microGame, int inputResponseIndex) {
            if (microGame.inputResponses.Count <= 0) {
                Debug.LogWarning("No InputResponse left to remove.");
                return;
            }

            MicroGameInputResponse inputResponseToRemove = microGame.inputResponses[inputResponseIndex];

            // Remove the InputResponse from the MicroGame
            microGame.inputResponses.RemoveAt(inputResponseIndex);

            // Delete the InputResponse asset
            AssetDatabase.RemoveObjectFromAsset(inputResponseToRemove);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}