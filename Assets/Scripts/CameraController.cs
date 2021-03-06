﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject mCharacter;
    private Vector3 mLowerLimit;
    public Vector3 mOffset;
    public float mDificultyScale;
    public Vector3 mInitialPos;
	private Camera camera;

    // Use this for initialization
    private void Start () {
		camera = GetComponent<Camera> ();
        mInitialPos = this.transform.position;
    }

	
    private float ComputeCameraSpeed(float x) 
    {
		// primeste ca input distanta dintre limita inferioara si player si returneaza viteza cu care camera se ridica.
        float rvalue = 1;	// valoarea returnata, 1 = nu are nici un efect.
       	
        if (x > Mathf.Epsilon)	// daca distanta e foarte aproape de limita inferioara camera se ridica cu viteza implicita  
        {
        	rvalue = Mathf.Pow(2.14f,(x / 1f));	// functie exponentiala ce are graficul asemanator cu e^x
        }

        return rvalue;
    }
    

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

    private void FixedUpdate()
    {
        mLowerLimit = transform.position + mOffset;
        PlayerController pcontroller = mCharacter.GetComponent<PlayerController>();

        GhemAgent agent = mCharacter.GetComponentInChildren<GhemAgent>();

        if ((pcontroller && pcontroller.mJumpedOnce) || (agent && agent.mJumpedOnce))
        {
            transform.position += Vector3.up * Time.fixedDeltaTime * mDificultyScale * ComputeCameraSpeed(mCharacter.transform.position.y - mLowerLimit.y);
        }

    }
}
