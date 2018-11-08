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
    private float m_oxygenDepletionRate = 2.0f;
    public float power = 100.0f;

    public Color resourceHigh = Color.green;
    public Color resourceMedium = Color.yellow;
    public Color resourceLow = new Color(1.0f, 0.66f, 0.0f);
    public Color resourceCritical = Color.red;

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

        //  DEPLETE OXYGEN & POWER
        DecreaseOxygenLevel(m_oxygenDepletionRate * Time.deltaTime);
        DecreasePowerLevel(m_oxygenDepletionRate * Time.deltaTime / 3.0f);

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
        if(SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            GetComponent<ScreenshotTool>().Capture();
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
        if (oxygen <= 25.0f) { m_oxygenBar.color = resourceCritical; }
        else if (oxygen <= 50.0f) { m_oxygenBar.color = resourceLow; }
        else if (oxygen <= 75.0f) { m_oxygenBar.color = resourceMedium; }
        else { m_oxygenBar.color = resourceHigh; }
        m_oxygenIcon.color = m_oxygenBar.color;

        //  POWER
        m_powerBar.fillAmount = power / 100.0f;
        if (power <= 25.0f) { m_powerBar.color = resourceCritical; }
        else if (power <= 50.0f) { m_powerBar.color = resourceLow; }
        else if (power <= 75.0f) { m_powerBar.color = resourceMedium; }
        else { m_powerBar.color = resourceHigh; }
        m_powerIcon.color = m_powerBar.color;
    }

    public void SetOxygenLevel(float _level)
    {
        oxygen = _level;
    }
    public void DecreaseOxygenLevel(float _amount)
    {
        SetOxygenLevel(oxygen - _amount);
    }

    public void SetPowerLevel(float _level)
    {
        power = _level;
    }
    public void DecreasePowerLevel(float _amount)
    {
        SetPowerLevel(power - _amount);
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