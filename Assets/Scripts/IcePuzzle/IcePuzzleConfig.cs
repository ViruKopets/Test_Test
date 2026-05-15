using UnityEngine;

[CreateAssetMenu(fileName = "IcePuzzleConfig", menuName = "Ice Puzzle/Config")]
public class IcePuzzleConfig : ScriptableObject
{
    [Header("Compass damage")]
    public int initialCondition = 100;
    public int damagePerWrongStrike = 20;

    [Header("Hint trigger")]
    [Tooltip("After this many wrong strikes, the 'tension lines' hint dialog fires.")]
    public int hitsBeforeHint = 2;
    [Tooltip("After this many wrong strikes, the Investigate button appears (usually one more than hitsBeforeHint).")]
    public int hitsBeforeButton = 3;

    [Header("Weak points")]
    public float weakPointHitRadius = 0.6f;

    [Header("Visual")]
    public bool randomizeCrackOrder = true;
}
