using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private PlayerMovementSettings m_movementSettings;

    [SerializeField]
    private Transform m_rightHand, m_leftHand;

    [SerializeField]
    private Vector2 m_trackpadInput;
    [SerializeField]
    private Vector2 m_trackpadNormalised;
    [SerializeField]
    private float m_trackpadDeadzone = 0.5f;

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        {
            m_trackpadInput = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.LeftHand);
            if(m_trackpadInput.magnitude <= m_trackpadDeadzone)
            {
                m_trackpadInput = Vector2.zero;
                m_trackpadNormalised = Vector2.zero;
            }
            else
            {
                m_trackpadNormalised = m_trackpadInput.normalized;
            }
        }
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public float movementSpeed = 1f;
    public float teleportDistance = 1f;
}