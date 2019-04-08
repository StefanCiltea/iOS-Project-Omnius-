using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Spawner")
        {
            print(this);
            GameObject backgroundGameObject = this.gameObject;
            float height = this.GetComponent<SpriteRenderer>().bounds.size.y;
            Instantiate(backgroundGameObject, this.transform.position + new Vector3(0,height,0), backgroundGameObject.transform.rotation);
        }
    }
}
