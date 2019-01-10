using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UiButton : MonoBehaviour {

    private BoxCollider m_col;
    private RectTransform m_rect;
    public UnityEvent onClick;

    private void Start()
    {
        m_rect = GetComponent<RectTransform>();

        m_col = GetComponent<BoxCollider>();
        if(m_col == null)
        {
            m_col = gameObject.AddComponent<BoxCollider>();
        }
        Vector3 newSize = m_rect.sizeDelta * 1.2f;
        newSize.z = 1.0f;
        newSize.y = newSize.x;
        m_col.size = newSize;
        m_col.center = new Vector3(-50, 0, 0);
    }

    public void TriggerButton()
    {
        Debug.Log("Triggered UI BUtton");
        onClick.Invoke();
    }
}
