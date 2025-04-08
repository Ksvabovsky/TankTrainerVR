using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public int NumberOfEnemies = 0;
    public int enemiesDestroyed = 0;

    public List<EnemyController> enemies; 
    

    public static EnemyManager instance;
    GameStateManager gameStateManager;

    public TMP_Text objectiveText;

    private void Awake()
    {
        instance = this;
        gameStateManager = GetComponent<GameStateManager>();
    }

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
        NumberOfEnemies++;
        objectiveText.text = "0/" + NumberOfEnemies.ToString();

    }

    public void RemoveEnemy(int id)
    {
        enemiesDestroyed++;
        objectiveText.text = enemiesDestroyed.ToString() + "/"+ NumberOfEnemies.ToString();
        if(gameStateManager.GetGameState() == GameState.Playing && enemiesDestroyed == NumberOfEnemies)
        {
            gameStateManager.GameFinished();
        }
    }

    public List<EnemyController> GetEnemies()
    {
        return enemies;
    }

    public void MissionOver()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.MissonOver();
        }
    }
}
