using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] string ItemsPurp;
    [SerializeField] bool Delete = true;
    [SerializeField] DialogueScr DialogueToActivate;
    [SerializeField] GameObject TurnOn;
    [SerializeField] GameObject TurnOff;
    [SerializeField] bool IsProgress;
    [SerializeField] int ProggressId;
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
        if (Delete) Destroy(this.gameObject);
    }
}
