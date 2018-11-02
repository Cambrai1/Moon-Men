using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabableObject : MonoBehaviour {

    public string name;
    public Rigidbody body;
    public bool isGrabbed;

    private void Start()
    {
        if (name == "") name = gameObject.name;
        if (!body) body = GetComponent<Rigidbody>();
        isGrabbed = false;
    }
}