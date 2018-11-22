using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonBehaviour : MonoBehaviour {
    Animator m_Animator;
    public bool isDoorOpen = false;
	// Use this for initialization
	void Start () {
        m_Animator = GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        print("Pressed!");
        switch (isDoorOpen)
        {
            case false:
                {
                    m_Animator.Play("Door_Open");
                    isDoorOpen = true;
                    break;
                }
            case true:
                {
                    m_Animator.Play("Door_Close");
                    isDoorOpen = false;
                    break;
                }
        }
    }
}
