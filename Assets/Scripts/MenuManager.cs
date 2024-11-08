using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateLobbyObj, JoinLobbyObj, QuitObj, BaseMenuParent,
        CreateLobbyMenuParent, JoinLobbyMenuParent, CreateLobbyBackObj, JoinLobbyBackObj;

    private Button CreateLobbyButton, JoinLobbyButton, QuitButton,
        CreateLobbyBackButton, JoinLobbyBackButton;

    private void Start()
    {
        CreateLobbyButton = CreateLobbyObj.GetComponent<Button>();
        JoinLobbyButton = JoinLobbyObj.GetComponent<Button>();
        QuitButton = QuitObj.GetComponent<Button>();
        CreateLobbyBackButton = CreateLobbyBackObj.GetComponent<Button>();
        JoinLobbyBackButton = JoinLobbyBackObj.GetComponent<Button>();

        CreateLobbyButton.onClick.AddListener(() => ChangeMenu(BaseMenuParent, CreateLobbyMenuParent));
        JoinLobbyButton.onClick.AddListener(() => ChangeMenu(BaseMenuParent, JoinLobbyMenuParent));
        CreateLobbyBackButton.onClick.AddListener(() => ChangeMenu(CreateLobbyMenuParent, BaseMenuParent));
        JoinLobbyBackButton.onClick.AddListener(() => ChangeMenu(JoinLobbyMenuParent, BaseMenuParent));

        QuitButton.onClick.AddListener(QuitApp);

        BaseMenuParent.SetActive(true);
        CreateLobbyMenuParent.SetActive(false);
        JoinLobbyMenuParent.SetActive(false);
    }

    private void ChangeMenu(GameObject from, GameObject to)
    {
        from.SetActive(false);
        to.SetActive(true);
    }

    private void QuitApp()
    {
        Debug.Log("Quit App");
        Application.Quit();
    }
}