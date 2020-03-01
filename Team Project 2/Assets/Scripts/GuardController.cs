using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    public NavMeshAgent agent;
    public PlayerController player;
    public GameObject sight, breath, fireball;
    public Light mouthLight;
    public float baseSpeed;
    private float haste, stunTime, searchTime, fireballCooldown, spitTime, breathTime;
    public bool stun;
    private bool kill;
    private Animator animator;
    private GameObject[] hotspots;
    // Start is called before the first frame update
    void Start()
    {
        hotspots = GameObject.FindGameObjectsWithTag("HotSpot");
        haste = Time.time;
        stunTime = Time.time;
        searchTime = Time.time;
        stun = false;
        animator = GetComponent<Animator>();
        RandomHotSpotsOn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Fire1") > 0.5 && player.equipped == 1)
        {
            agent.SetDestination(player.transform.position);
        }
        if (LineOfSight())
        {
            agent.SetDestination(player.transform.position);
            if (fireballCooldown < Time.time && !animator.GetBool("Spit"))
            {
                agent.isStopped = true;
                animator.SetBool("Walking", false);
                animator.SetBool("Spit", true);
                spitTime = Time.time + 1.6f + Random.value * 1.5f;
                StartCoroutine(FaceMe(player.gameObject));
            }
            else if (fireballCooldown > Time.time)
            {
                SetSpeed();
                haste = Time.time + 2f;
            }
        }

        if (spitTime < Time.time && animator.GetBool("Spit"))
        {
            animator.SetBool("Spit", false);
            fireballCooldown = Time.time + 1f + Random.value * 3;
        }
        if (haste < Time.time)
        {
            SetSpeed();
        }

        if (stunTime > Time.time)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }
        else
        {
            stun = false;
        }
        if ((agent.remainingDistance <= agent.stoppingDistance) && searchTime < Time.time && !kill && !animator.GetBool("Spit") && breathTime < Time.time)
        {
            agent.isStopped = true;
            animator.SetBool("Walking", false);
            if (Vector3.Distance(transform.position, ClosestHotSpot().transform.position) <= agent.stoppingDistance * 2 && breathTime < Time.time)
            {
                animator.SetTrigger("Breath");
                FaceMe(ClosestHotSpot());
                breathTime = Time.time + 3f;
            }
            else
            {
                animator.SetTrigger("Searching");
                searchTime = Time.time + 1.5f;
            }
        }
        if (searchTime > Time.time && (LineOfSight() || agent.remainingDistance > agent.stoppingDistance) && !kill && !animator.GetBool("Spit"))
        {
            agent.isStopped = false;
            animator.SetBool("Walking", true);
        }
        if (stunTime < Time.time && searchTime < Time.time && !kill && !animator.GetBool("Spit") && breathTime < Time.time)
        {
            DeactivateBreath();
            agent.isStopped = false;
            animator.SetBool("Walking", true);
        }

        if (kill)
        {
            player.gameObject.transform.localPosition = new Vector3(0, player.gameObject.transform.localPosition.y > 2.06f ? 2.1f : player.gameObject.transform.localPosition.y + 0.04f, 1.5f);
            player.gameObject.transform.localRotation = new Quaternion();
            player.gameObject.transform.Rotate(new Vector3(0, 180));
        }
    }

    void SetSpeed()
    {
        float targetSpeed = baseSpeed;
        if (player.chalice)
        {
            targetSpeed *= 1.5f;
        }
        if (LineOfSight())
        {
            targetSpeed *= 1.5f;
        }
        agent.speed = targetSpeed;
        animator.SetFloat("SpeedMult", (1 + targetSpeed / baseSpeed));
    }

    bool LineOfSight()
    {
        if (!player.invisible && !stun)
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
        }
        return false;
    }

    GameObject RollHotSpot()
    {
        GameObject candidate = null;
        bool success = false;
        int i = 0;
        while (!success)
        {
            candidate = hotspots[Random.Range(0, hotspots.Length)];
            if (!candidate.transform.GetChild(0).gameObject.activeSelf)
            {
                success = true;
            }
            if (i > 99)
            {
                success = true;
            }
            i++;
        }
        return candidate;
    }

    GameObject ClosestHotSpot()
    {
        float distance = 20;
        GameObject candidate = null;
        foreach (GameObject q in hotspots)
        {
            if (Vector3.Distance(transform.position, q.transform.position) < distance && !q.transform.GetChild(0).gameObject.activeSelf)
            {
                distance = Vector3.Distance(transform.position, q.transform.position);
                candidate = q;
            }
        }
        if (candidate == null)
        {
            candidate = hotspots[0];
        }
        return candidate;
    }

    void Searching()
    {
        agent.SetDestination(RollHotSpot().transform.position);
        agent.isStopped = false;
        animator.SetBool("Walking", true);
    }

    void HotSpotOn()
    {
        ClosestHotSpot().transform.GetChild(0).gameObject.SetActive(true);
    }

    public void RandomHotSpotsOn()
    {
        foreach (GameObject q in hotspots)
        {
            if (Random.value * 2 > 1)
            {
                q.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Balloon"))
        {
            stun = true;
            animator.SetTrigger("Stunned");
            stunTime = Time.time + 1.25f;
            if (animator.GetBool("Spit"))
            {
                animator.SetBool("Spit", false);
                fireballCooldown = Time.time + 4 + Random.value * 3;
            }
        }

        if (collision.collider.CompareTag("Player") && !stun)
        {
            kill = true;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetTrigger("Kill");
            collision.collider.transform.parent = gameObject.transform;
            collision.collider.transform.localPosition = new Vector3(0, 2f, 1.5f);
            collision.collider.transform.localRotation = new Quaternion();
            collision.collider.transform.Rotate(new Vector3(0, 180));
            collision.collider.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    void ActivateBreath()
    {
        breath.SetActive(true);
        mouthLight.range = 5;
    }

    void DeactivateBreath()
    {
        breath.SetActive(false);
        mouthLight.range = 0.5f;
    }

    void SpawnFireball()
    {
        Instantiate(fireball, mouthLight.transform.position, new Quaternion());
    }

    void Ignite()
    {
        player.fire = true;
    }

    IEnumerator FaceMe(GameObject target)
    {
        for (int i = 0; i < 31; i++)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), i / 30);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
