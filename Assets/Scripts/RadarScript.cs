using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.XR.CoreUtils;
using UnityEngine;

public class RadarScript : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] ProtoTankController playerController;
    [SerializeField] EnemyManager enemyManager;

    [SerializeField] Transform PlayerTriangle;
    [SerializeField] GameObject enemyTrianlgePrefab;
    [SerializeField] List<EnemyController> enemies;

    [SerializeField] float maxDist;
    [SerializeField] float multiplier;

    [Header("PlayerLock")]

    [SerializeField] bool Locked;
    Coroutine lockCoroutine;
    [SerializeField] Material material;
    //[SerializeField] AudioClip lockSound;
    //[SerializeField] AudioClip newContact;
    [SerializeField] AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        player = ProtoTankController.instance.transform;
        playerController = ProtoTankController.instance;
        enemyManager = EnemyManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        bool isLocked = false;

        enemies = enemyManager.GetEnemies();
        foreach(EnemyController enemy in enemies)
        {
            if (enemy.IsPlayerLocked())
            {
                isLocked = true;
            }
            if (enemy.enabled)
            {
                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distance < maxDist)
                {

                    if (enemy.square == null)
                    {
                        enemy.square = Instantiate(enemyTrianlgePrefab, this.transform);
                        enemy.square.SetActive(true);
                        enemy.direction = enemy.square.transform.GetChild(0).gameObject;
                    }
                    if (enemy.square)
                    {
                        Vector3 dist = player.transform.position - enemy.transform.position;
                        dist = dist / multiplier;
                        enemy.square.transform.localPosition = PlayerTriangle.localPosition + new Vector3(dist.x, 0f, dist.z);
                        enemy.direction.transform.localEulerAngles = new Vector3(0f, enemy.transform.eulerAngles.y, 0f);
                    }

                }
                else
                {
                    if (enemy.square != null)
                    {
                        Destroy(enemy.square);
                    }
                }
            }
            else
            {
                if (enemy.square != null)
                {
                    Destroy(enemy.square);
                }
            }


        }
        PlayerTriangle.localEulerAngles = new Vector3(0f, player.eulerAngles.y, 0f);

        if (isLocked)
        {
            PlayerLocked();
        }
        else
        {
            PlayerUnlocked();
        }
    }

    private void OnDisable()
    {
        enemies = enemyManager.GetEnemies();
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.square != null)
            {
                Destroy(enemy.square);
            }
        }
    }

    public void PlayerLocked()
    {
        if (!Locked)
        {
            Locked = true;
            if (lockCoroutine == null)
            {
                lockCoroutine = StartCoroutine(LockSound());
                material.EnableKeyword("_EMISSION");
            }
        }
    }

    public void PlayerUnlocked()
    {
        if (Locked)
        {
            Locked = false;
            if (lockCoroutine != null)
            {
                StopCoroutine(lockCoroutine);
                lockCoroutine = null;
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ClearWarningLight()
    {
        if (lockCoroutine != null)
        {
            StopCoroutine(lockCoroutine);
            lockCoroutine = null;
        }
        material.DisableKeyword("_EMISSION");
    }

    IEnumerator LockSound()
    {

        //source.Play();
        //yield return new WaitForSeconds(0.25f);
        //material.DisableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.EnableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.DisableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.EnableKeyword("_EMISSION");
        //yield return new WaitForSeconds(2f);

        //source.clip = lockSound;
        while (true)
        {

            source.Play();
            yield return new WaitForSeconds(3f);
        }
    }

}
