using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject effect;
    [SerializeField] float damage = 1; 

    [SerializeField] BulletManager BM;

    public void OnEnable()
    {
        BM = BulletManager.instance;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hit, 5f, mask))
        {
            Debug.Log(hit.collider.gameObject.name);
            BM = BulletManager.instance;
            BM.hits.Add(hit.point);
            if(hit.collider.gameObject.TryGetComponent<HitboxPointer>(out HitboxPointer HB)){
                IHealth targetHealth = HB.GetHealthComp();
                targetHealth.GetDamage(damage);
                
            }
           
            Collided();
        }
    }

    void Collided()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward *5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collided();
    }
}
