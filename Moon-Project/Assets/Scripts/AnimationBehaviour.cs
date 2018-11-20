using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Animator))]
public class AnimationBehaviour : MonoBehaviour {

    Animator m_Animator;
    float state = 0;
    public GameObject Player;
    public float dist;

	void Start () {
        m_Animator = GetComponent<Animator>();
        dist = Vector3.Distance(Player.transform.position, this.gameObject.transform.position);
        print(dist);
	}
	

	void Update () {
        dist = Vector3.Distance(Player.transform.position, this.gameObject.transform.position);
        if ((dist < 5) && (state == 0))
        {
            m_Animator.Play("Door_Open");
            state = 1;
        }
        else if ((dist > 7) && (state == 1))
        {
            m_Animator.Play("Door_Close");
            state = 0;
        }
    }

}
