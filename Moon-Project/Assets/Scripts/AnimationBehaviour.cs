using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviour : MonoBehaviour {

    Animator m_Animator;
    float state = 0;

	void Start () {
        m_Animator = GetComponent<Animator>();
	}
	

	void Update () {
        if (Input.GetKey(KeyCode.X) && (state == 0))
        {
                m_Animator.Play("Door_Open");
                state = 1;           
        }
        if (Input.GetKey(KeyCode.Z) && (state == 1))
        {
                m_Animator.Play("Door_Close");
                state = 0;
        }
	}

}
