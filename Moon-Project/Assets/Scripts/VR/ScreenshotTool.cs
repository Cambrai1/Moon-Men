using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotTool : MonoBehaviour {

    public int width = 3840;
    public int height = 2160;
    public float fov = 0.0f;

    public Camera cam;
    public KeyCode captureKey = KeyCode.P;

    private RenderTexture m_tex;

    [MenuItem("Tools/Screenshot")]
    private static void CaptureEditorScreenshot()
    {
        GameObject g = GameObject.Find("ScreenshotCam");
        if(g != null)
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


    private void Start()
    {
        if (!cam)
        {
            if(GetComponent<Camera>())
            {
                cam = GetComponent<Camera>();
            }
            else
            {
                cam = Camera.main;
            }
        }

        if (fov == 0.0f) fov = cam.fieldOfView;
    }

    private void Update()
    {
        if(Input.GetKeyDown(captureKey))
        {
            Capture();
        }
    }

    public void Capture()
    {
        Capture(cam, 1);
    }

    public void Capture(Camera _cam, int _multiplier)
    {
        StartCoroutine(CaptureCoroutine(cam, _multiplier));
    }

    private IEnumerator CaptureCoroutine(Camera _cam, int _multiplier)
    {
        int actualWidth = width * _multiplier;
        int actualHeight = height * _multiplier;
        string filename = Time.time.ToString();
        m_tex = new RenderTexture(actualWidth, actualHeight, 24);
        TextureFormat f = TextureFormat.RGB24;
        _cam.targetTexture = m_tex;
        Texture2D screenshot = new Texture2D(actualWidth, actualHeight, f, false);
        _cam.Render();
        RenderTexture.active = m_tex;
        screenshot.ReadPixels(new Rect(0, 0, actualWidth, actualHeight), 0, 0);
        _cam.targetTexture = null;
        byte[] data = screenshot.EncodeToPNG();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + filename + ".png", data);
        Debug.Log("TOOK A SCREENSHOT");
        yield return null;
    }
}