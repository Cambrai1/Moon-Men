using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DoorController : MonoBehaviour
{

    [SerializeField]
    private bool m_open = true;
    [SerializeField]
    private bool m_locked = false;
    private bool m_animating = false;

    public AnimationCurve curve;

    public Transform doorTransform;
    public Vector3 closedPos, openPos;

    public bool toggle = false;

    private void Update()
    {
        if(toggle)
        {
            ToggleDoor();
            toggle = false;
        }
    }

    public void OpenDoor()
    {
        if (m_locked) return;
        if (m_animating) return;
        StartCoroutine(AnimateDoor(0.0f, 1.0f));
        m_open = true;
    }
    public void CloseDoor()
    {
        if (m_locked) return;
        if (m_animating) return;
        StartCoroutine(AnimateDoor(1.0f, 0.0f));
        m_open = false;
    }

    public void ToggleDoor()
    {
        if (m_open) CloseDoor();
        else OpenDoor();
    }

    public IEnumerator AnimateDoor(float _start, float _end)
    {
        m_animating = true;
        float t = _start;
        if (_start < _end)
        {
            doorTransform.localPosition = closedPos;
            while (t <= 1.0f)
            {
                doorTransform.localPosition = Vector3.Lerp(closedPos, openPos, curve.Evaluate(t));
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            doorTransform.localPosition = openPos;
        }
        else
        {
            doorTransform.localPosition = openPos;
            while (t >= 0.0f)
            {
                doorTransform.localPosition = Vector3.Lerp(closedPos, openPos, curve.Evaluate(t));
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            doorTransform.localPosition = closedPos;
        }
        m_animating = false;
        yield return null;
    }
}