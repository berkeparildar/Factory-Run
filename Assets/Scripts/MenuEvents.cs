using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuEvents : MonoBehaviour
{
    public GameObject box;
    public Animator menuAnimator;
    public Animator canvasAnimator;
    private static readonly int BuyScreen = Animator.StringToHash("BuyScreen");
    private static readonly int Back = Animator.StringToHash("GoBack");
    private static bool _isInMenu;
    public GameObject wheel;
    public static int ModelIndex;
    public static int[] Prices = new[] { 100, 200, 300, 400, 500, 600, 700, 800 };
    public static int Money;
    public TextMeshProUGUI moneyText;
    public Button buyButton;
    public TextMeshProUGUI buttonText;
    private Vector2 touchStartPos;
    private const float minSwipeDistance = 50f;

    void Start()
    {
        StartCoroutine(BoxSpawner());
        Money = PlayerPrefs.GetInt("Money", 0);
    }

    private void Update()
    {
        moneyText.text = Money.ToString();
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

                    if (swipeDelta.magnitude > minSwipeDistance)
                    {
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 0)
                            {
                                wheel.transform.DORotate(new Vector3(0, 45, 0), 1).SetRelative();
                                ModelIndex++;
                            }
                            else
                            {
                                wheel.transform.DORotate(new Vector3(0, -45, 0), 1).SetRelative();
                                ModelIndex--;
                            }
                        }
                    }

                    break;
            }
        }

        if (ModelIndex == 8)
        {
            ModelIndex = 0;
        }

        if (ModelIndex == -1)
        {
            ModelIndex = 7;
        }

        if (ModelIndex == PlayerPrefs.GetInt("Model"))
        {
            buyButton.interactable = false;
            buttonText.text = "OWNED";
        }
        else
        {
            buyButton.interactable = true;
            buttonText.text = "BUY";
        }

        if (Money < Prices[ModelIndex])
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
        PlayerPrefs.SetInt("Model", ModelIndex);
        Money -= Prices[ModelIndex];
        PlayerPrefs.SetInt("Money", Money);
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
        _isInMenu = true;
    }

    public void GoBack()
    {
        menuAnimator.SetTrigger(Back);
        canvasAnimator.SetTrigger(Back);
        _isInMenu = false;
    }
}