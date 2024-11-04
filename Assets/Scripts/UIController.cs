using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Slider WeaponTempSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public TMP_Text overheatedMessage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
