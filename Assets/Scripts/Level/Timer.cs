using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    private TMP_Text m_TimerText;
    private float m_Time;

    void Awake()
    {
        m_TimerText = GetComponent<TMP_Text>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Time += Time.deltaTime;

        m_TimerText.text = TimeSpan.FromSeconds(m_Time).ToString(format: @"mm\:ss\:ff");
    }
}
