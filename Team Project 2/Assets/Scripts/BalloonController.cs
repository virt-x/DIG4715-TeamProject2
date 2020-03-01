using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    public Rigidbody body;
    public GameObject milk;
    // Start is called before the first frame update
    void Start()
    {
        body.AddForce(transform.forward * 5 + new Vector3(0,3,0), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Instantiate(milk, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.parent.CompareTag("HotSpot"))
        {
            collider.gameObject.SetActive(false);
            Instantiate(milk, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
