using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPointer : MonoBehaviour
{
    [SerializeReference] private HealthScript healthScript;

    public IHealth GetHealthComp()
    {
        return healthScript;
    }
}
