using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed , RunSpeed;
    private float ActiveMoveSpeed;
    
    private Vector3 MoveDir;
    private Vector3 MoveMent;
    
    public Transform ViewPoint;
    public float MouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool WithLook;

    public CharacterController CC;

    private Camera cam;

    public float JumpForce , GM;

    public Transform GroundChick;
    public bool isGround;
    public LayerMask GroundLayers;

    public GameObject bulletImpact;

    //public float TimeShoot;
    private float ShootCounter;

    public float muzzDisplayTime;
    private float muzzCounter;

    public float maxHeat, /*HeatPerShot,*/ coolRate, OverHeatCoolRate;
    private float HeatCounter;
    private bool OverHeated;

    public Gun[] AllGuns;
    private int selectedGun;
    
    void SwitchGun()
    {
        foreach (Gun gun in AllGuns)
        {
            gun.gameObject.SetActive(false);
        }
        AllGuns[selectedGun].gameObject.SetActive(true);
        AllGuns[selectedGun].Muzz.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        UIController.instance.WeaponTempSlider.maxValue = maxHeat;
        SwitchGun();
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * MouseSensitivity;
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if (WithLook)
        {
            ViewPoint.rotation = Quaternion.Euler(verticalRotStore,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z);

        }
        else
        {
            ViewPoint.rotation = Quaternion.Euler(-verticalRotStore,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z);

        }

        MoveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            ActiveMoveSpeed = RunSpeed;
        }
        else
        {
            ActiveMoveSpeed = MoveSpeed;
        }

        float yVel = MoveMent.y;
        
        MoveMent = ((transform.forward * MoveDir.z) + (transform.right * MoveDir.x)).normalized * ActiveMoveSpeed;
        
        MoveMent.y = yVel;
        
        if (CC.isGrounded)
        {
            MoveMent.y = 0f;
        }

        isGround = Physics.Raycast(GroundChick.position, Vector3.down, .25f, GroundLayers);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            MoveMent.y = JumpForce;
        }

        MoveMent.y += Physics.gravity.y * Time.deltaTime * GM;
        
        //transform.position += MoveMent * MoveSpeed * Time.deltaTime;

        CC.Move(MoveMent *ActiveMoveSpeed* Time.deltaTime);

        if (AllGuns[selectedGun].Muzz.activeInHierarchy)
        {
            muzzCounter -= Time.deltaTime;
            if (muzzCounter <= 0)
            {
                AllGuns[selectedGun].Muzz.SetActive(false);
            }
        }

        if (!OverHeated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (Input.GetMouseButton(0) && AllGuns[selectedGun].isAutomatic)
            {
                ShootCounter -= Time.deltaTime;

                if (ShootCounter <= 0)
                {
                    Shoot();
                }
            }

            HeatCounter -= coolRate * Time.deltaTime;

        }
        else
        {
            HeatCounter -= OverHeatCoolRate * Time.deltaTime;
            if (HeatCounter <= 0)
            {
                OverHeated = false;
                
                UIController.instance.overheatedMessage.gameObject.SetActive(false);

            }
        }

        if (HeatCounter < 0)
        {
            HeatCounter = 0f;
        }

        UIController.instance.WeaponTempSlider.value = HeatCounter;

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if (selectedGun >= AllGuns.Length)
            {
                selectedGun = 0;
            }

            SwitchGun();
            
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)

        {
            selectedGun--;
            if (selectedGun < 0)
            {
                selectedGun = AllGuns.Length - 1;
            }
            SwitchGun();
        }

    }

    private void LateUpdate()
    {
        cam.transform.position = ViewPoint.position;
        cam.transform.rotation = ViewPoint.rotation;
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            Debug.Log("We hit" + hit.collider.gameObject.name);

            GameObject bulletImpactObj = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal,Vector3.up));

            Destroy(bulletImpactObj,1f);
        }

        ShootCounter = AllGuns[selectedGun].timeBetweenShots;

        HeatCounter += AllGuns[selectedGun].heatPerShot;
        if (HeatCounter >= maxHeat)
        {
            HeatCounter = maxHeat;
            OverHeated = true;
            
            UIController.instance.overheatedMessage.gameObject.SetActive(true);
            
        }
        
        AllGuns[selectedGun].Muzz.SetActive(true);
        muzzCounter = muzzDisplayTime;
    }
    
    
    
}
