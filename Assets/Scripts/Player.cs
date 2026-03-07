using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float Speed;
    [SerializeField] SpriteRenderer Sprite;
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
        if (hor < 0)
        {
            Sprite.flipX = true;
        }
        else if (hor > 0)
        {
            Sprite.flipX = false;
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
