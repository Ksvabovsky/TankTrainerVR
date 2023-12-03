using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HealthScript, IHealth
{
    [SerializeField] float hp = 1;
    [SerializeField] float startHP = 1;

    private void Start()
    {
        hp = startHP;
    }

    public override float GetHealth()
    {
        return hp;
    }

    public override void SetHealth(float value)
    {
        hp = value;
    }

    public override void GetDamage(float damage)
    {
        hp = hp - damage;
        Debug.Log("Took damage: " + damage);
    }
}
