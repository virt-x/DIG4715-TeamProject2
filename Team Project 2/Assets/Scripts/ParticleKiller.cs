using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour
{
    public float lifespan;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (time + lifespan < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
