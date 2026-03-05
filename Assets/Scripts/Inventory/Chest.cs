using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] string ItemsPurp;

    public string PassInfo()
    {
        return ItemsPurp;
    }
    public void ItemFit()
    {
        Destroy(this.gameObject);
    }
}
