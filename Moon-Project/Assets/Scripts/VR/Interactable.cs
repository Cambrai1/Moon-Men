using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
//  CLASS IS USED FOR ALL INTERACTABLE OBJECTS
public class Interactable : MonoBehaviour {

    public bool isHovered;                  //  Is the interactable being hovered over by the player?
    public bool useCollider = true;         //  Should the collider be used to detect hover status?
    [HideInInspector]
    public Collider collider;               //  The collider of the interactable
    public float checkRange = 0.5f;         //  The range at which the interactable will add to the player's list
    public float grabRange = 0.4f;
    [HideInInspector]
    public PlayerController player;         //  A reference to the player controller
    [HideInInspector]
    public Renderer renderer;               //  The interactable's main renderer component
    [HideInInspector]
    public Rigidbody body;                  //  The interactable's rigidbody component
    [HideInInspector]
    public Color originalColour;            //  The original colour of the interactable's material

    public virtual void HoverStart(Transform _hand)
    {
        if (isHovered) return;
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", originalColour);
        isHovered = true;
    }
    public virtual void HoverEnd(Transform _hand)
    {
        if (!isHovered) return;
        renderer.material.SetColor("_EmissionColor", Color.black);
        isHovered = false;
    }

    private void Start()
    {
        InteractableInit();
    }

    private void Update()
    {
        CheckRange();
        InteractableUpdate();
    }

    public virtual void InteractableInit()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        collider = GetComponent<Collider>();
        if (!collider) useCollider = false;

        renderer = GetComponent<Renderer>();
        if (renderer) originalColour = renderer.material.color;

        body = GetComponent<Rigidbody>();
    }

    public void CheckRange()
    {
        if(Vector2.Distance(XYZToXZ(player.rHandTransform.position), XYZToXZ(transform.position)) <= checkRange
        || Vector2.Distance(XYZToXZ(player.lHandTransform.position), XYZToXZ(transform.position)) <= checkRange)
        {
            player.AddInteractable(this);
        }
        else
        {
            player.RemoveInteractable(this);
        }
    }

    public Vector2 XYZToXZ(Vector3 input)
    {
        Vector2 result = Vector2.zero;
        result.x = input.x;
        result.y = input.z;
        return result;
    }

    public virtual void InteractableUpdate()
    {

    }
}