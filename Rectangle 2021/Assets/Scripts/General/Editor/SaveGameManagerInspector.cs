using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rectangle.General
{
    /// <summary>
    /// Custom Inspector to save the game data manually.
    /// </summary>
    [CustomEditor(typeof(SaveGameManager))]
    public class SaveGameManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Save"))
            {
                SaveGameManager saveManager = (SaveGameManager)target;

                saveManager.SaveData();
            }
        }
    }
}
