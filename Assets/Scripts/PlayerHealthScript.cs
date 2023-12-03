using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : HealthScript, IHealth
{
    [SerializeField] float hp = 100;
    [SerializeField] float startHP = 100;

    [SerializeField] Material hpBarMaterial;

    private void Start()
    {
        hp = startHP;
        hpBarMaterial.SetFloat("_FillRate", 0.18f * (hp / startHP));
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
        hpBarMaterial.SetFloat("_FillRate", 0.18f * (hp / startHP));
    }
}
