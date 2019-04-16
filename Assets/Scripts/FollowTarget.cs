using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public GameObject targetToFollow;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (targetToFollow.transform.position.y > this.transform.position.y)
        {
            this.transform.position = new Vector3(this.transform.position.x, targetToFollow.transform.position.y);
        }    
    }
}
