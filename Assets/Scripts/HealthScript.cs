using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthScript : MonoBehaviour, IHealth
{
    public abstract float GetHealth();

    public abstract void SetHealth(float value);

    public abstract void GetDamage(float damage);
}
