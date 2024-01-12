using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    [SerializeField] GaugeScript rpm1G;
    [SerializeField] GaugeScript temp1G;
    [SerializeField] GaugeScript oil1G;

    [SerializeField] GaugeScript rpm2G;
    [SerializeField] GaugeScript temp2G;
    [SerializeField] GaugeScript oil2G;

    [SerializeField] GaugeScript pressG;
    [SerializeField] GaugeScript fuelG;

    [SerializeField] float rpm1V;
    [SerializeField] float rpm2V;

    [SerializeField] float temp1V;
    [SerializeField] float temp2V;

    [SerializeField] float oil1V;
    [SerializeField] float oil2V;

    [SerializeField] float pressV;
    [SerializeField] float fuelV;

    [SerializeField] float rpm1T;
    [SerializeField] float rpm2T;

    [SerializeField] float temp1T;
    [SerializeField] float temp2T;

    [SerializeField] float oil1T;
    [SerializeField] float oil2T;

    [SerializeField] float pressT;
    [SerializeField] float fuelT;

    // Start is called before the first frame update
    void OnEnable()
    {
        SetNumbers(vrpm1: 0.6f,vtemp1: 0.8f,voil1: 0.66f,vrpm2: 0.6f,vtemp2: 0.8f,voil2: 0.66f,vpress: 0.5f,vfuel: 0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGauge(rpm1G, ref rpm1V, rpm1T);
        UpdateGauge(temp1G, ref temp1V, temp1T);
        UpdateGauge(oil1G, ref oil1V, oil1T);

        UpdateGauge(rpm2G, ref rpm2V, rpm2T);
        UpdateGauge(temp2G, ref temp2V, temp2T);
        UpdateGauge(oil2G, ref oil2V, oil2T);

        UpdateGauge(fuelG,ref fuelV ,fuelT);
        UpdateGauge(pressG, ref pressV, pressT);
        
    }

    void UpdateGauge(GaugeScript gauge, ref float value, float target)
    {
        if (value != target)
        {
            value = Mathf.Lerp(value, target, Time.deltaTime);
            if (target - value < 0.005)
            {
                value = target;
                
            }
            gauge.updateValue(value);
            
        }

    }

    public void SetNumbers(float vrpm1 = -1f,float vtemp1 = -1f, float voil1 = -1f, float vrpm2 = -1f, float vtemp2 = -1f, float voil2 = -1f, float vpress = -1f, float vfuel = -1f)
    {
        rpm1T = vrpm1 != -1 ? vrpm1 : rpm1T;
        temp1T = vtemp1 != -1 ? vtemp1 : temp1T;
        oil1T = voil1 != -1 ? voil1 : oil1T;

        rpm2T = vrpm2 != -1 ? vrpm2 : rpm2T;
        temp2T = vtemp2 != -1 ? vtemp2 : temp2T;
        oil2T = voil2 != -1 ? voil2 : oil2T;

        pressT = vpress != -1 ? vpress : pressT;
        fuelT = vfuel != -1 ? vfuel : fuelT;
    }
}
