using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class RocketScript : MonoBehaviour, IBullet
{

    [SerializeField] LayerMask mask;
    [SerializeField] GameObject effect;
    [SerializeField] GameObject trail;
    [SerializeField] float damage = 1;
    [SerializeField] float speed = 200f;
    [SerializeField] float rotateSpeed = 5f;

    [SerializeField] BulletManager BM;
    [SerializeField] ProtoTankController player;
    Rigidbody rb;

    public void OnEnable()
    {
        BM = BulletManager.instance;
        player = ProtoTankController.instance;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, 5f, mask))
        {
            Debug.Log(hit.collider.gameObject.name);
            BM = BulletManager.instance;
            BM.hits.Add(hit.point);
            if (hit.collider.gameObject.TryGetComponent<HitboxPointer>(out HitboxPointer HB))
            {
                IHealth targetHealth = HB.GetHealthComp();
                targetHealth.GetDamage(damage);

            }
            Instantiate(effect, hit.point, this.transform.rotation);
            
            Collided();
        }
        rb.velocity = transform.forward * speed;
        Vector3 heading = player.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(heading);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation,rotateSpeed * Time.deltaTime));
    }

    public void BulletInit(float Bdamage, float Bspeed, LayerMask Bmask)
    {
        damage = Bdamage;
        speed = Bspeed;
        mask = Bmask;
    }

    void Collided()
    {
        trail.transform.SetParent(BM.transform, true);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collided();
    }
}
