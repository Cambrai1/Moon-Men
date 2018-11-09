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
    private Color m_originalColour;
    private Renderer m_renderer;

    [SerializeField]
    private PlayerController m_player;
    private Transform m_playerRightHand, m_playerLeftHand;

    private void Start()
    {
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
            m_playerRightHand = m_player.rHandTransform;
            m_playerLeftHand = m_player.lHandTransform;
        }
        else return;


        m_ready = true;
    }

    private void Update()
    {
        if (!m_ready) return;

        if(isHovered && !isGrabbed)
        {
            m_renderer.material.color = Color.blue;
        }
        else
        {
            m_renderer.material.color = m_originalColour;
        }
    }
}