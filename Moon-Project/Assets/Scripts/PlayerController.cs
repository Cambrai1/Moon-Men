using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour {

    private Transform m_transform;

    [SerializeField]
    private PlayerMovementSettings m_movementSettings;

    [Header("Input")]
    public PlayerControllerInput rightHand;
    public PlayerControllerInput leftHand;
    [SerializeField]
    private float m_trackpadDeadzone = 0.5f;

    private void Start()
    {
        m_transform = transform;
    }

    private void Update()
    {
        GetInput();
        if (m_movementSettings.useTrackpad) { TrackpadMovement(); }
    }

    private void GetInput()
    {
        //  RIGHT HAND
        {
            rightHand.trackpadAbsolute = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.RightHand);
            if (rightHand.trackpadAbsolute.magnitude <= m_trackpadDeadzone)
            {
                rightHand.trackpadAbsolute = Vector2.zero;
                rightHand.trackpadNormalised = Vector2.zero;
            }
            else
            {
                rightHand.trackpadNormalised = rightHand.trackpadAbsolute.normalized;
            }
        }
        //  LEFT HAND
        {
            leftHand.trackpadAbsolute = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.LeftHand);
            if(leftHand.trackpadAbsolute.magnitude <= m_trackpadDeadzone)
            {
                leftHand.trackpadAbsolute = Vector2.zero;
                leftHand.trackpadNormalised = Vector2.zero;
            }
            else
            {
                leftHand.trackpadNormalised = leftHand.trackpadAbsolute.normalized;
            }
        }
    }

    private void TrackpadMovement()
    {
        Vector3 move = Vector3.zero;
        move.x = leftHand.trackpadNormalised.x * m_movementSettings.movementSpeed;
        move.z = leftHand.trackpadNormalised.y * m_movementSettings.movementSpeed;
        move *= Time.deltaTime;

        m_transform.position += move;
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public bool useTrackpad = true;
    public float movementSpeed = 1f;
    public float teleportDistance = 1f;
}

[System.Serializable]
public class PlayerControllerInput
{
    public Vector2 trackpadAbsolute;
    public Vector2 trackpadNormalised;
}