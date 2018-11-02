using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabableObject : MonoBehaviour {

    public string name;
    public Rigidbody body;
    public bool isGrabbed;
    public bool isHovered;

    private void Start()
    {
        if (name == "") name = gameObject.name;
        if (!body) body = GetComponent<Rigidbody>();
        isGrabbed = false;
        m_renderer = GetComponent<Renderer>();
        m_originalColour = m_renderer.material.color;
    }

    private Color m_originalColour;
    private Renderer m_renderer;

    private void Update()
    {
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