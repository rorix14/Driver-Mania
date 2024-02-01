using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartTransition(string scene)
    {
        StartCoroutine(Transition(scene));
    }
    private IEnumerator Transition(string toScene)
    {
        DontDestroyOnLoad(gameObject);
        var player = GameObject.FindWithTag("Player");
        if (player)
            player.SetActive(false);
        
        yield return SceneManager.LoadSceneAsync(toScene);
        Destroy(gameObject);
    }
}