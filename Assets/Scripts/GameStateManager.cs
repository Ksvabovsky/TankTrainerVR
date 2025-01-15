using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance;
    EnemyManager enemyManager;
    [SerializeField] private ProtoTankController player;


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

    public void GameStarted()
    {
        gameState = GameState.Playing;
    }

    public void GameFinished()
    {
        gameState = GameState.Completed;
        player.TankStop(gameState);

    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        player.TankStop(gameState);
        enemyManager.MissionOver();
    }

    public GameState GetGameState()
    {
        return gameState;
    }

}

public enum GameState
{
    NotStarted,
    Playing,
    Completed,
    GameOver,
}
