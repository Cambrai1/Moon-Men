using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabableObject : MonoBehaviour {

    public string name;
    public Rigidbody body;
    public bool isGrabbed;
    public bool isHovered;

    [SerializeField]
    private bool m_ready;

    [SerializeField]
    private bool useCollider = true;
    private Collider m_col;
    [SerializeField]
    private float grabRange = 0.2f;
    [SerializeField]
    private float m_checkRange = 0.5f;
    private Color m_originalColour;
    private Renderer m_renderer;
    private Transform m_transform;

    [SerializeField]
    private PlayerController m_player;
    private Transform m_rHand, m_lHand;
    private Transform m_closestHand;
    [SerializeField]
    private float m_handDistance;

    private void Start()
    {
        m_transform = gameObject.transform;

        if (name == "") name = gameObject.name;
        if (!body) body = GetComponent<Rigidbody>();

        m_renderer = GetComponent<Renderer>();
        if (m_renderer)
        {
            m_originalColour = m_renderer.material.color;
        }
        else return;

        m_col = GetComponent<Collider>();
        if (!m_col) { useCollider = false; }

        if (!m_player)
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if (m_player)
        {
            m_rHand = m_player.rHandTransform;
            m_lHand = m_player.lHandTransform;
        }
        else return;

        m_ready = true;
    }

    private void Update()
    {
        if (!m_ready) return;

        GetHandDistances();

        isHovered = false;
        if (!useCollider)
        {
            if (m_handDistance <= grabRange) isHovered = true;
        }
        else
        {
            if(m_handDistance <= m_checkRange)
            {
                if (m_col.bounds.Contains(m_closestHand.position))
                {
                    isHovered = true;
                }
            }
        }

        if(isHovered && !isGrabbed)
        {
            m_renderer.material.color = Color.blue;
        }
        else
        {
            m_renderer.material.color = m_originalColour;
        }
    }

    public void GetHandDistances()
    {
        float rHandDist = Vector3.Distance(m_transform.position, m_rHand.position);
        float lHandDist = Vector3.Distance(m_transform.position, m_lHand.position);
        if (rHandDist <= lHandDist) { m_closestHand = m_rHand; m_handDistance = rHandDist; }
        else { m_closestHand = m_lHand; m_handDistance = lHandDist; }
    }
}