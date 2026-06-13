using UnityEngine;

public class PictureCodePuzzle : MonoBehaviour
{
    [Header("4 puzzle pictures (left to right)")]
    [SerializeField] Transform[] Pictures = new Transform[4];

    [Header("Picture #5 — appears on success")]
    [SerializeField] Transform RevealPicture;

    [Header("Code (clicks per picture, left to right)")]
    [SerializeField] int[] ExpectedCode = { 3, 1, 2, 4 };

    [Header("Tuning")]
    [SerializeField] float DegreesPerClick = 15f;
    [SerializeField] int   ClicksToReset   = 5;
    [Tooltip("Direction sign per picture (+1 = right, -1 = left). Defaults to alternating starting right.")]
    [SerializeField] int[] Directions = { +1, -1, +1, -1 };

    [Header("Wiring")]
    [SerializeField] Camera Cam;

    [SerializeField] AudioManager AudioManager;

    int[] clickCounts;
    Quaternion[] originalLocalRot;
    bool solved;

    void Start()
    {
        clickCounts = new int[Pictures.Length];
        originalLocalRot = new Quaternion[Pictures.Length];
        for (int i = 0; i < Pictures.Length; i++)
            if (Pictures[i] != null)
                originalLocalRot[i] = Pictures[i].localRotation;
    }

    void Update()
    {
        if (solved) return;
        if (!Input.GetMouseButtonDown(0)) return;
        var cam = Cam != null ? Cam : Camera.main;
        if (cam == null) return;

        Vector2 mp = cam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mp, Vector2.zero);
        if (hit.collider == null) return;

        for (int i = 0; i < Pictures.Length; i++)
        {
            if (Pictures[i] == null) continue;
            if (hit.collider.transform == Pictures[i] ||
                hit.collider.transform.IsChildOf(Pictures[i]))
            {
                OnPictureClicked(i);
                return;
            }
        }
    }

    void OnPictureClicked(int idx)
    {
        AudioManager.PlaySFXRanPitch(1);
        clickCounts[idx] = (clickCounts[idx] + 1) % Mathf.Max(1, ClicksToReset);

        float dir = (Directions != null && idx < Directions.Length) ? Mathf.Sign(Directions[idx]) : ((idx % 2 == 0) ? +1f : -1f);
        if (dir == 0f) dir = +1f;
        float angle = dir * DegreesPerClick * clickCounts[idx];
        // Pure 2D rotation around the local Z axis — does not skew the sprite regardless of original X/Y rotation.
        Pictures[idx].localRotation = originalLocalRot[idx] * Quaternion.AngleAxis(angle, Vector3.forward);

        if (CodeMatches()) Solve();
    }

    bool CodeMatches()
    {
        if (ExpectedCode == null || ExpectedCode.Length != clickCounts.Length) return false;
        for (int i = 0; i < clickCounts.Length; i++)
            if (clickCounts[i] != ExpectedCode[i]) return false;
        return true;
    }

    void Solve()
    {
        solved = true;
        if (RevealPicture != null)
        {
            // Picture was hidden; it appears on correct code.
            RevealPicture.gameObject.SetActive(true);
            // Re-arm the click that advances the game (FlipAPic listens for "Pic" tag).
            RevealPicture.gameObject.tag = "Pic";
        }
    }
}
