using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloSightScript : MonoBehaviour
{

    [SerializeField]
    private Transform headsetTransform;
    [SerializeField]
    private Transform viewTransform;
    [SerializeField]
    private Transform sightTransform;
    [SerializeField]
    private Transform uiTransform;

    [SerializeField]
    private Vector3 focusPoint;
    [SerializeField]
    private Vector3 uiStartPoint;
    [SerializeField]
    private Vector3 viewStartPoint;

    private Vector3 uiStartScale;

    [SerializeField] float viewA;
    [SerializeField] float viewB;
    [SerializeField] float viewC;
    
    [SerializeField] float uiA;
    [SerializeField] float uiB;
    [SerializeField] float uiC;

    [SerializeField] float viewZ;
    [SerializeField] float uiS;

    [SerializeField] Vector3 head;
    [SerializeField] Vector3 view;


    // Start is called before the first frame update
    void Start()
    {
        uiStartScale = uiTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        head = headsetTransform.position;
        view = viewTransform.position;


        viewTransform.position = headsetTransform.position;

        viewA = Vector3.Distance(viewTransform.localPosition, focusPoint);
        viewB = viewTransform.localPosition.x;
        viewC = viewTransform.localPosition.y - viewStartPoint.y;

        uiA = Vector3.Distance(uiTransform.localPosition, focusPoint);

        float proportion = viewA / uiA;

        uiB = viewB / proportion;
        uiC = viewC / proportion;

        viewZ= viewTransform.localPosition.z;

        float scaleProportion;

        uiS = viewStartPoint.z;

        scaleProportion = viewZ / viewStartPoint.z;

        uiTransform.localScale = new Vector3(scaleProportion, scaleProportion, scaleProportion) * 0.001f;

        uiTransform.localPosition = uiStartPoint + new Vector3(uiB, uiC, 0f);

    }

}
