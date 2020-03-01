using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotController : MonoBehaviour
{
    private float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime < Time.time)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        lifetime = Time.time + 10 + Random.value * 15;
    }
}
