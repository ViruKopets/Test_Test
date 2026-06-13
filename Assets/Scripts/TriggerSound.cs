using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    [SerializeField] AudioManager AudioMan;
    [SerializeField] bool Sfx;
    [SerializeField] bool Ui;
    [SerializeField] int AudioIndex;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlaySoundByIndex();
    }

    public void PlaySoundByIndex()
    {
        if (Sfx)
        {
            AudioMan.PlaySFX(AudioIndex);

        }
        else if(Ui) 
        {
            AudioMan.PlayUI(AudioIndex);
        }
    }
}
