using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorScreenshot : MonoBehaviour {

    [MenuItem("Tools/Screenshot")]
    private static void CaptureEditorScreenshot()
    {
        GameObject g = GameObject.Find("ScreenshotCam");
        if (g != null)
        {
            Camera c = g.GetComponent<Camera>();
            ScreenshotTool t = g.GetComponent<ScreenshotTool>();
            Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
            Quaternion rot = SceneView.lastActiveSceneView.camera.transform.rotation;
            g.transform.position = pos;
            g.transform.rotation = rot;
            c.enabled = true;
            c.fieldOfView = 55.5f;
            t.Capture(c, 1);
            c.enabled = false;
        }
    }
}
