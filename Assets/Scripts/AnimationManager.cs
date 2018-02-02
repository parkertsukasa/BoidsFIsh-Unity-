using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {

    private Vector3 pastpos = Vector3.zero;//1フレーム前の位置
    private Vector3 nowpos = Vector3.zero;//今の位置

    private Animator anim;
    private enum state{forward, right, left}
    private state animstate;

	// Use this for initialization
	void Start ()
	{
        anim = gameObject.transform.Find("Model").GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        pastpos = nowpos;
        nowpos = transform.position;
        Vector3 move = nowpos - pastpos;
        float angle = Vector3.Angle(transform.forward, move);
        if (angle > 15)
            animstate = state.right;
        else if (angle < -15)
            animstate = state.left;
        else
            animstate = state.forward;


        switch(animstate)
        {
            case state.forward:
                anim.SetBool("Forward", true);
                anim.SetBool("Right", false);
                anim.SetBool("Left", false);
                break;
            case state.right:
                anim.SetBool("Forward", false);
                anim.SetBool("Right", true);
                anim.SetBool("Left", false);
                break;
            case state.left:
                anim.SetBool("Forward", false);
                anim.SetBool("Right", false);
                anim.SetBool("Left", true);
                break;
        }
	}
}
