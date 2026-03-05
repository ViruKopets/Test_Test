using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float Speed;
    float hor;
    bool Freeze;

    void Update()
    {
        if (Freeze) return;
        Moving();
    }
    public void Moving()
    {
        hor = Input.GetAxis("Horizontal");
        if (hor != 0)
        {
            rb.linearVelocity = new Vector2(Speed * hor, rb.linearVelocity.y);
        }
    }
    public void IsFreezed(bool IsIn)
    {
        if (IsIn)
        {
            rb.linearVelocity = Vector2.zero;
            Freeze = true;
        }
        else
        {
            Freeze = false;
        }
    }
}
