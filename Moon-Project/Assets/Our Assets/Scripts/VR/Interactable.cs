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
    public List<Renderer> renderers;
    [HideInInspector]
    public Rigidbody body;                  //  The interactable's rigidbody component
    [HideInInspector]
    public Color originalColour;            //  The original colour of the interactable's material

    public virtual void HoverStart(Transform _hand)
    {
        if (isHovered) return;
        foreach(Renderer r in renderers)
        {
            r.material.SetInt("_ShowOutline", 1);
        }
        isHovered = true;
    }
    public virtual void HoverEnd(Transform _hand)
    {
        if (!isHovered) return;
        foreach (Renderer r in renderers)
        {
            r.material.SetInt("_ShowOutline", 0);
        }
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

        body = GetComponent<Rigidbody>();
    }

    public void CheckRange()
    {
        if(Vector2.Distance(StaticMethods.TopDown(player.rHandTransform.position), StaticMethods.TopDown(transform.position)) <= checkRange
        || Vector2.Distance(StaticMethods.TopDown(player.lHandTransform.position), StaticMethods.TopDown(transform.position)) <= checkRange)
        {
            player.AddInteractable(this);
        }
        else
        {
            player.RemoveInteractable(this);
        }
    }

    public virtual void InteractableUpdate()
    {

    }
}