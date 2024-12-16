using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Countdown : MonoBehaviour
{
    private TMP_Text m_TimerText;
    public float startTime;
    private float m_Time;

    private void Awake()
    {
        m_TimerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        m_Time -= Time.deltaTime;

        m_TimerText.text = TimeSpan.FromSeconds(m_Time).ToString(format: @"mm\:ss");
    }

    public void ResetCountdown()
    {  m_Time = startTime; }
}
