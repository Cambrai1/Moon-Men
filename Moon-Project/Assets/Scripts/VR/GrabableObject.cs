using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
//  INHERIT FROM INTERACTABLE & MONOBEHAVIOUR
public class GrabableObject : Interactable {

    public GrabMethod grabMethod;                   //  The method used to grab the object
    public bool isGrabbed;                          //  Is the object currently grabbed?
    private Transform m_handTransform;              //  The hand currently grabbing the object
    public bool disableColliderOnGrab = false;      //  Should the collider be disabled when grabbed?
    public Transform grabPoint;                     //  The angle and position to grab the object with.

    [SerializeField]
    private int m_momentumExtrapolation = 10;       //  The number of frames used to extrapolate velocity
    private Vector3[] m_positionFrames;             //  The array of the most recent extrapolation frames

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

    public void Release(Transform _hand)
    {
        if (_hand != m_handTransform) return;
        Release();
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

        body.velocity = EstimateVelocity();
    }

    public override void InteractableUpdate()
    {
        for(int i = m_momentumExtrapolation - 1; i > 0; i--)
        {
            m_positionFrames[i] = m_positionFrames[i - 1];
        }
        m_positionFrames[0] = transform.position;

        if(grabMethod == GrabMethod.position)
        {
            if(isGrabbed && m_handTransform)
            {
                transform.position = m_handTransform.position;
            }
        }
    }

    public Vector3 EstimateVelocity()
    {
        return (m_positionFrames[0] - m_positionFrames[m_momentumExtrapolation - 1]) / (Time.deltaTime * m_momentumExtrapolation);
    }

    public override void InteractableInit()
    {
        base.InteractableInit();
        m_positionFrames = new Vector3[m_momentumExtrapolation];
    }
}