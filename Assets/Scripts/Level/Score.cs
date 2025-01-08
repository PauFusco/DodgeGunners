using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI score;

    private int _score = 0;
    
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

    public int GetScore() 
    { return _score; }

    public void SetScore(int score) 
    { _score = score; }
}
