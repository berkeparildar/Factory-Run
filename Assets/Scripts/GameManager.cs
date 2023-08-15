using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float cooldown;

    [SerializeField] private GameObject platform;
    [SerializeField] private Player player;
    private static int _lastAddedPosition;
    private static int _playerTargetPosition;
    

    [SerializeField] private GameObject platformContainer;

    // Start is called before the first frame update
    void Start()
    {
        cooldown = 0;
        _playerTargetPosition = 3;
        _lastAddedPosition = 12;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerPosition();
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        Debug.Log(cooldown);
    }

    private void CheckPlayerPosition()
    {
        if (player.transform.position.z >= _playerTargetPosition)
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
        Destroy(platformContainer.transform.GetChild(0).gameObject);
    }
}