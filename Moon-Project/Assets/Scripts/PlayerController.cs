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
    private Vector2 m_movementInput;
    [SerializeField]
    private Vector2 m_normalisedInput;

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        m_movementInput = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.LeftHand);
        m_normalisedInput = m_movementInput.normalized;
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public float movementSpeed = 1f;
    public float teleportDistance = 1f;
}