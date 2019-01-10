using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDetector : MonoBehaviour {

    [SerializeField]
    private Transform m_player;
    private Transform m_transform;
    [SerializeField]
    private bool m_useTopDownDistance = true;

    [SerializeField]
    private float m_triggerDistance = 3.0f;
    [SerializeField]
    private float m_untriggerDistance = 4.0f;

    private bool m_triggered = false;
    private float m_currentDistane;

    public UnityEvent TriggerStart;
    public UnityEvent TriggerEnd;
    public UnityEvent TriggerChange;

    private void Start()
    {
        m_transform = transform;
        if (!m_player) m_player = GameObject.FindWithTag("Player").transform.Find("Camera");
    }

    private void Update()
    {
        if(m_useTopDownDistance) m_currentDistane = Vector2.Distance(StaticMethods.TopDown(m_player.position), StaticMethods.TopDown(m_transform.position));
        else m_currentDistane = Vector3.Distance(m_player.position, m_transform.position);
        if (m_currentDistane <= m_triggerDistance && !m_triggered)
        {
            m_triggered = true;
            TriggerStart.Invoke();
            TriggerChange.Invoke();
        }
        else if (m_currentDistane > m_untriggerDistance && m_triggered)
        {
            m_triggered = false;
            TriggerEnd.Invoke();
            TriggerChange.Invoke();
        }
    }
}