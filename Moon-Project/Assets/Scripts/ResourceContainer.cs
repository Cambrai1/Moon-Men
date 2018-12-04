using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceContainer : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceQuantity;
    public Transform dialNeedle;

    private PlayerController m_player;
    private float m_dialSpeed = 100.0f;

    private void Start()
    {
        if (!m_player) m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        SetResource(resourceType, resourceQuantity);
    }

    public void SetResource(ResourceType _type, int _quantity)
    {
        resourceType = _type;
        resourceQuantity = _quantity;
        if(dialNeedle)
        {
            float angle = Mathf.Lerp(155.0f, 385.0f, resourceQuantity / 100.0f);
            dialNeedle.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void CollectResource()
    {
        if(m_player)
        {
            if (dialNeedle) { StartCoroutine(MoveDial(resourceQuantity)); }
            switch (resourceType)
            {
                case ResourceType.Oxygen:
                    m_player.IncreaseOxygenLevel((float)resourceQuantity);
                    break;
                case ResourceType.Power:
                    m_player.IncreasePowerLevel((float)resourceQuantity);
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator MoveDial(int _quantity)
    {
        bool done = false;
        while (!done)
        {
            Vector3 rot = dialNeedle.localEulerAngles;
            if (rot.z >= 155.0f)
            {
                rot.z -= m_dialSpeed * Time.deltaTime;
                dialNeedle.localEulerAngles = rot;
            }
            else done = true;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}

public enum ResourceType
{
    Oxygen,
    Power
}