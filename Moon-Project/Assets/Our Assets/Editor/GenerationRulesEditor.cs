using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GenerationRules))]
public class GenerationRulesEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerationRules rules = (GenerationRules)target;

        if (GUILayout.Button("Set Up"))
        {
            rules.SetUp();
        }
    }
}