using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ImpactPointerScript : MonoBehaviour
{
    [SerializeField] private LayerMask mask;


    [SerializeField] private GameObject point;
    [SerializeField] private Transform sight;
    [SerializeField] private float defaultDistance;

    [SerializeField] private Vector3 DEBUG;

    // Update is called once per frame
    void Update()
    {
        //lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 150f, mask))
        {

            point.transform.position = hit.point;

            float distance = Vector3.Distance(point.transform.position, sight.position);
            float scale = distance / defaultDistance;

            DEBUG = hit.normal;
            point.transform.rotation = Quaternion.FromToRotation (-Vector3.forward, hit.normal);;
        }

    }
}