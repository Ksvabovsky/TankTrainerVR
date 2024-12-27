using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance;
    EnemyManager enemyManager;


    [SerializeField] private GameState gameState;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        enemyManager = GetComponent<EnemyManager>();

        gameState = GameState.NotStarted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStarted()
    {
        gameState = GameState.Playing;
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        enemyManager.MissionOver();
    }

}

public enum GameState
{
    NotStarted,
    Playing,
    GameOver,
}
