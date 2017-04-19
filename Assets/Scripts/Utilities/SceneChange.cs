using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

    [SerializeField]
    private string m_selectedScene = "";

	public void SetScene()
    {
        if (m_selectedScene == "")
            return;

        SceneManager.LoadScene(m_selectedScene);
    }

    public void SetSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetSceneByNameDelayed(string sceneName, float duration)
    {
        StartCoroutine(DelayedSceneSetByName(sceneName, duration));
    }

    private IEnumerator DelayedSceneSetByName(string sceneName, float duration)
    {
        string savedSceneName = sceneName;

        yield return new WaitForSeconds(duration);

        SetSceneByName(savedSceneName);
    }
}
