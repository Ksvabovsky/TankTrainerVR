using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{

    public float GetHealth();

    public void SetHealth(float health);

    public void GetDamage(float damage);
}
