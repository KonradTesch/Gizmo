using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rectangle.Level
{
    [CustomEditor(typeof(LevelBuilder))]
    public class LevelBuilderInspector : Editor
    {
        LevelBuilder builder;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            builder = (LevelBuilder)target;

            if (GUILayout.Button("Build Level"))
            {
                builder.BuildLevel();
            }
        }
    }

}
