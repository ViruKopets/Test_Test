using UnityEngine;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{
    [SerializeField] string ItemsPurp;
    [SerializeField] bool Delete = true;
    [SerializeField] DialogueScr DialogueToActivate;
    [SerializeField] GameObject TurnOn;
    [SerializeField] GameObject TurnOff;
    [SerializeField] bool IsProgress;
    [SerializeField] int ProggressId;
    
    [Header("Audio")]
    [SerializeField] bool Sfx;
    [SerializeField] bool UI;
    [SerializeField] AudioManager audioManager;
    [SerializeField] int AudioIndex;

    [SerializeField] bool endgate;

    public string PassInfo()
    {
        return ItemsPurp;
    }
    public void ItemFit()
    {
        if (DialogueToActivate != null)
        {
            DialogueToActivate.ActivateDialogue();
        }
        if (IsProgress)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
            objects[0].GetComponent<GameManager>().Progressed(ProggressId);
        }
        if (TurnOn != null)
        {
            TurnOn.SetActive(true);
        }
        if (TurnOff != null)
        {
            TurnOff.SetActive(false);
        }
        if (endgate)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
            if (objects[0].GetComponent<GameManager>().IsGoodEnding())
            {
                SceneManager.LoadScene("GoodEnding");
            }
            else
            {
                SceneManager.LoadScene("BadEnding");
            }

        }
        if (Sfx)
        {
            audioManager.PlaySFX(AudioIndex);

        }
        else if (UI)
        {
            audioManager.PlayUI(AudioIndex);
        }
        
        if (Delete) Destroy(this.gameObject);
    }
}
