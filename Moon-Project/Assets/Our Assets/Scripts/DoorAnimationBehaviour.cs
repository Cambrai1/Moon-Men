using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Animator))]

public class DoorAnimationBehaviour : MonoBehaviour {

    Animator m_Animator;
    public bool isDoorOpen = false;
    public bool isButtonControlled = false;
    public GameObject Player;
    public float dist;

	void Start()
    {
        if (!Player) return;

        m_Animator = GetComponent<Animator>();
        dist = Vector3.Distance(Player.transform.position, this.gameObject.transform.position);
        
        foreach (Transform child in transform)
        {
            if (child.name == "Door_Button")
            {
                isButtonControlled = true;
            }
        }
	}
	

	void Update()
    {
        if (!Player) return;

        if (isButtonControlled == false)
        {
            dist = Vector3.Distance(Player.transform.position, this.gameObject.transform.position);
            if ((dist < 5) && (isDoorOpen == false))
            {
                m_Animator.Play("Door_Open");
                isDoorOpen = true;
            }
            else if ((dist > 7) && (isDoorOpen == true))
            {
                m_Animator.Play("Door_Close");
                isDoorOpen = false;
            }
        }
        else
        {
            //check for button press and then relay door open action
        }
            
    }

}
