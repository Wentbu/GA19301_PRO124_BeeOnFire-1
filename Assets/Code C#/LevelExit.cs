using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] public MauNVC HocThuc;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && HocThuc.currentHealth >= 70)
        {
            StartCoroutine(LoadTrueEnd());
        }

        else if (other.gameObject.CompareTag("Player") && (HocThuc.currentHealth < 70 || HocThuc.currentHealth >= 30))
        {
            StartCoroutine(LoadNormalEnd());
        }

        else
        {
            StartCoroutine(LoadBadEnd());
        }
    }

    IEnumerator LoadBadEnd()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //int nextSceneIndex = currentSceneIndex + 1;

        //if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        //{
        //    nextSceneIndex = 0;
        //}
        //FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene("BadEnd");
    }

    IEnumerator LoadNormalEnd()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);

        SceneManager.LoadScene("NormalEnd");
    }

    IEnumerator LoadTrueEnd()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);

        SceneManager.LoadScene("CutScenes");
    }
}
