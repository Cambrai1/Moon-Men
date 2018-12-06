using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WristUiInteractor : MonoBehaviour
{
    public Transform targetTransform;
    public float lerpSpeed = 50.0f;
    public Transform handPointer;
    public RectTransform canvasPointer;
    private BoxCollider m_col;
    public LayerMask mask;
    private Transform m_screenTransform;
    private Vector3 m_localHitPoint;
    private Transform m_transform;
    public Material screenMaterial;

    public float xAdd, yAdd;
    public float xMul, yMul;
    public float xAddP, yAddP;

    private bool m_animating;
    private bool m_active = true;

    private void Start()
    {
        m_transform = transform;
        if (!m_col) m_col = GetComponentInChildren<BoxCollider>();
        if (!targetTransform) targetTransform = GameObject.Find("WristUiTargetTransform").transform;
        m_screenTransform = m_col.transform;
        if (!handPointer) handPointer = GameObject.Find("WristUiPointer").transform;
        if (!canvasPointer) canvasPointer = GameObject.Find("WristUiCanvasPointer").GetComponent<RectTransform>();

        StartCoroutine(TurnOnCoroutine());
    }

    private void Update()
    {
        if (!handPointer) return;
        if (!m_col) return;
        Pointer();
    }

    private void LateUpdate()
    {
        if (!targetTransform) return;
        LerpPos(Time.deltaTime);
    }

    private void Pointer()
    {
        RaycastHit hit;
        Ray ray = new Ray(handPointer.position, -m_screenTransform.up);
        Debug.DrawLine(handPointer.position, handPointer.position - m_screenTransform.up);
        Physics.Raycast(handPointer.position, -m_screenTransform.up, out hit, 1.0f);
        Vector3 result = new Vector3(9999,9999,9999);
        if (hit.point != null)
        {
            m_localHitPoint = new Vector3();
            m_localHitPoint = transform.InverseTransformPoint(hit.point);
            result = new Vector3(((m_localHitPoint.x + xAdd) * xMul) + xAddP, ((m_localHitPoint.y + yAdd) * yMul) + yAddP, 0.0f);
        }
        if(result.x > 400.0f || result.x < -400.0f|| result.y > 300.0f || result.y < -300.0f)
        {
            canvasPointer.gameObject.SetActive(false);
        }
        else
        {
            canvasPointer.gameObject.SetActive(true);
            canvasPointer.anchoredPosition = result;
        }
    }

    private void LerpPos(float _dt)
    {
        Vector3 pos = Vector3.Lerp(m_transform.position, targetTransform.position, lerpSpeed * _dt);
        Quaternion rot = Quaternion.Lerp(m_transform.rotation, targetTransform.rotation, lerpSpeed * _dt);
        m_transform.position = pos;
        m_transform.rotation = rot;
    }

    public void TurnOn()
    {
        if (m_active) return;
        if (!m_animating) StartCoroutine(TurnOnCoroutine());
    }
    public void TurnOff()
    {
        if (!m_active) return;
        if (!m_animating) StartCoroutine(TurnOffCoroutine());
    }
    public void Toggle()
    {
        if (m_active) TurnOff();
        else TurnOn();
    }

    private IEnumerator TurnOnCoroutine()
    {
        m_animating = true;
        m_active = true;
        float t = 0.0f;
        screenMaterial.SetFloat("_Completion", 0.0f);
        screenMaterial.SetFloat("_Fade", 0.0f);
        while (t < 1.0f)
        {
            screenMaterial.SetFloat("_Fade", t);
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0.0f, 1.0f);
            yield return new WaitForEndOfFrame();
        }
        screenMaterial.SetFloat("_Fade", 1.0f);
        m_animating = false;
        yield return null;
    }
    private IEnumerator TurnOffCoroutine()
    {
        m_animating = true;
        m_active = false;
        float t = 0.0f;
        screenMaterial.SetFloat("_Completion", 0.0f);
        screenMaterial.SetFloat("_Fade", 1.0f);
        while (t < 1.0f)
        {
            screenMaterial.SetFloat("_Completion", t);
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0.0f, 1.0f);
            yield return new WaitForEndOfFrame();
        }
        screenMaterial.SetFloat("_Fade", 0.0f);
        screenMaterial.SetFloat("_Completion", 1.0f);
        m_animating = false;
        yield return null;
    }
}