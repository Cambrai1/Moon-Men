using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Transform m_transform;

    [SerializeField]
    private PlayerMovementSettings m_movementSettings;

    [Header("Input")]

    public PlayerControllerInput rightHand;
    public PlayerControllerInput leftHand;
    [SerializeField]
    private float m_trackpadDeadzone = 0.5f;

    [Header("Interaction")]

    public bool canGrab = true;
    public GrabableObject rightHandObject;
    public GrabableObject leftHandObject;

    [Header("Stats")]

    public Transform oxygenUi;
    private Text m_oxygenIcon;
    private Image m_oxygenBar;
    public Transform powerUi;
    private Image m_powerIcon;
    private Image m_powerBar;
    public float oxygen = 100.0f;
    private float m_oxygenDepletionRate = 1.0f;
    public float power = 100.0f;

    private void Start()
    {
        m_transform = transform;

        if(oxygenUi)
        {
            m_oxygenIcon = oxygenUi.GetChild(0).GetComponent<Text>();
            m_oxygenBar = oxygenUi.GetChild(1).GetComponent<Image>();
        }
        if (powerUi)
        {
            m_powerIcon = powerUi.GetChild(0).GetComponent<Image>();
            m_powerBar = powerUi.GetChild(1).GetComponent<Image>();
        }
    }

    private void Update()
    {
        //  INPUT & MOVEMENT
        GetInput();
        if (m_movementSettings.useTrackpad) { TrackpadMovement(); }

        //  DEPLETE OXYGEN
        oxygen -= m_oxygenDepletionRate * Time.deltaTime;

        //  UPDATE WRIST UI
        UpdateWristUi();
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

    private void UpdateWristUi()
    {
        //  OXYGEN
        m_oxygenBar.fillAmount = oxygen / 100.0f;

        //  POWER
        m_powerBar.fillAmount = power / 100.0f;
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