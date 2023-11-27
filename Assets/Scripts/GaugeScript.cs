using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour
{
    private float maxAngle = 280f;
    [SerializeField] float value;

    [SerializeField] Transform pointer;
    

    public void updateValue(float value)
    {
        this.value = value;
        pointer.localEulerAngles = new Vector3 (0, maxAngle * this.value, 0);
    }
}
