using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    public LayerMask mask;

    public GameObject point;
    private LineRenderer lr;

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 150f, mask))
        {

            point.transform.position = hit.point;
            //if (hit.collider)
            //{
                //lr.SetPosition(1, hit.point);
            //}
        }
        //else
        //{
        //    lr.SetPosition(1, transform.forward * 5000);
        //}
    }
}