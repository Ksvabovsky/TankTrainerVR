using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ProtoDriveController : MonoBehaviour
{
    InputReader input;
    Rigidbody rb;

    [Header("HoverPhysics")]

    [SerializeField] private Transform[] anchors = new Transform[4];
    [SerializeField] RaycastHit[] hits = new RaycastHit[4];

    public AnimationCurve hoverPower;
    [Space(10)]
    [SerializeField] private float currentPower;
    [SerializeField] private float multiplier;
    [SerializeField] private float moveForce, turnTorque;



    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputReader>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        rb.AddForce(input.GetDrive() * moveForce * transform.forward);
        rb.AddForce(input.GetStrafe() * moveForce * transform.right);
        rb.AddTorque(input.GetTurn() * turnTorque * transform.up);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            //currentPower = Mathf.Sqrt(multiplier * (hoverPower.Evaluate(force) + 1));
            rb.AddForceAtPosition(transform.up * force, anchor.position, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            Gizmos.DrawSphere(anchors[i].position, 0.2f);

            Physics.Raycast(anchors[i].position, -anchors[i].up, out hit);
            Gizmos.DrawSphere(hit.point, 0.2f);
            Gizmos.DrawLine(anchors[i].position, hit.point);
        }
    }
}
