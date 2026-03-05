using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string SceneName;

    [Header("First Menu Load")]
    [SerializeField] bool MenuLoad = false;
    [SerializeField] float Time = 1f;
    [SerializeField] GameObject BlackPanel;

    public void LoadScene()
    {
        if (MenuLoad)
        {
            StartCoroutine(MenuPanel());
        }
        else
        {
            SceneManager.LoadScene(SceneName);
        }
    }

    IEnumerator MenuPanel()
    {
        BlackPanel.SetActive(true);
        yield return new WaitForSeconds(Time);
        SceneManager.LoadScene(SceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LoadScene();
    }
}
