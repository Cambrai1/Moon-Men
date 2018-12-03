using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ModularWorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ModularWorldGenerator gen = (ModularWorldGenerator)target;

        if (GUILayout.Button("Load Modules"))
        {
            gen.LoadModulesFromChildren();
        }
        if (GUILayout.Button("Create Seed"))
        {
            gen.CreateWorldSeed();
        }
    }
}
