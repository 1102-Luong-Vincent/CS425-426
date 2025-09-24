using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeGameManage : MonoBehaviour
{
    [Header("UI")]
    public Button gameButton;

    [Header("Game Objects")]
    public Image point;     
    public Image winRange;     

    [Header("Settings")]
    public float rotateSpeed = 100f; 
    public float maxWinAngle = 30f;  

    private bool isStarGame = false;
    private float pointAngle = 0f;

    public TextMeshProUGUI resultText;


    void Start()
    {
        gameButton.onClick.AddListener(OnGameButtonClick);
        resultText.gameObject.SetActive(false);

    }

    void Update()
    {
        if (isStarGame)
        {
            RotatePoint();
        }
    }

    void OnGameButtonClick()
    {
        if (isStarGame)
        {
            ToEndGame();
        }
        else
        {
            ToStartGame();
        }
    }

    void ToStartGame()
    {
        gameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "STOP";
        isStarGame = true;
        pointAngle = 0f;
        point.rectTransform.rotation = Quaternion.Euler(0, 0, pointAngle);
        float randomStartAngle = Random.Range(0f, 360f);
        winRange.rectTransform.rotation = Quaternion.Euler(0, 0, -randomStartAngle);
        float randomRange = Random.Range(5f, maxWinAngle);
        winRange.fillAmount = randomRange / 360f;
        resultText.gameObject.SetActive(false);
    }

    void ToEndGame()
    {
        gameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "START NEW GAME";

        isStarGame = false;

        float currentAngle = NormalizeAngle(point.rectTransform.eulerAngles.z);

        float winStart = NormalizeAngle(winRange.rectTransform.eulerAngles.z);
        float winEnd = NormalizeAngle(winStart + winRange.fillAmount * 360f);

        bool isWin = AngleInRange(currentAngle, winStart, winEnd);
        resultText.gameObject.SetActive(true);
        resultText.text = isWin ? "YOU WIN!" : "YOU LOSE!";
    }


    void RotatePoint()
    {
        pointAngle += rotateSpeed * Time.deltaTime;
        pointAngle = NormalizeAngle(pointAngle);
        point.rectTransform.rotation = Quaternion.Euler(0, 0, -pointAngle);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    bool AngleInRange(float angle, float start, float end)
    {
        if (start <= end)
            return angle >= start && angle <= end;
        else
            return angle >= start || angle <= end; 
    }
}
