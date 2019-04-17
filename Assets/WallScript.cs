using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    public GameObject followTarget;
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (followTarget.transform.position.y > this.transform.position.y)
        {
            this.transform.position = new Vector3(this.transform.position.x, followTarget.transform.position.y);
        }
    }
}
