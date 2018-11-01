using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private PlayerMovementSettings m_movement;

    private Vector2 m_movementInput;
    private Vector2 m_normalisedInput;

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {

    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public float movementSpeed = 1f;
    public float teleportDistance = 1f;
}