using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemyHealth))]
[RequireComponent (typeof(EnemyTurretController))]
public class EnemyController : MonoBehaviour
{
    [Header("Turret")]
    [SerializeField] protected TurretState turretState;

    protected ProtoTankController player;
    protected EnemyTurretController turretController;

    [SerializeField] protected EnemyManager enemyManager;
    private int ID;
    public GameObject square;
    public GameObject direction;

    [SerializeField] protected List<MeshRenderer> elementsMaterials = new List<MeshRenderer>();
    [SerializeField] protected List<MeshRenderer> outlinesMaterials = new List<MeshRenderer>();


    [SerializeField] protected bool playerInRange;

    private void OnEnable()
    {
        if (enemyManager)
        {
            enemyManager.AddEnemy(this);
        }
        Activate();
    }

    private void OnDisable()
    {
        Deactivate();
        turretController.enabled = false;
        enemyManager.RemoveEnemy(ID);
    }

    protected void Activate()
    {
        foreach (var element in elementsMaterials)
        {
            element.material.EnableKeyword("_EMISSION");
            element.material.SetColor("_BaseColor", Color.red);
            element.material.SetColor("_EmissionColor", new Color(1, 0, 0, 1) * 3.2f);
        }
        foreach (var outline in outlinesMaterials)
        {
            outline.material.EnableKeyword("_EMISSION");
            outline.material.SetColor("_Color", Color.red);
            outline.material.SetColor("_EmissionColor", new Color(1, 0, 0, 1));
        }
        //DynamicGI.UpdateEnvironment();
    }

    protected void Deactivate()
    {
        foreach (var element in elementsMaterials)
        {
            element.material.SetColor("_BaseColor", Color.white);
            element.material.SetColor("_EmissionColor", Color.white);
        }
        foreach (var outline in outlinesMaterials)
        {
            outline.material.SetColor("_Color", Color.white);
            outline.material.SetColor("_EmissionColor", new Color(1, 1, 1, 1));
        }
        //DynamicGI.UpdateEnvironment();
    }

    public bool IsPlayerLocked()
    {
        return playerInRange;
    }

    public void SetID(int id)
    {
        ID = id;
    }

    public void Dead()
    {
        this.enabled = false;
    }

    public void MissonOver()
    {
        Debug.Log("Roger, Stop");
        this.enabled = false;
    }
}

public enum EnemyState
{
    active,
    Moving,
    Attacking,
    disabled,
}

public enum PlayerDetectionState
{
    NotDetected,
    Detected,
    Lost
}



