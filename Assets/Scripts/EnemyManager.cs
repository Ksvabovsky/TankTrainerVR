using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public List<EnemyController> enemies; 

    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    public List<EnemyController> GetEnemies()
    {
        return enemies;
    }
}
