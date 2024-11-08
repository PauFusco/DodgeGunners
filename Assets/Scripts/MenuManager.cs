using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateLobbyObj;

    [SerializeField]
    private GameObject JoinLobbyObj;

    private Button CreateLobbyButton;
    private Button JoinLobbyButton;

    private void Start()
    {
        CreateLobbyButton = CreateLobbyObj.GetComponent<Button>();
        JoinLobbyButton = JoinLobbyObj.GetComponent<Button>();

        CreateLobbyButton.onClick.AddListener(CreateLobbyEnable);
        JoinLobbyButton.onClick.AddListener(JoinLobbyEnable);
    }

    private void CreateLobbyEnable()
    {
        Debug.Log("Create Lobby");
    }

    private void JoinLobbyEnable()
    {
        Debug.Log("Join Lobby");
    }
}