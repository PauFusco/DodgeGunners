using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyIP : MonoBehaviour
{
    [SerializeField]
    private GameObject hostIPObj, logObj;

    private TextMeshProUGUI hostIP;
    private TextMeshProUGUI log;
    private string debugText;

    private void Start()
    {
        hostIP = hostIPObj.GetComponent<TextMeshProUGUI>();
        log = logObj.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        log.text = debugText;
    }

    public void CopyText()
    {
        if (hostIP != null)
        {
            GUIUtility.systemCopyBuffer = hostIP.text;
            debugText = "Copied to Clipboard!";
        }
        else
        {
            Debug.LogWarning("Button Text component is not assigned!");
        }
    }
}
