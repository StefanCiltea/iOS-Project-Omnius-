using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveParent : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.Compare(collision.gameObject.name,"GarbageCollector") == 0)
        {
            GameObject parent = this.gameObject.transform.parent.gameObject;
            if(parent)
            {
                Destroy(parent);
            }
        }

    }
}
