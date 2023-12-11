using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHealthScript : HealthScript, IHealth
{
    [SerializeField] float hp = 100;
    [SerializeField] float startHP = 100;

    [SerializeField] float shield = 80;
    [SerializeField] float startShield = 80;

    [SerializeField] Material hpBarMaterial;
    [SerializeField] Material shieldBarMaterial;

    Coroutine shieldRecharge = null;

    private void Start()
    {
        shield = startShield;
        hp = startHP;
        hpBarMaterial.SetFloat("_FillRate", 0.18f * (hp / startHP));
        shieldBarMaterial.SetFloat("_FillRate", 0.12f * (shield / startShield));
        shieldBarMaterial.SetFloat("_Alpha", 1f);
    }

    public override float GetHealth()
    {
        return hp;
    }

    public override void SetHealth(float value)
    {
        hp = value;
    }

    public override void GetDamage(float damage)
    {
        if(shieldRecharge != null)
        {
            StopCoroutine(shieldRecharge);
            shieldRecharge = null;
            
        }

        if (shield > 0)
        {
            shield -= damage;
            if (shield <= 0)
            {
                shield = 0;
            }

            shieldBarMaterial.SetFloat("_FillRate", 0.12f * (shield / startShield));
            

        }
        else
        {
            hp = hp - damage;
            Debug.Log("Took damage: " + damage);
            hpBarMaterial.SetFloat("_FillRate", 0.18f * (hp / startHP));
        }

        if (shield <= 0)
        {
            
            shieldRecharge = StartCoroutine(ShieldFullRecharge());
        }
        else
        {
            shieldRecharge = StartCoroutine(ShieldRecharge());
        }


    }

    IEnumerator ShieldRecharge()
    {
        yield return new WaitForSeconds(10f);

        while (shield < startShield)
        {
            shield = shield + 2f;
            shieldBarMaterial.SetFloat("_FillRate", 0.12f * (shield / startShield));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShieldFullRecharge()
    {
        shieldBarMaterial.SetFloat("_Alpha", 0f);
        shieldBarMaterial.SetFloat("_FillRate", 0.12f);
        
        for (int i = 0;i<6f;i++)
        {
            

            for(float j = 0;j<1;j += Time.deltaTime *4)
            {
                shieldBarMaterial.SetFloat("_Alpha", j);
                yield return null;
            }
            shieldBarMaterial.SetFloat("_Alpha", 1f);
            yield return new WaitForSeconds(0.05f);

            for (float j = 1; j >0; j -= Time.deltaTime *4)
            {
                shieldBarMaterial.SetFloat("_Alpha", j);
                yield return null;
            }
            shieldBarMaterial.SetFloat("_Alpha", 0);
            yield return new WaitForSeconds(1);

        }
        shieldBarMaterial.SetFloat("_Alpha", 1f);

        while (shield < startShield)
        {
            shield = shield + 2f;
            shieldBarMaterial.SetFloat("_FillRate", 0.12f * (shield / startShield));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
