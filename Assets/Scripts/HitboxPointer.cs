using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPointer : MonoBehaviour
{
    [SerializeReference] private GameObject mainParent;
    [SerializeReference] private HealthScript healthScript;

    [SerializeField] private bool hardArmor;

    public IHealth GetHealthComp()
    {
        return healthScript;
    }

    public GameObject GetMainGameObject()
    {
        return mainParent;
    }

    public bool IsHardArmor() { 
        return hardArmor;
    }

}
