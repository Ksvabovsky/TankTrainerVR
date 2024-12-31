using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private GameObject menuFirstButton;

    [Space(10)]

    [SerializeField] private GameObject adjustMenu;
    [SerializeField] private float adjustValue;
    [SerializeField] private TMP_Text heightText;
    [SerializeField] private float heightValue;
    [SerializeField] private GameObject adjustFirstButton;

    [Space(10)]

    [SerializeField] private Animation StartUpAnim;

    [Space(10)]

    [SerializeField] private GameObject Completed;
    [SerializeField] private GameObject CompletedFirstButton;

    [Space(10)]

    [SerializeField] private GameObject Over;
    [SerializeField] private GameObject OverFirstButton;

    [Space(15)]

    [SerializeField] private Transform XRrig;
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
    [SerializeField] float compassUVOffsetValue;


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
        float shield = playerHP.GetShieldPercent();
        if (shield > 0f)
        {
            shieldText.text = "SHIELD\n" + shield *1000f / 10f + "%";
        }
        else
        {
            shieldText.text = "SHIELD\n" + "RECHARGING";
        }
        
        speedText.text = "SPEED\n" + Mathf.Round(player.GetSpeed() * 3600f / 1000f) + "KPH";


        compassOffsetValue = playerDirection / 360f + compassUVOffsetValue;


        compassMat.SetTextureOffset("_BaseMap", new Vector2(compassOffsetValue, 0));
        compassMat.SetTextureOffset("_EmissionMap", new Vector2(compassOffsetValue, 0));
    }

    public void MissionStart()
    {
        Debug.Log("start");
        player.StartTank();
        StartUpAnim.Play();
    }


    public void MissionCompleted()
    {

    }

    public void MissionOver()
    {
        ChangeHUDToOver();
    } 

    public void ChangeMenuToHud()
    {
        Menu.SetActive(false);
        HUD.SetActive(true);
    }

    public void ChangeHUDToMenu()
    {
        HUD.SetActive(false);
        Menu.SetActive(true);
    }

    public void ChangeMenuToAdjust()
    {
        Menu.SetActive(false);
        adjustMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(adjustFirstButton);
    }

    public void ChangeAdjustToMenu()
    {
        adjustMenu.SetActive(false);
        Menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
    }

    public void ChangeHUDToCompleted()
    {
        HUD.SetActive(false);
        Completed.SetActive(true);
        EventSystem.current.SetSelectedGameObject(CompletedFirstButton);
    }

    public void ChangeHUDToOver()
    {
        HUD.SetActive(false );
        Over.SetActive(true );
        EventSystem.current.SetSelectedGameObject(OverFirstButton);

    }

    public void MoveUpSeat()
    {
        XRrig.Translate(0f, adjustValue, 0f);
        heightValue += adjustValue;
        heightText.text = heightValue.ToString();
        
    }

    public void MoveDownSeat()
    {
        XRrig.Translate(0f, -adjustValue, 0f);
        heightValue -= adjustValue;
        heightText.text = heightValue.ToString();
    }
}
