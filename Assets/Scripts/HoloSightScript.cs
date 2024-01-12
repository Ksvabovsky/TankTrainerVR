using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoloSightScript : MonoBehaviour
{
    
     private TankController player;
     private Transform playerTransform;
     private PlayerHealthScript playerHP;

    [Header("UI Elements")]

    [SerializeField] private GameObject HUD;
    [SerializeField] private TMP_Text hullText;
    [SerializeField] private TMP_Text shieldText;
    [SerializeField] private TMP_Text speedText;

    [Space(10)]

    [SerializeField] private GameObject Menu;

    [Space(15)]

    [SerializeField] private Transform headsetTransform;
    [SerializeField] private Transform viewTransform;
    [SerializeField] private Transform sightTransform;
    [SerializeField] private Transform uiTransform;

    [SerializeField] bool moveUI;

    [SerializeField] private Vector3 focusPoint;
    [SerializeField] private Vector3 uiStartPoint;
    [SerializeField] private Vector3 viewStartPoint;

    private Vector3 uiStartScale;

    float viewA;
    float viewB;
    float viewC;
    
    float uiA;
    float uiB;
    float uiC;

    float viewZ;
    float uiS;

    Vector3 head;
    Vector3 view;

    [Header("Compass")]

    [SerializeField] Material compassMat;
    [SerializeField] float playerDirection;
    [SerializeField] float compassOffsetValue;


    private void Awake()
    {
        player = TankController.instance;
        playerTransform = player.transform;
        playerHP = player.GetComponent<PlayerHealthScript>();
    }

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

        if(moveUI) {

        uiTransform.localScale = new Vector3(scaleProportion, scaleProportion, scaleProportion) * 0.001f;

        uiTransform.localPosition = uiStartPoint + new Vector3(uiB, uiC, 0f);
        }


        if (player.IsActive())
        {
            UpdateHUD();
        }
        
    }


    void UpdateHUD()
    {
        playerDirection = playerTransform.rotation.eulerAngles.y;

        if (playerDirection > 180f)
        {
            playerDirection -= 360f;
        }

        hullText.text = "HULL\n" + playerHP.GetHealthPercent() * 1000f / 10f + "%";
        shieldText.text = "SHIELD\n" + playerHP.GetShieldPercent() * 1000f / 10f + "%";
        speedText.text = "SPEED\n" + Mathf.Round(player.GetSpeed() * 3600f / 1000f) + "KPH";


        compassOffsetValue = playerDirection / 360f;


        compassMat.SetTextureOffset("_BaseMap", new Vector2(compassOffsetValue, 0));
        compassMat.SetTextureOffset("_EmissionMap", new Vector2(compassOffsetValue, 0));
    }

    public void StartMission()
    {
        Debug.Log("dupa");
        player.StartTank();
        ChangeMenuToHud();
    }

    void ChangeMenuToHud()
    {
        Menu.SetActive(false);
        HUD.SetActive(true);
    }

    void ChangeHUDToMenu()
    {
        HUD.SetActive(false);
        Menu.SetActive(true);
    }

}
