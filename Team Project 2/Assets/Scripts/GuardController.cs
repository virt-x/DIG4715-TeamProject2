using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player, sight;
    private float haste;
    // Start is called before the first frame update
    void Start()
    {
        haste = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Fire1") > 0.5)
        {
            agent.SetDestination(player.transform.position);
        }
        if (LineOfSight())
        {
            agent.SetDestination(player.transform.position);
            agent.speed = 7f;
            haste = Time.time + 1f;
        }
        if (haste < Time.time)
        {
            agent.speed = 5f;
        }
    }

    bool LineOfSight()
    {
        RaycastHit hit;
        Vector3 dir = player.transform.position - sight.transform.position;
        Debug.DrawRay(sight.transform.position, dir, Color.green);
        float sightAngle = Vector3.Angle((sight.transform.position - transform.position), (player.transform.position - transform.position));
        if (sightAngle < 60f && Physics.Raycast(sight.transform.position, dir, out hit, Vector3.Distance(sight.transform.position, player.transform.position)))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
