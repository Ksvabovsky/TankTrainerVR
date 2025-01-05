using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class RadarScript : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] TankController playerController;
    [SerializeField] EnemyManager enemyManager;

    [SerializeField] Transform PlayerTriangle;
    [SerializeField] GameObject enemyTrianlgePrefab;
    [SerializeField] List<EnemyController> enemies;

    [SerializeField] float maxDist;
    [SerializeField] float multiplier;

    // Start is called before the first frame update
    void Start()
    {
        player = TankController.instance.transform;
        playerController = TankController.instance;
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

            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if(distance < maxDist) {
                if (enemy.square == null)
                {
                    enemy.square = Instantiate(enemyTrianlgePrefab,this.transform);
                    enemy.square.SetActive(true);
                    enemy.direction =  enemy.square.transform.GetChild(0).gameObject;
                }
                if (enemy.square)
                {
                    Vector3 dist = player.transform.position - enemy.transform.position;
                    dist = dist / multiplier;
                    enemy.square.transform.localPosition = PlayerTriangle.localPosition + new Vector3(dist.x, 0f,dist.z);
                    enemy.direction.transform.localEulerAngles = new Vector3( 0f, enemy.transform.eulerAngles.y,0f );
                }
            }
            else
            {
                if(enemy.square != null)
                {
                    Destroy(enemy.square);
                }
            }


        }
        PlayerTriangle.localEulerAngles = new Vector3(0f, player.eulerAngles.y, 0f);

        if (isLocked)
        {
            playerController.PlayerLocked();
        }
        else
        {
            playerController.PlayerUnlocked();
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

}
