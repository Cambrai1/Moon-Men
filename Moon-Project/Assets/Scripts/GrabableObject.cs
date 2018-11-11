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
    public float closestHandDistance;

    public GrabMethod grabMethod;

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

        //isHovered = false;
        bool flag = false;
        if (!useCollider)
        {
            if (closestHandDistance <= grabRange)flag = true;
        }
        else
        {
            if(closestHandDistance <= m_checkRange)
            {
                if (m_col.bounds.Contains(m_closestHand.position))
                {
                    flag = true;
                }
            }
        }
        m_player.RequestHoverStatus(this, m_closestHand, flag);

        if (isHovered && !isGrabbed)
        {
            m_renderer.material.color = Color.blue;
        }
        else
        {
            m_renderer.material.color = m_originalColour;
        }

        if(isGrabbed && grabMethod == GrabMethod.position)
        {
            m_transform.position = m_closestHand.position;
            m_transform.rotation = m_closestHand.rotation;
        }
    }

    public void GetHandDistances()
    {
        float rHandDist = Vector3.Distance(m_transform.position, m_rHand.position);
        float lHandDist = Vector3.Distance(m_transform.position, m_lHand.position);
        if (rHandDist <= lHandDist) { m_closestHand = m_rHand; closestHandDistance = rHandDist; }
        else { m_closestHand = m_lHand; closestHandDistance = lHandDist; }
    }

    public void ConfirmHoveredObject(Transform _hand, bool _state)
    {
        isHovered = _state;
    }

    public enum GrabMethod { parent, spring, position }

    public void Grab(Transform _hand)
    {
        switch (grabMethod)
        {
            case GrabMethod.parent:
                ParentGrab(_hand);
                break;
            case GrabMethod.spring:
                SpringGrab(_hand);
                break;
            case GrabMethod.position:
                PositionGrab(_hand);
                break;
            default:
                break;
        }
    }
    private void ParentGrab(Transform _hand)
    {
        m_transform.SetParent(_hand);
        m_transform.localPosition = Vector3.zero;
        m_transform.rotation = _hand.rotation;
        body.isKinematic = true;
        body.useGravity = false;
        isGrabbed = true;
    }
    private void SpringGrab(Transform _hand)
    {
        SpringJoint spring = GetComponent<SpringJoint>();
        if (!spring)
        {
            spring = gameObject.AddComponent<SpringJoint>();
        }
        spring.connectedBody = _hand.GetComponent<Rigidbody>();
        body.useGravity = false;
        isGrabbed = true;
    }
    private void PositionGrab(Transform _hand)
    {
        isGrabbed = true;
    }
}