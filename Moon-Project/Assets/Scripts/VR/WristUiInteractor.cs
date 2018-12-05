using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WristUiInteractor : MonoBehaviour
{
    public Transform handPointer;
    public RectTransform canvasPointer;
    private BoxCollider m_col;
    public LayerMask mask;
    private Transform m_screenTransform;
    private Vector3 m_localHitPoint;

    public float xAdd, yAdd;
    public float xMul, yMul;
    public float xAddP, yAddP;

    private void Start()
    {        
        if (!m_col) m_col = GetComponentInChildren<BoxCollider>();
        m_screenTransform = m_col.transform;
        if (!handPointer) handPointer = GameObject.Find("WristUiPointer").transform;
        if (!canvasPointer) canvasPointer = GameObject.Find("WristUiCanvasPointer").GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!handPointer) return;
        if (!m_col) return;

        Pointer();
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
}