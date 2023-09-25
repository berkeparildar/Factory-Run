using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static float PlatformCooldown;
    public static float BoxSpawnCooldown;
    public static int Coins;
    private static int _score;
    private static int _lastAddedPosition;
    private static int _playerTargetPosition;
    [SerializeField] private Player player;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject platformContainer;
    [SerializeField] private GameObject hitUI;
    [SerializeField] private GameObject endWall;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI collectedCoins;
    [SerializeField] private TextMeshProUGUI totalCoins;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI allTimeScore;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject ghostObject;
    [SerializeField] private bool notLoaded;
    
    // Start is called before the first frame update
    private void Start()
    {
        notLoaded = true;
        targetObject = ghostObject;
        PlatformCooldown = 0;
        BoxSpawnCooldown = 0;
        _playerTargetPosition = 3;
        _lastAddedPosition = 18;
       //InitializeFirstPlatforms();
    }

    // Update is called once per frame
    private void Update()
    {
        if (notLoaded)
        {
            MoveGhost();
        }
        CheckPlayerPosition();
        CheckCoolDowns();
        SetUIText();
    }

    public void OnLoaded()
    {
        player.gameObject.SetActive(true);
        targetObject = player.gameObject;
        _playerTargetPosition = 3;
        notLoaded = false;
    }

    private void MoveGhost()
    {
        ghostObject.transform.Translate(Vector3.forward * 20 * Time.deltaTime);
    }

    private void InitializeFirstPlatforms()
    {
        for (int i = 0; i < 50; i++)
        {
            var generatedPlatform = Instantiate(platform, new Vector3(0, 0, _lastAddedPosition), Quaternion.identity);
            generatedPlatform.transform.SetParent(platformContainer.transform);
            //Debug.Log(PlatformCooldown);
            _lastAddedPosition += 3;
        }
    }

    private static void CheckCoolDowns()
    {
        if (PlatformCooldown > 0)
        {
            PlatformCooldown -= Time.deltaTime;
        }

        if (BoxSpawnCooldown > 0)
        {
            BoxSpawnCooldown -= Time.deltaTime;
        }
    }

    private void CheckPlayerPosition()
    {
        if (targetObject.transform.position.z >= _playerTargetPosition)
        {
            GeneratePlatform();
            _playerTargetPosition += 3;
        }
    }

    private void GeneratePlatform()
    {
        var generatedPlatform = Instantiate(platform, new Vector3(0, 0, _lastAddedPosition), Quaternion.identity);
        generatedPlatform.transform.SetParent(platformContainer.transform);
        _lastAddedPosition += 3;
        endWall.transform.position = (new Vector3(0, 2, generatedPlatform.transform.position.z + 1));
        Destroy(platformContainer.transform.GetChild(0).gameObject);
    }

    public static IEnumerator ScoreRoutine()
    {
        while (Player.IsAlive)
        {
            _score += 1;
            yield return new WaitForSeconds(1);
        }
        yield break;
    }

    public void ShowHitUI()
    {
        hitUI.SetActive(true);
        var highScore = PlayerPrefs.GetInt("HighScore", 0);
        var currentCoins = PlayerPrefs.GetInt("Money", 0);
        currentCoins += Coins;
        if (_score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
            recordText.gameObject.SetActive(true);
        }
        collectedCoins.text = Coins.ToString();
        totalCoins.text = currentCoins.ToString();
        currentScore.text = _score.ToString();
        allTimeScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        PlayerPrefs.SetInt("Money", currentCoins);
        PlayerPrefs.Save();
    }

    private void SetUIText()
    {
        coinText.text = Coins.ToString();
        scoreText.text = _score.ToString();
    }

    public void RestartLevel()
    {
        DOTween.KillAll();
        Debug.Log("pressed");
        _score = 0;
        Coins = 0;
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }

   
}