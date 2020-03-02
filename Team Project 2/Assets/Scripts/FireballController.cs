using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody body;
    public AudioSource soundBlock;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        body = gameObject.GetComponent<Rigidbody>();
        body.AddForce((Vector3.Normalize(player.transform.position - transform.position) * 50) + new Vector3((Random.value - 0.5f) * 1.5f, (Random.value - 0.5f) * 1.5f, (Random.value - 0.5f) * 1.5f), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Shield"))
        {
            Instantiate(soundBlock, transform.position, transform.rotation);
            player.GetComponent<PlayerController>().ShieldBlock();
            Destroy(gameObject);
        }
    }
}
