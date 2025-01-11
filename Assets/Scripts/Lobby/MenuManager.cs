using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject JoinParent, BaseParent, CreateParent;

    [SerializeField]
    private Button CreateLobbyButton, JoinLobbyButton, QuitButton,
        CreateLobbyBackButton, JoinLobbyBackButton;

    private void Start()
    {
        CreateLobbyButton.onClick.AddListener(() => ChangeMenu(BaseParent, CreateParent));
        JoinLobbyButton.onClick.AddListener(() => ChangeMenu(BaseParent, JoinParent));
        CreateLobbyBackButton.onClick.AddListener(() => ChangeMenu(CreateParent, BaseParent));
        JoinLobbyBackButton.onClick.AddListener(() => ChangeMenu(JoinParent, BaseParent));

        QuitButton.onClick.AddListener(QuitApp);

        BaseParent.SetActive(true);
        CreateParent.SetActive(false);
        JoinParent.SetActive(false);
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