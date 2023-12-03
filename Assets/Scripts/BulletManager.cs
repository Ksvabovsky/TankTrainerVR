using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public List<Vector3> hits;

    public static BulletManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach(var hit in hits)
        {
            Gizmos.DrawSphere(hit, 0.2f);
        }
    }
}
