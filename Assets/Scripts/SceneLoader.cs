using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string SceneName;
    [SerializeField] GameManager Gm;
    [SerializeField] int NextScenePosId;
    [SerializeField] bool LoadByTrigger = true;
    [SerializeField] bool ByProgress = false;
    [SerializeField] int ProgressCheck;
    [SerializeField] DialogueScr NotYetDialigue;

    [Header("First Menu Load")]
    [SerializeField] bool MenuLoad = false;
    [SerializeField] float Time = 1f;
    [SerializeField] GameObject BlackPanel;

    private void Start()
    {
        if (Gm == null && !MenuLoad)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
            Gm = objects[0].GetComponent<GameManager>();
        }
    }
    public void LoadScene()
    {
        if (MenuLoad)
        {
            StartCoroutine(MenuPanel());
        }
        else
        {
            if (Gm == null)
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
                Gm = objects[0].GetComponent<GameManager>();
            }
            if (ByProgress)
            {
                if (!Gm.CheckProgress(ProgressCheck))
                {
                    NotYetDialigue.ActivateDialogue();
                    return;
                }
            }
            Gm.SetTransitionInfo(NextScenePosId);
            if (SceneName == "0MainMenu")
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
                Destroy(objects[0]);
                objects = GameObject.FindGameObjectsWithTag("InventoryCanvas");
                Destroy(objects[0]);

            }
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
        if (LoadByTrigger) LoadScene();
    }

    public void QuitBut()
    {
        Application.Quit();
    }
}
