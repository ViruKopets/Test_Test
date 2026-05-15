using UnityEngine;

public class Camerafollower : MonoBehaviour
{
    [SerializeField] GameObject Target;
    [SerializeField] Vector3 Offset = new Vector3(0, 0, -10);
    [SerializeField] float LeftX;
    [SerializeField] float RightX;
    Vector3 RightVector;
    void Update()
    {
        if (Target == null) return;
        Vector3 tp = Target.transform.position;
        float x = tp.x + Offset.x;
        if (x < LeftX) x = LeftX;
        else if (x > RightX) x = RightX;
        RightVector = new Vector3(x, tp.y + Offset.y, tp.z + Offset.z);
        transform.position = RightVector;
    }
}
