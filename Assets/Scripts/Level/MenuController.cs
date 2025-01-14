using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject quitButtonObj, winnerBannerObj, countdownObj, prizeObj;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private TextMeshProUGUI winnerBanner;

    public void EnableMenu(string winnerName)
    {
        quitButton.onClick.AddListener(() => Application.Quit());

        quitButtonObj.SetActive(true);
        winnerBannerObj.SetActive(true);
        countdownObj.SetActive(false);
        prizeObj.SetActive(true);

        winnerBanner.text = winnerName + " WINS!!";
    }
}