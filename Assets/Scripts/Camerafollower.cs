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
        if (Target.transform.position.x + Offset.x < LeftX)
        {
            RightVector = new Vector3(LeftX, Target.transform.position.y + Offset.y, Target.transform.position.z + Offset.z);
        }
        else if (Target.transform.position.x + Offset.x > RightX)
        {
            RightVector = new Vector3(RightX, Target.transform.position.y + Offset.y, Target.transform.position.z + Offset.z);
        }
        else
        {
            RightVector = new Vector3(Target.transform.position.x + Offset.x, Target.transform.position.y + Offset.y, Target.transform.position.z + Offset.z);
        }
        this.gameObject.transform.position = RightVector;
    }
}
