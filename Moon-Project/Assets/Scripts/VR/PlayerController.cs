using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

//  CLASS IS USED TO CONTROLL PLAYER MOVEMENT AND INTERACTION
public class PlayerController : MonoBehaviour {

    private Transform m_transform;                          //  The transform of the vr controller

    [Header("Movement")]

    [SerializeField]
    private PlayerMovementSettings m_movementSettings;      //  The movement settings

    [Header("Bounds")]
    [SerializeField]
    private bool m_useBounds = true;
    [SerializeField]
    private float m_boundsRadius = 0.2f;
    [SerializeField]
    private LayerMask m_boundsMask;
    [SerializeField]
    private PostProcessProfile m_postProfile;
    private float m_vignetteFade = 0.0f;
    [SerializeField]
    private float m_boundsFadeSpeed = 1.0f;
    private Vignette m_vignette;
    public bool forceVignette = false;

    [Header("Input")]

    public PlayerControllerInput rHandInput;                //  The right hand input
    public PlayerControllerInput lHandInput;                //  The left hand input
    [SerializeField]
    private float m_trackpadDeadzone = 0.5f;                //  The trackpad movement deadzone

    [Header("Interaction")]

    private Transform m_headTransform;                      //  The transform of the head object
    public Transform rHandTransform;                        //  The transform of the right hand
    public Transform lHandTransform;                        //  The transform of the left hand
    private Camera m_vrCam;
    private LayerMask m_originalCamLayers;
    public bool canGrab = true;                             //  Should the player be able to grab?
    [HideInInspector]
    public Interactable rHoverObject;                       //  The interactable being hovered over by the right hand
    [HideInInspector]
    public Interactable lHoverObject;                       //  The interactable being hovered over by the left hand
    public GrabableObject rGrabbedObject;                   //  The grabable being grabbed by the right hand
    public GrabableObject lGrabbedObject;                   //  The grabable being grabbed by the left hand
    private List<Interactable> m_interactables;             //  A list of all nearby interactables

    [Header("Stats")]

    public Transform oxygenUi;                              //  The main transform of the Oxygen UI
    private Text m_oxygenIcon;                              //  The O2 UI text
    private Image m_oxygenBar;                              //  The O2 resource bar
    public Transform powerUi;                               //  The main transform of the Power UI
    private Image m_powerIcon;                              //  The Power UI image
    private Image m_powerBar;                               //  The Power resource bar
    public RawImage heartRateImage;                         //  The Heart rate monitor UI
    public WristUiInteractor wristUi;

    public float oxygen = 100.0f;                           //  The amount of oxygen remaining
    private float m_oxygenDepletionRate = 2.0f;             //  The rate at which oxygen depletes
    public float power = 100.0f;                            //  The amount of power remaining
    public float heartRate = 50.0f;

    public Color resourceHigh = Color.green;                //  The colour used to indicate high resource quantity
    public Color resourceMedium = Color.yellow;             //  The colour used to indicate medium resource quantity
    public Color resourceLow = new Color(1.0f, 0.6f, 0.0f); //  The colour used to indicate low resource quantity
    public Color resourceCritical = Color.red;              //  The colour used to indicate critical resource quantity

    public float oxygenDeprivationTime = 10.0f;             //  The time before the player dies from oxygen deprivation
    private float m_oxDeprivation = 0.0f;
    [HideInInspector]
    public bool suffocating;
    private bool m_prevSuffocating;
    public Canvas deadCanvas;

    public HoloMap portableHoloMap;

    [Header("Audio")]

    public AudioSource playerSource;
    public List<AudioClip> suffocationClips;
    private bool m_playingSuffocatingClip;
    public List<AudioClip> oxygenatedClips;
    private bool m_playingOxygenatedClip;
    private bool m_playingBreathingAudio;

    private void Start()
    {
        m_transform = transform;

        m_interactables = new List<Interactable>();

        if(!m_headTransform)
        {
            m_headTransform = m_transform.Find("Camera");
        }
        m_vrCam = m_headTransform.GetComponent<Camera>();
        m_originalCamLayers = m_vrCam.cullingMask;
        if(!rHandTransform || !lHandTransform)
        {
            rHandTransform = m_transform.Find("Controller (right)");
            lHandTransform = m_transform.Find("Controller (left)");
        }

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

        m_vignette = m_postProfile.GetSetting<Vignette>();
    }

    private void Update()
    {
        HoverInteractable();

        //  INPUT & MOVEMENT
        GetInput();
        if (m_movementSettings.useTrackpad) { TrackpadMovement(); }

        //  DEPLETE OXYGEN & POWER
        DecreaseOxygenLevel(m_oxygenDepletionRate * Time.deltaTime);

        //  UPDATE WRIST UI
        UpdateWristUi();

        //  CHECK PLAYER BOUNDS
        PlayerBounds();

        PowerDeprivation();
        OxygenDeprivation();
    }

    private void GetInput()
    {
        //  RIGHT HAND
        {
            //  TRACKPAD
            rHandInput.trackpadAbsolute = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.RightHand);
            if (rHandInput.trackpadAbsolute.magnitude <= m_trackpadDeadzone)
            {
                rHandInput.trackpadAbsolute = Vector2.zero;
                rHandInput.trackpadNormalised = Vector2.zero;
            }
            else
            {
                rHandInput.trackpadNormalised = rHandInput.trackpadAbsolute.normalized;
            }

            //  TRIGGER
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                //GetComponent<ScreenshotTool>().Capture();
                TryGrab(rHoverObject, rHandTransform, ref rGrabbedObject);
            }
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand))
            {
                TryRelease(ref rGrabbedObject, rHandTransform);
            }

            //  MENU BUTTON
            if (SteamVR_Input._default.inActions.Menu.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                wristUi.Toggle();
            }
        }
        //  LEFT HAND
        {
            //  TRACKPAD
            lHandInput.trackpadAbsolute = SteamVR_Input._default.inActions.Trackpad.GetAxis(SteamVR_Input_Sources.LeftHand);
            if(lHandInput.trackpadAbsolute.magnitude <= m_trackpadDeadzone)
            {
                lHandInput.trackpadAbsolute = Vector2.zero;
                lHandInput.trackpadNormalised = Vector2.zero;
            }
            else
            {
                lHandInput.trackpadNormalised = lHandInput.trackpadAbsolute.normalized;
            }

            //  TRIGGER
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                //GetComponent<ScreenshotTool>().Capture();
                TryGrab(lHoverObject, lHandTransform, ref lGrabbedObject);
            }
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                TryRelease(ref lGrabbedObject, lHandTransform);
            }

            //  MENU BUTTON
            if (SteamVR_Input._default.inActions.Menu.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                if(portableHoloMap)
                {
                    portableHoloMap.Toggle();
                }
            }
        }
    }

    private void TrackpadMovement()
    {
        Vector2 input = Vector2.zero;
        Vector3 direction = Vector3.zero;
        input.x = lHandInput.trackpadAbsolute.x;
        input.y = lHandInput.trackpadAbsolute.y;
        switch (m_movementSettings.movementOrientation)
        {
            case MovementOrientation.head:
                input = RotateVector2(input, m_headTransform.localEulerAngles.y);
                break;
            case MovementOrientation.controller:
                break;
            default:
                break;
        }
        input.Normalize();
        input *= m_movementSettings.movementSpeed;
        direction.x = input.x; direction.z = input.y;
        direction *= Time.deltaTime;

        m_transform.position += direction;
    }
    private Vector2 RotateVector2(Vector2 _vector, float _angle)
    {
        float t = (_angle / 180.0f) * Mathf.PI;
        float newX = _vector.x * Mathf.Cos(t) + _vector.y * Mathf.Sin(t);
        float newY = _vector.y * Mathf.Cos(t) - _vector.x * Mathf.Sin(t);
        return new Vector2(newX, newY);
    }

    public void AddInteractable(Interactable _obj)
    {
        if(!m_interactables.Contains(_obj))
        {
            m_interactables.Add(_obj);
        }
    }
    public void RemoveInteractable(Interactable _obj)
    {
        if (m_interactables.Contains(_obj))
        {
            m_interactables.Remove(_obj);
        }

    }
    private void HoverInteractable()
    {
        if(rHoverObject)
        {
            if(rHoverObject.useCollider)
            {
                if(!rHoverObject.collider.bounds.Contains(rHandTransform.position))
                {
                    rHoverObject.HoverEnd(rHandTransform);
                    rHoverObject = null;
                }
            }
            else
            {
                if(Vector3.Distance(rHoverObject.transform.position, rHandTransform.position) > rHoverObject.grabRange)
                {
                    rHoverObject.HoverEnd(rHandTransform);
                    rHoverObject = null;
                }
            }
        }
        if (lHoverObject)
        {
            if (lHoverObject.useCollider)
            {
                if (!lHoverObject.collider.bounds.Contains(lHandTransform.position))
                {
                    lHoverObject.HoverEnd(lHandTransform);
                    lHoverObject = null;
                }
            }
            else
            {
                if (Vector3.Distance(lHoverObject.transform.position, lHandTransform.position) > lHoverObject.grabRange)
                {
                    lHoverObject.HoverEnd(lHandTransform);
                    lHoverObject = null;
                }
            }
        }

        foreach (Interactable obj in m_interactables)
        {
            //  RIGHT HAND
            if(!rGrabbedObject)
            {
                if (obj.collider.bounds.Contains(rHandTransform.position))
                {
                    if(rHoverObject)
                    {
                        if (Vector3.Distance(obj.transform.position, rHandTransform.position) < Vector3.Distance(rHoverObject.transform.position, rHandTransform.position))
                        {
                            rHoverObject.HoverEnd(rHandTransform);
                            rHoverObject = obj;
                        }
                    }
                    else
                    {
                        rHoverObject = obj;
                    }
                }
            }

            //  LEFT HAND
            if (!lGrabbedObject)
            {
                if (obj.collider.bounds.Contains(lHandTransform.position))
                {
                    if (lHoverObject)
                    {
                        if (Vector3.Distance(obj.transform.position, lHandTransform.position) < Vector3.Distance(lHoverObject.transform.position, lHandTransform.position))
                        {
                            lHoverObject.HoverEnd(lHandTransform);
                            lHoverObject = obj;
                        }
                    }
                    else
                    {
                        lHoverObject = obj;
                    }
                }
            }
        }

        if (rHoverObject) rHoverObject.HoverStart(rHandTransform);
        if (lHoverObject) lHoverObject.HoverStart(lHandTransform);
    }

    private void TryGrab(Interactable _targetObject, Transform _handTransform, ref GrabableObject _grabbedObject)
    {
        if (_targetObject is GrabableObject)
        {
            if (_targetObject && !_grabbedObject)
            {
                GrabableObject obj = _targetObject as GrabableObject;
                _grabbedObject = obj.Grab(_handTransform);
            }
        }
    }
    private void TryRelease(ref GrabableObject _grabbedObject, Transform _hand)
    {
        if(_grabbedObject) _grabbedObject.Release(_hand);
        _grabbedObject = null;
    }

    private enum Hand { right, left }

    private void UpdateWristUi()
    {
        int numBars = 20;
        float fillAmount = 100.0f/(float)numBars;

        //  OXYGEN
        fillAmount = (float)(Mathf.Ceil(numBars * (oxygen / 100)) * (100 / numBars))/100.0f;
        m_oxygenBar.fillAmount = fillAmount;
        if (oxygen <= 25.0f) { m_oxygenBar.color = resourceCritical; }
        else if (oxygen <= 50.0f) { m_oxygenBar.color = resourceLow; }
        else if (oxygen <= 75.0f) { m_oxygenBar.color = resourceMedium; }
        else { m_oxygenBar.color = resourceHigh; }
        m_oxygenIcon.color = m_oxygenBar.color;

        //  POWER
        fillAmount = (float)(Mathf.Ceil(numBars * (power / 100)) * (100 / numBars)) / 100.0f;
        m_powerBar.fillAmount = fillAmount;
        if (power <= 25.0f) { m_powerBar.color = resourceCritical; }
        else if (power <= 50.0f) { m_powerBar.color = resourceLow; }
        else if (power <= 75.0f) { m_powerBar.color = resourceMedium; }
        else { m_powerBar.color = resourceHigh; }
        m_powerIcon.color = m_powerBar.color;

        if(heartRateImage)
        {
            Rect uvRect = heartRateImage.uvRect;
            if (uvRect.x >= 1.0f) uvRect.x = 0.0f;
            uvRect.x += heartRate/60 * Time.deltaTime;
            heartRateImage.uvRect = uvRect;
        }
    }

    public void SetOxygenLevel(float _level)
    {
        oxygen = Mathf.Clamp(_level, 0.0f, 100.0f);
    }
    public void IncreaseOxygenLevel(float _amount)
    {
        SetOxygenLevel(oxygen + _amount);
    }
    public void DecreaseOxygenLevel(float _amount)
    {
        SetOxygenLevel(oxygen - _amount);
    }

    public void SetPowerLevel(float _level)
    {
        power = Mathf.Clamp(_level, 0.0f, 100.0f);
    }
    public void IncreasePowerLevel(float _amount)
    {
        SetPowerLevel(power + _amount);
    }
    public void DecreasePowerLevel(float _amount)
    {
        SetPowerLevel(power - _amount);
    }

    private void PlayerBounds()
    {
        if (forceVignette || (m_useBounds && Physics.CheckSphere(m_headTransform.position, m_boundsRadius, m_boundsMask)) && m_useBounds)
        {
            m_vignetteFade = Mathf.MoveTowards(m_vignetteFade, 1.0f, m_boundsFadeSpeed * Time.deltaTime);
            if (m_vignetteFade >= 1.0f)
            {
                m_vrCam.cullingMask = new LayerMask();
                m_vrCam.clearFlags = CameraClearFlags.SolidColor;
            }
        }
        else
        {
            m_vignetteFade = Mathf.MoveTowards(m_vignetteFade, 0.0f, m_boundsFadeSpeed * Time.deltaTime);
            if (m_vignetteFade < 1.0f)
            {
                m_vrCam.cullingMask = m_originalCamLayers;
                m_vrCam.clearFlags = CameraClearFlags.Skybox;
            }
        }

        
        m_postProfile.GetSetting<Vignette>().opacity.value = m_vignetteFade;
    }

    private void OxygenDeprivation()
    {
        if(oxygen <= 0.0f)
        {
            m_oxDeprivation += (100.0f / oxygenDeprivationTime) * Time.deltaTime;
            suffocating = true;
        }
        else
        {
            m_oxDeprivation -= 100 * Time.deltaTime;
            suffocating = false;
        }

        m_oxDeprivation = Mathf.Clamp(m_oxDeprivation, 0.0f, 100.0f);

        if (m_oxDeprivation >= 100.0f) deadCanvas.enabled = true;
        else deadCanvas.enabled = false;

        m_postProfile.GetSetting<ColorGrading>().saturation.value = -m_oxDeprivation;

        if(suffocating && !m_prevSuffocating && !m_playingSuffocatingClip)
        {
            playerSource.Stop();
            playerSource.loop = true;
            playerSource.clip = GetRandomClip(suffocationClips);
            playerSource.Play();
            m_playingSuffocatingClip = true;
            m_playingOxygenatedClip = false;
            m_prevSuffocating = true;
        }

        if (!suffocating && m_prevSuffocating && !m_playingOxygenatedClip)
        {
            playerSource.Stop();
            playerSource.loop = false;
            playerSource.clip = GetRandomClip(oxygenatedClips);
            playerSource.Play();
            m_playingSuffocatingClip = false;
            m_playingOxygenatedClip = true;
            m_prevSuffocating = false;
        }
    }

    private AudioClip GetRandomClip(List<AudioClip> _clips)
    {
        if (_clips.Count == 1) return _clips[0];
        int random = Random.Range(0, _clips.Count);
        return _clips[random];
    }

    private void PowerDeprivation()
    {
        if(power <= 0.0f)
        {
            wristUi.TurnOff();
        }
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public bool useTrackpad = true;
    public float movementSpeed = 2f;
    public MovementOrientation movementOrientation;
    public float teleportDistance = 1f;
}

[System.Serializable]
public class PlayerControllerInput
{
    public Vector2 trackpadAbsolute;
    public Vector2 trackpadNormalised;
}

public enum MovementOrientation
{
    world,
    head,
    controller
}