using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int equipped;
    public GameObject shield, balloon, scroll, torch, thrownBalloon, blocker, milk;
    public Image uiShield, uiBalloon, uiScroll, uiShieldMeter;
    public Text uiBalloonCount, uiTime, uiObjective, uiScrollUsed;
    public bool invisible, chalice, fire = false;
    public Image InvisOverlay, FireOverlay;
    public PlayerMovement mover;
    public MeshRenderer exit;
    public Material exitMaterial;
    private float shieldHeat, inputCooldown, invisibleTime, time, timeOffset, fireTime;
    private int balloonInventory;
    private bool scrollUsed, shieldDisabled, win = false;
    // Start is called before the first frame update
    void Start()
    {
        equipped = 1;
        UpdateUIInventory();
        shieldHeat = 0;
        balloonInventory = 5;
        inputCooldown = Time.time;
        timeOffset = Time.time;
        time = 91;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.paused)
        {
            return;
        }
        if (inputCooldown < Time.time && inputCooldown >= 0)
        {
            if (Input.GetAxisRaw("Equip1") > 0.9f && !shieldDisabled && equipped != 1)
            {
                equipped = 1;
                UpdateUIInventory();
                shield.SetActive(true);
                balloon.SetActive(false);
                scroll.SetActive(false);
                inputCooldown = Time.time + 0.25f;
            }
            else if (Input.GetAxisRaw("Equip1") > 0.9f && equipped == 1)
            {
                equipped = 0;
                UpdateUIInventory();
                shield.SetActive(false);
                inputCooldown = Time.time + 0.25f;
            }
            if (Input.GetAxisRaw("Equip2") > 0.9f && balloonInventory > 0 && equipped != 2)
            {
                equipped = 2;
                UpdateUIInventory();
                shield.SetActive(false);
                balloon.SetActive(true);
                scroll.SetActive(false);
                inputCooldown = Time.time + 0.25f;
            }
            else if (Input.GetAxisRaw("Equip2") > 0.9f && equipped == 2)
            {
                equipped = 0;
                UpdateUIInventory();
                balloon.SetActive(false);
                inputCooldown = Time.time + 0.25f;
            }
            if (Input.GetAxisRaw("Equip3") > 0.9f && !scrollUsed && equipped != 3)
            {
                equipped = 3;
                UpdateUIInventory();
                shield.SetActive(false);
                balloon.SetActive(false);
                scroll.SetActive(true);
                inputCooldown = Time.time + 0.25f;
            }
            else if (Input.GetAxisRaw("Equip3") > 0.9f && equipped == 3)
            {
                equipped = 0;
                UpdateUIInventory();
                scroll.SetActive(false);
                inputCooldown = Time.time + 0.25f;
            }

            if (Input.GetAxisRaw("Fire1") > 0.9f && equipped == 1)
            {
                torch.GetComponent<Animator>().SetTrigger("Noise");
                inputCooldown = Time.time + 0.25f;
            }
            if (Input.GetAxisRaw("Fire2") > 0.9f && equipped == 1)
            {
                shield.transform.localPosition = new Vector3(0, 0.1f);
                blocker.SetActive(true);
                blocker.transform.position = shield.transform.position;
                blocker.transform.rotation = shield.transform.rotation;
                mover.speed = mover.baseSpeed / 4;
            }
            else
            {
                shield.transform.localPosition = Vector3.zero;
                blocker.SetActive(false);
                mover.speed = mover.baseSpeed;
            }
            if (shieldHeat > 3)
            {
                shieldHeat = 3;
                shieldDisabled = true;
                uiShieldMeter.color = new Color(1, 0, 0, 0.5f);
                if (equipped == 1)
                {
                    equipped = 0;
                    UpdateUIInventory();
                    shield.SetActive(false);
                }
            }
            if (shieldHeat > 0)
            {
                shieldHeat -= Time.deltaTime / 3;
            }
            if (shieldHeat < 0)
            {
                shieldHeat = 0;
                shieldDisabled = false;
                uiShieldMeter.color = new Color(1, 0.5f, 0.5f, 0.5f);
            }
            uiShieldMeter.transform.localScale = new Vector3(shieldHeat / 3f, 1, 1);

            if (Input.GetAxisRaw("Fire1") > 0.9f && equipped == 2)
            {
                Instantiate(thrownBalloon, gameObject.transform.position + transform.forward, new Quaternion(GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.x, GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w));
                balloon.SetActive(false);
                balloonInventory--;
                UpdateBalloonCount();
                equipped = 0;
                UpdateUIInventory();
                inputCooldown = Time.time + 0.25f;
            }
            if (Input.GetAxisRaw("Fire2") > 0.9f && equipped == 2)
            {
                balloon.transform.localPosition = new Vector3(0, 0.25f);
                inputCooldown = Time.time + 0.25f;
                StartCoroutine(SelfPop());
            }

            if (Input.GetAxisRaw("Fire1") > 0.9f && equipped == 3)
            {
                scroll.GetComponent<Animator>().SetTrigger("Open");
                inputCooldown = Time.time + 1f;
                StartCoroutine(HideScroll());
            }
        }
        if (invisible && invisibleTime < Time.time)
        {
            invisible = false;
            InvisOverlay.gameObject.SetActive(false);
        }
        if (fire)
        {
            fireTime += Time.deltaTime;
        }
        if (!win)
        {
            FireOverlay.color = new Color(1, 0.5f, 0, fireTime / 5);
        }
        if (fireTime > 5)
        {
            time = -1;
            inputCooldown = -1;
        }
        if (fireTime > 7)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
        uiTime.text = time > 0 ? Mathf.Floor(time + timeOffset - Time.time).ToString() : "0";

        if (time + timeOffset < Time.time && time > 0)
        {
            transform.position = GameObject.FindGameObjectWithTag("Guard").transform.position + new Vector3(0,1,1);
        }
    }

    private void UpdateUIInventory()
    {
        uiShield.color = new Color(1, 1, 1, 0.2f);
        uiBalloon.color = new Color(1, 1, 1, 0.2f);
        uiScroll.color = new Color(1, 1, 1, 0.2f);

        if (equipped == 1)
        {
            uiShield.color = new Color(1, 1, 1, 0.7f);
        }
        if (equipped == 2)
        {
            uiBalloon.color = new Color(1, 1, 1, 0.7f);
        }
        if (equipped == 3)
        {
            uiScroll.color = new Color(1, 1, 1, 0.7f);
        }
    }

    public void ShieldBlock()
    {
        shieldHeat++;
    }

    private void UpdateBalloonCount()
    {
        uiBalloonCount.text = "x" + balloonInventory;
        if (balloonInventory == 0)
        {
            uiBalloonCount.color = new Color(0.9f, 0.1f, 0.1f);
        }
        else
        {
            uiBalloonCount.color = Color.white;
        }
    }

    IEnumerator SelfPop()
    {
        yield return new WaitForSeconds(0.25f);
        Instantiate(milk, balloon.transform.position, balloon.transform.rotation);
        fire = false;
        fireTime = 0;
        balloon.transform.localPosition = new Vector3(0, 0.1f);
        balloon.SetActive(false);
        balloonInventory--;
        UpdateBalloonCount();
        equipped = 0;
        UpdateUIInventory();
        yield break;
    }

    IEnumerator HideScroll()
    {
        yield return new WaitForSeconds(1f);
        scroll.SetActive(false);
        InvisOverlay.gameObject.SetActive(true);
        invisibleTime = Time.time + 5f;
        scrollUsed = true;
        invisible = true;
        equipped = 0;
        uiScrollUsed.gameObject.SetActive(true);
        UpdateUIInventory();
        yield break;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Guard"))
        {
            if (!collision.collider.GetComponent<GuardController>().stun)
            {
                time = -1;
                equipped = 0;
                UpdateUIInventory();
                GetComponent<PlayerMovement>().speed = 0;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseLook>().enabled = false;
                GameObject.FindGameObjectWithTag("MainCamera").transform.localRotation = new Quaternion(0, 0, 0, 0);
                inputCooldown = -1;
                Destroy(torch);
                Destroy(balloon);
                Destroy(shield);
                Destroy(scroll);
            }
            else
            {
                StartCoroutine(Bounce());
            }
        }
        if (collision.collider.CompareTag("Fire"))
        {
            if (fire)
            {
                fireTime += 0.5f;
            }
            else
            {
                fire = true;
            }
        }
        if (collision.collider.CompareTag("Exit"))
        {
            if (chalice)
            {
                inputCooldown = -1;
                win = true;
                StartCoroutine(Win());
            }
            else
            {
                StartCoroutine(FlashObjective(false));
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Chalice"))
        {
            chalice = true;
            uiObjective.text = "Objective: Escape by reaching the entrance!";
            Destroy(collider.gameObject);
            StartCoroutine(FlashObjective(true));
            exit.material = exitMaterial;
            GameObject.FindGameObjectWithTag("Guard").GetComponent<GuardController>().RandomHotSpotsOn();
        }
        if (collider.CompareTag("Fire"))
        {
            if (fire)
            {
                fireTime += 0.5f;
            }
            else
            {
                fire = true;
            }
        }
    }

    IEnumerator Win()
    {
        for(float i = 0; i < 121; i++)
        {
            fire = false;
            fireTime = 0;
            FireOverlay.color = new Color(1, 1, 1, i / 120);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        yield break;
    }

    IEnumerator FlashObjective(bool chal)
    {
        if (chal)
        {
            for (float i = 1; i < 31; i++)
            {
                Vector3 col = Vector3.Slerp(Vector3.one, new Vector3(1, 0.7f, 0.2f), i / 30);
                uiObjective.color = new Color(col.x, col.y, col.z);
                yield return new WaitForEndOfFrame();
            }
            for (float i = 1; i < 61; i++)
            {
                Vector3 col = Vector3.Slerp(new Vector3(1, 0.7f, 0.2f), Vector3.one, i / 60);
                uiObjective.color = new Color(col.x, col.y, col.z);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            for (int q = 3; q > 0; q--)
            {
                for (float i = 1; i < 31; i++)
                {
                    Vector3 col = Vector3.Slerp(Vector3.one, new Vector3(1, 0.5f, 0.1f), i / 30);
                    uiObjective.color = new Color(col.x, col.y, col.z);
                    yield return new WaitForEndOfFrame();
                }
                for (float i = 1; i < 31; i++)
                {
                    Vector3 col = Vector3.Slerp(new Vector3(1, 0.5f, 0.1f), Vector3.one, i / 30);
                    uiObjective.color = new Color(col.x, col.y, col.z);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        yield break;
    }

    IEnumerator Bounce()
    {
        for(int i = 45; i > 0; i--)
        {
            gameObject.GetComponent<CharacterController>().Move((transform.forward * -5 - transform.forward * Mathf.Sqrt(i)) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
