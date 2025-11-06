using UnityEngine;
using System.Collections;

public class BotController : MonoBehaviour
{
    public StickmanController stickman;
    public float delay = 1f;
    private bool passing = false;

    void Update()
    {
        if (stickman.hasBall && !passing)
            StartCoroutine(PassRoutine());
    }

    IEnumerator PassRoutine()
    {
        passing = true;
        yield return new WaitForSeconds(delay);

        StickmanController[] all = FindObjectsOfType<StickmanController>();

        // Build candidate list excluding self
        var candidates = new System.Collections.Generic.List<StickmanController>();
        foreach (var s in all)
            if (s != stickman) candidates.Add(s);

        // MODE B: remove/keep the player based on current chance
        if (GameRules.CurrentMode == GameRules.GameMode.PlayerChanceDecreases && GameRules.PlayerRef != null)
        {
            float p = GameRules.CurrentPlayerChance(); // 1 → 0 over time
            if (Random.value > p)
            {
                // roll failed → don't target the player this pass
                candidates.Remove(GameRules.PlayerRef);
            }
        }

        // Fallback: if we removed down to zero by accident, just repopulate with anyone-but-self
        if (candidates.Count == 0)
        {
            foreach (var s in all)
                if (s != stickman) candidates.Add(s);
        }

        // Pick final target
        StickmanController target = candidates[Random.Range(0, candidates.Count)];
        stickman.InitiatePass(target);

        passing = false;
    }
}
