using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] string ItemsPurp;
    [SerializeField] bool Delete = true; 
    public string PassInfo()
    {
        return ItemsPurp;
    }
    public void ItemFit()
    {
        Destroy(this.gameObject);
    }
}
