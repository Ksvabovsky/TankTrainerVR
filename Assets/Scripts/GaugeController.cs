using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    [SerializeField] GaugeScript eng1rpm;
    [SerializeField] GaugeScript eng1temp;
    [SerializeField] GaugeScript eng1oil;

    [SerializeField] GaugeScript eng2rpm;
    [SerializeField] GaugeScript eng2temp;
    [SerializeField] GaugeScript eng2oil;

    [SerializeField] GaugeScript pressureG;
    [SerializeField] GaugeScript fuelG;

    [SerializeField] float rpm1;
    [SerializeField] float rpm2;

    [SerializeField] float temp1;
    [SerializeField] float temp2;

    [SerializeField] float oil1;
    [SerializeField] float oil2;

    [SerializeField] float press;
    [SerializeField] float fuel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Startup(0.95f));
    }

    // Update is called once per frame
    void Update()
    {
        eng1rpm?.updateValue(rpm1);
        eng1temp?.updateValue(temp1);
        eng1oil?.updateValue(oil1);

        eng2rpm?.updateValue(temp2);
        eng2temp?.updateValue(temp2);
        eng2oil?.updateValue(oil2);

        pressureG?.updateValue(press);
        fuelG?.updateValue(fuel);
    }

    IEnumerator Startup(float rmp1)//,float temp1, float oil1, float rpm2,float temp2, float oil2, float press, float fuel)
    {
        while(this.fuel <0.95f)
        {
            Debug.Log("chuj");
            if(0.95f - this.fuel < 0.01)
            {
                this.fuel = fuel; break;
            }
            this.fuel = Mathf.Lerp(this.fuel, fuel, Time.deltaTime);
            yield return null;
        }
        
    }
}
