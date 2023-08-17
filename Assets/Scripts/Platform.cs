using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject box;
    private int _chanceToContinueHigh;
    private static bool _highPlatformSpawning;
    private static int _currentAxisOfHigh;
    private static int _numberOfHighPlatforms;
    private static int _oldIndex;
    private static bool _initAfterContinuation;
    
    // Start is called before the first frame update
    void Start()
    {
        CheckIfHigh();
        InitializeHighPlatforms();
        CheckObstacleSpawning();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private static void CheckIfHigh()
    {
        if (!_highPlatformSpawning && GameManager.cooldown <= 0)
        {
            var chanceToBeHigh = Random.Range(0, 5);
            if (chanceToBeHigh != 4) return;
            _highPlatformSpawning = true;
            _numberOfHighPlatforms = Random.Range(6, 10);
            // 0 Left, 1 Middle, 2 Right
            _currentAxisOfHigh = Random.Range(0, 3);
        }
    }

    private void CheckObstacleSpawning()
    {
        var chanceOfObstacles = Random.Range(0, 5);
        if (chanceOfObstacles == 4)
        {
            if (_highPlatformSpawning)
            {
                var spots = new List<int>() { -1, 0, 1 };
                spots.Remove(_currentAxisOfHigh);
                var coinToss = Random.Range(0, 2);
                var boxXPos = spots[coinToss];
                Instantiate(box, new Vector3(boxXPos, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
            }
            else
            {
                var xPos = Random.Range(-1, 2);
                Instantiate(box, new Vector3(xPos, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
            }
        }
    }

    private void InitializeHighPlatforms()
    {
        if (_highPlatformSpawning)
        {
            var indexOfHighPlatform = _currentAxisOfHigh + 3;
            transform.GetChild(_currentAxisOfHigh).gameObject.SetActive(false);  
            transform.GetChild(indexOfHighPlatform).gameObject.SetActive(true);
            if (_initAfterContinuation)
            {
                var index = _oldIndex + 3;
                transform.GetChild(_oldIndex).gameObject.SetActive(false);  
                transform.GetChild(index).gameObject.SetActive(true);
                _initAfterContinuation = false;
                Debug.Log("Should Initialize two");
            }
            _numberOfHighPlatforms--;
            if (_numberOfHighPlatforms == 0)
            {
                var chanceToContinue = Random.Range(0, 3);
                if (chanceToContinue == 0)
                {
                    _oldIndex = _currentAxisOfHigh;
                    _initAfterContinuation = true;
                    Debug.Log("continueing...");
                    switch (_currentAxisOfHigh)
                    {
                        case 0:
                            _currentAxisOfHigh = 1;
                            break;
                        case 1:
                            var leftRight = Random.Range(0, 2);
                            _currentAxisOfHigh = leftRight == 0 ? 0 : 2;
                            break;
                        case 2:
                            _currentAxisOfHigh = 1;
                            break;
                    }
                    _numberOfHighPlatforms = Random.Range(6, 10);
                }
                else
                {
                    _highPlatformSpawning = false;
                    GameManager.cooldown = Random.Range(6, 10);
                }
            }
        }
    }
}