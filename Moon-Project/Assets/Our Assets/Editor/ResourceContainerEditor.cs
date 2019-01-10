using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ResourceContainer))]
public class ResourceContainerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ResourceContainer res = (ResourceContainer)target;

        if (GUILayout.Button("Set Resource"))
        {
            res.SetResource(res.resourceType, res.resourceQuantity);
        }
        if (GUILayout.Button("Collect Resource"))
        {
            res.CollectResource();
        }
    }
}