using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float Speed;
    float hor;

    void Update()
    {
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
}
