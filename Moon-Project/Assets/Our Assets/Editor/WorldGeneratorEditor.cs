﻿using System.Collections;
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
        if (GUILayout.Button("Set Up Modules & Apply Prefabs"))
        {
            foreach(RoomModule mod in gen.loadedModules)
            {
                mod.SetUp();
                if(PrefabUtility.GetCorrespondingObjectFromSource(mod))
                {
                    PrefabUtility.ReplacePrefab(mod.gameObject, PrefabUtility.GetCorrespondingObjectFromSource(mod), ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }
        if (GUILayout.Button("Save Seed"))
        {
            gen.SaveSeed();
        }
    }
}