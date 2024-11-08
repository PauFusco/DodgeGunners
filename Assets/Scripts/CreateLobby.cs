using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateObj, LogObj, UsernameInputFieldObj;

    private Button CreateButton;
    private TextMeshProUGUI Log;
    private TMP_InputField UsernameInput;

    private void Start()
    {
        CreateButton = CreateObj.GetComponent<Button>();
        Log = LogObj.GetComponent<TextMeshProUGUI>();
        UsernameInput = UsernameInputFieldObj.GetComponent<TMP_InputField>();

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