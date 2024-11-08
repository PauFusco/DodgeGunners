using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateObj, LogObj;

    private Button CreateButton;
    private TextMeshProUGUI Log;

    private void Start()
    {
        CreateButton = CreateObj.GetComponent<Button>();
        Log = LogObj.GetComponent<TextMeshProUGUI>();

        CreateButton.onClick.AddListener(LobbyCreate);
    }

    private void LobbyCreate()
    {
        CreatePrintLog("Lobby Created");
    }

    public void CreatePrintLog(string text)
    {
        Log.text = text;
    }
}