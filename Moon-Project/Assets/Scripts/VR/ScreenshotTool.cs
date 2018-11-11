using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenshotTool : MonoBehaviour {

    public int width = 3840;
    public int height = 2160;
    public float fov = 0.0f;

    public Camera cam;

    private RenderTexture m_tex;

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

    public void Capture()
    {
        Capture(1);
    }

    public void Capture(int _multiplier)
    {
        StartCoroutine(CaptureCoroutine(_multiplier));
    }

    private IEnumerator CaptureCoroutine(int _multiplier)
    {
        int actualWidth = width * _multiplier;
        int actualHeight = height * _multiplier;
        string filename = Time.time.ToString();
        m_tex = new RenderTexture(actualWidth, actualHeight, 24);
        TextureFormat f = TextureFormat.RGB24;
        cam.targetTexture = m_tex;
        Texture2D screenshot = new Texture2D(actualWidth, actualHeight, f, false);
        cam.Render();
        RenderTexture.active = m_tex;
        screenshot.ReadPixels(new Rect(0, 0, actualWidth, actualHeight), 0, 0);
        cam.targetTexture = null;
        byte[] data = screenshot.EncodeToPNG();
        RenderTexture.active = null;
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + filename + ".png", data);
        Debug.Log("TOOK A SCREENSHOT");
        yield return null;
    }
}