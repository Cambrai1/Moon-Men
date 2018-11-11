using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabableObject : Interactable {

    [SerializeField]
    private bool m_ready;
    public GrabMethod grabMethod;
    public bool isGrabbed;
    private Transform m_handTransform;
    public bool disableColliderOnGrab = false;

    public void ConfirmHoveredObject(Transform _hand, bool _state)
    {
        isHovered = _state;
    }

    public enum GrabMethod { parent, spring, position }

    public GrabableObject Grab(Transform _hand)
    {
        if (_hand == m_handTransform) return null;
        if (!body) return null;
        m_handTransform = _hand;
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
        body.useGravity = false;
        if (disableColliderOnGrab) collider.enabled = false;
        isGrabbed = true;
        Debug.Log("Grabable '" + gameObject.name + "' was grabbed by '" + m_handTransform.name + "'");
        return this;
    }
    private void ParentGrab(Transform _hand)
    {
        transform.SetParent(_hand);
        transform.localPosition = Vector3.zero;
        transform.rotation = _hand.rotation;
        body.isKinematic = true;
    }
    private void SpringGrab(Transform _hand)
    {
        SpringJoint spring = GetComponent<SpringJoint>();
        if (!spring)
        {
            spring = gameObject.AddComponent<SpringJoint>();
        }
        spring.connectedBody = _hand.GetComponent<Rigidbody>();
    }
    private void PositionGrab(Transform _hand)
    {
        body.isKinematic = true;
    }

    public void Release()
    {
        if (!body) return;
        switch (grabMethod)
        {
            case GrabMethod.parent:
                transform.SetParent(null);
                break;
            case GrabMethod.spring:
                Destroy(GetComponent<SpringJoint>());
                break;
            case GrabMethod.position:
                break;
            default:
                break;
        }
        body.isKinematic = false;
        body.useGravity = true;
        if (disableColliderOnGrab) collider.enabled = true;
        isGrabbed = false;
        m_handTransform = null;
    }

    public override void InteractableUpdate()
    {
        if(grabMethod == GrabMethod.position)
        {
            if(isGrabbed && m_handTransform)
            {
                transform.position = m_handTransform.position;
            }
        }
    }
}