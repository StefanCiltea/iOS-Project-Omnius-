using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveParent : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.Compare(collision.gameObject.name, "GarbageCollector") == 0)
        {

            GameObject parent = transform.parent.gameObject;

            if(parent)
            {
                foreach(Transform child in this.transform.parent.parent)
                {
                    if (this.transform.parent.gameObject.tag == "ClonaObstacol" && child.tag == "Agent")
                    {
                        GhemAgent script = child.gameObject.GetComponent<GhemAgent>();
                        script.onObstacleDestroyed();
                    }
                }
                Destroy(parent);
            }
        }

    }
}
