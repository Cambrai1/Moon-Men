using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiButton : MonoBehaviour {

    private BoxCollider m_col;
    private RectTransform m_rect;

    private void Start()
    {
        m_rect = GetComponent<RectTransform>();

        m_col = GetComponent<BoxCollider>();
        if(m_col == null)
        {
            m_col = gameObject.AddComponent<BoxCollider>();
        }
        m_col.size = m_rect.sizeDelta;
    }
}
