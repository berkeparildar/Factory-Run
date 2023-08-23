using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static float PlatformCooldown;
    public static float BoxSpawnCooldown;
    public static int Coins;
    public GameObject endWall;
    [SerializeField] private GameObject platform;
    [SerializeField] private Player player;
    private static int _lastAddedPosition;
    private static int _playerTargetPosition;
    [SerializeField] private GameObject platformContainer;

    // Start is called before the first frame update
    private void Start()
    {
        PlatformCooldown = 0;
        BoxSpawnCooldown = 0;
        _playerTargetPosition = 3;
        _lastAddedPosition = 12;
       InitializeFirstPlatforms();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckPlayerPosition();
        CheckCoolDowns();
    }

    private void InitializeFirstPlatforms()
    {
        for (int i = 0; i < 50; i++)
        {
            var generatedPlatform = Instantiate(platform, new Vector3(0, 0, _lastAddedPosition), Quaternion.identity);
            generatedPlatform.transform.SetParent(platformContainer.transform);
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
        endWall.transform.position = (new Vector3(0, 2, generatedPlatform.transform.position.z + 1));
        Destroy(platformContainer.transform.GetChild(0).gameObject);
    }
}