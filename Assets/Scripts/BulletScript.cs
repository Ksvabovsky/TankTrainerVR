using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject effect;

    [SerializeField] BulletManager BM;


    private void FixedUpdate()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hit, 10f, mask))
        {
            Debug.Log(hit.collider.gameObject.name);
            BM = BulletManager.instance;
            BM.hits.Add(hit.transform.position);
            Collided();
        }
    }

    void Collided()
    {
        Destroy(this);
    }
}
