using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuEvents : MonoBehaviour
{
    [SerializeField] private GameObject box;
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private bool isInMenu;
    [SerializeField] private GameObject wheel;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Vector2 touchStartPos;
    [SerializeField] private GameObject backgroundMusic;
    private static int _modelIndex;
    private static readonly int[] Prices = new[] { 100, 200, 300, 400, 500, 600, 700, 800 };
    private static readonly int BuyScreen = Animator.StringToHash("BuyScreen");
    private static readonly int Back = Animator.StringToHash("GoBack");
    private static int _money;
    private const float MinSwipeDistance = 50f;

    private void Start()
    {
        StartCoroutine(BoxSpawner());
        _money = PlayerPrefs.GetInt("Money", 0);
    }

    private void Update()
    {
        moneyText.text = _money.ToString();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    Vector2 swipeDelta = touch.position - touchStartPos;

                    if (swipeDelta.magnitude > MinSwipeDistance)
                    {
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 0)
                            {
                                wheel.transform.DORotate(new Vector3(0, 45, 0), 1).SetRelative();
                                _modelIndex++;
                            }
                            else
                            {
                                wheel.transform.DORotate(new Vector3(0, -45, 0), 1).SetRelative();
                                _modelIndex--;
                            }
                        }
                    }

                    break;
            }
        }

        if (_modelIndex == 8)
        {
            _modelIndex = 0;
        }

        if (_modelIndex == -1)
        {
            _modelIndex = 7;
        }

        if (_modelIndex == PlayerPrefs.GetInt("Model"))
        {
            buyButton.interactable = false;
            buttonText.text = "OWNED";
        }
        else
        {
            buyButton.interactable = true;
            buttonText.text = "BUY";
        }

        if (_money < Prices[_modelIndex])
        {
            buyButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
        }
    }


    public void Buy()
    {
        PlayerPrefs.SetInt("Model", _modelIndex);
        _money -= Prices[_modelIndex];
        PlayerPrefs.SetInt("Money", _money);
        buttonText.text = "OWNED";
        buyButton.interactable = false;
    }

    private IEnumerator BoxSpawner()
    {
        while (true)
        {
            var chance = Random.Range(0, 2); // 0 or 1
            var xPos = chance == 0 ? -1 : 1;
            Instantiate(box, new Vector3(xPos, 5, -1), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }

    public void StartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void GoToBuy()
    {
        menuAnimator.SetTrigger(BuyScreen);
        canvasAnimator.SetTrigger(BuyScreen);
        isInMenu = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoBack()
    {
        menuAnimator.SetTrigger(Back);
        canvasAnimator.SetTrigger(Back);
        isInMenu = false;
    }
}