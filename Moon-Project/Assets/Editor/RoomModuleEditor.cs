using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RoomModule))]
public class RoomModuleEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomModule mod = (RoomModule)target;

        if(GUILayout.Button("Set Up"))
        {
            mod.SetUp();

        }
    }
}