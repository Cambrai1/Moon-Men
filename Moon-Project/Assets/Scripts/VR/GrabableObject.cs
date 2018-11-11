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

    public void Grab(Transform _hand)
    {
        if (!body) return;

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
        transform.SetParent(_hand);
        transform.localPosition = Vector3.zero;
        transform.rotation = _hand.rotation;
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
        body.isKinematic = true;
        body.useGravity = false;
        if (disableColliderOnGrab) collider.enabled = false;
        m_handTransform = _hand;
        isGrabbed = true;
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