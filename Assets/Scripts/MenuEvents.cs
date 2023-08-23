using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour
{
    public GameObject box;
    void Start()
    {
        StartCoroutine(BoxSpawner());
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
}
