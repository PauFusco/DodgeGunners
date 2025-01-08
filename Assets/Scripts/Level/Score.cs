using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI score;

    private uint _score = 0;
    
    private void Start()
    {
        score = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        score.text = _score.ToString();
    }

    public void Increase() 
    { _score++; }

    public uint GetScore() 
    { return _score; }

    public void SetScore(uint score) 
    { _score = score; }
}
