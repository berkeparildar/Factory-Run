using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject box;
    public GameObject tunnel;
    public GameObject coin;
    private GameObject _coinContainer;
    private GameObject _obstacleContainer;
    private int _chanceToContinueHigh;
    private static bool _highPlatformSpawning;
    private static bool _coinSpawning;
    private static int _currentAxisOfHigh;
    private static int _numberOfHighPlatforms;
    private static int _emptySpot;
    private static int _oldIndex;
    private static int _numberOfCoins;
    private static bool _initAfterContinuation;
    private static int _currentAxisOfCoin;
    private int _currentAxisOfObstacle;
    
    void Start()
    {
        Debug.Log(_highPlatformSpawning);
        _coinContainer = GameObject.Find("CoinContainer");
        _obstacleContainer = GameObject.Find("ObstacleContainer");
        _currentAxisOfObstacle = 10;
        CheckIfHigh();
        InitializeHighPlatforms();
        InitializeObstacles();
        CheckCoinSpawning();
        InitializeCoin();
        
    }
    
    private static void CheckIfHigh()
    {
        if (!_highPlatformSpawning && GameManager.PlatformCooldown <= 0)
        {
            //var chanceToBeHigh = Random.Range(0, 6);
            //if (chanceToBeHigh != 3)
            //{
                _highPlatformSpawning = true;
                _numberOfHighPlatforms = Random.Range(6, 10);
                _emptySpot = Random.Range(3, _numberOfHighPlatforms);
                // 0 Left, 1 Middle, 2 Right
                _currentAxisOfHigh = Random.Range(0, 3);
            //}
        }
    }

    private void InitializeObstacles()
    {
        var chanceOfObstacles = Random.Range(0, 2);
        if (chanceOfObstacles == 1 && GameManager.BoxSpawnCooldown <= 0)
        {
            var boxOrTunnel = Random.Range(0, 2);
            var position = transform.position;
            if (_highPlatformSpawning)
            {
                var spots = new List<int>() { -1, 0, 1 };
                var boxXPos = 0;
                spots.Remove(_currentAxisOfHigh - 1);
                if (_initAfterContinuation)
                {
                    spots.Remove(_oldIndex - 1);
                    spots.Remove(_currentAxisOfHigh - 1);
                    boxXPos = spots[0];
                }
                else
                {
                    var coinToss = Random.Range(0, 2);
                    boxXPos = spots[coinToss];
                }
                _currentAxisOfObstacle = boxXPos;
                if (boxOrTunnel == 1)
                {
                    var spawnedBox = Instantiate(box, new Vector3(boxXPos, position.y + 0.8f, position.z), Quaternion.identity);
                    spawnedBox.transform.SetParent(_obstacleContainer.transform);
                    var spawnedCoin = Instantiate(coin, new Vector3(boxXPos, position.y + 3f, position.z),
                        Quaternion.identity);
                    spawnedCoin.transform.SetParent(_coinContainer.transform);
                }
                else
                {
                    var spawnedTunnel = Instantiate(tunnel, new Vector3(boxXPos, transform.position.y, position.z), Quaternion.identity);
                    spawnedTunnel.transform.SetParent(_obstacleContainer.transform);
                    var spawnedCoin = Instantiate(coin, new Vector3(boxXPos, position.y + 1.2f, position.z),
                        Quaternion.identity);
                    spawnedCoin.transform.SetParent(_coinContainer.transform);
                }
            }
            else
            {
                var xPos = Random.Range(-1, 2);
                _currentAxisOfObstacle = xPos;
                if (boxOrTunnel == 1)
                {
                    var spawnedBox = Instantiate(box, new Vector3(xPos, transform.position.y + 0.8f, position.z), Quaternion.identity);
                    spawnedBox.transform.SetParent(_obstacleContainer.transform);
                    var spawnedCoin = Instantiate(coin, new Vector3(xPos, position.y + 3f, position.z),
                        Quaternion.identity);
                    spawnedCoin.transform.SetParent(_coinContainer.transform);
                }
                else
                {
                    var spawnedTunnel = Instantiate(tunnel, new Vector3(xPos, transform.position.y, position.z), Quaternion.identity);
                    spawnedTunnel.transform.SetParent(_obstacleContainer.transform);
                    var spawnedCoin = Instantiate(coin, new Vector3(xPos, position.y + 1.2f, position.z),
                        Quaternion.identity);
                    spawnedCoin.transform.SetParent(_coinContainer.transform);
                }
            }
            Destroy(_obstacleContainer.transform.GetChild(0).gameObject);
                GameManager.BoxSpawnCooldown += 2;
                GameManager.PlatformCooldown += 0.5f;
        }
    }

    private void InitializeHighPlatforms()
    {
        if (_highPlatformSpawning)
        {
            if (_numberOfHighPlatforms != _emptySpot)
            {
                var position = transform.position;
                var indexOfHighPlatform = _currentAxisOfHigh + 3;
                transform.GetChild(_currentAxisOfHigh).gameObject.SetActive(false);  
                transform.GetChild(indexOfHighPlatform).gameObject.SetActive(true);
                var spawnedCoin = Instantiate(coin,
                    new Vector3(_currentAxisOfHigh - 1, position.y + 2.6f, position.z),
                    Quaternion.identity);
                spawnedCoin.transform.SetParent(_coinContainer.transform);
                if (_initAfterContinuation)
                {
                    var index = _oldIndex + 3;
                    transform.GetChild(_oldIndex).gameObject.SetActive(false);  
                    transform.GetChild(index).gameObject.SetActive(true);
                    _initAfterContinuation = false;
                    Debug.Log("Should Initialize two");
                }
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
                    _numberOfHighPlatforms = Random.Range(3, 10);
                }
                else
                {
                    _highPlatformSpawning = false;
                        GameManager.PlatformCooldown += 0.5f;
                        GameManager.BoxSpawnCooldown += 1;
                }
            }
        }
    }

    private void CheckCoinSpawning()
    {
        var chanceOfCoinSpawning = Random.Range(0, 4);
        if (chanceOfCoinSpawning == 2)
        {
            _coinSpawning = true;
            _currentAxisOfCoin = Random.Range(-1, 2);
            _numberOfCoins = Random.Range(4, 10);
        }
    }

    private void InitializeCoin()
    {
        var position = transform.position;
        if (_coinSpawning && (_currentAxisOfCoin != _currentAxisOfHigh - 1) && (_currentAxisOfCoin != _currentAxisOfObstacle))
        {
            var spawnedCoin = Instantiate(coin, new Vector3(_currentAxisOfCoin, position.y + 1, position.z), Quaternion.identity);
            spawnedCoin.transform.SetParent(_coinContainer.transform);
            _numberOfCoins--;
        }

        if (_numberOfCoins == 0)
        {
            _coinSpawning = false;
        }
    }
}