using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public static EndGame Instance;

    public GameObject winImage;
    public GameObject loseImage;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // รับผลจาก GameManager หรือผ่าน static bool
        bool isWin = GameResult.isWin; // GameResult เป็น static class เก็บผล
        ShowResult(isWin);
    }

    public void ShowResult(bool isWin)
    {
        winImage.SetActive(isWin);
        loseImage.SetActive(!isWin);
    }

    public static class GameResult
    {
        public static bool isWin;
    }

    

    public void PlayAgain()
    {
        SceneManager.LoadScene("262_FinalProject");
    }

    public void GoHomeMenu()
    {
        SceneManager.LoadScene("HomeMenu");
    }
}




