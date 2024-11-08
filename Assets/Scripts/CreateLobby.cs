using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateObj;

    private Button CreateButton;

    private void Start()
    {
        CreateButton = CreateObj.GetComponent<Button>();

        CreateButton.onClick.AddListener(LobbyCreate);
    }

    private void LobbyCreate()
    {
        Debug.Log("CreateLobby");
    }
}