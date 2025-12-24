using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        // --- OMEGA: force player if required to guarantee 4 within X ---
        if (GameRules.CurrentMode == GameRules.GameMode.PlayerChanceDecreases && GameRules.PlayerRef != null)
        {
            if (GameRules.MustForceNextPassToPlayer())
            {
                if (GameRules.PlayerRef != stickman)
                {
                    stickman.InitiatePass(GameRules.PlayerRef);
                    passing = false;
                    yield break;
                }
            }
        }

        // Build candidates excluding self and applying omega "hard block"
        List<StickmanController> candidates = new List<StickmanController>();
        foreach (var s in all)
        {
            if (s == null) continue;
            if (s == stickman) continue;
            if (!GameRules.IsAllowedTarget(s)) continue;
            candidates.Add(s);
        }

        // Fallback: if none available, pick anyone-but-self (still respect IsAllowedTarget)
        if (candidates.Count == 0)
        {
            foreach (var s in all)
            {
                if (s == null) continue;
                if (s == stickman) continue;
                if (!GameRules.IsAllowedTarget(s)) continue;
                candidates.Add(s);
            }
        }

        if (candidates.Count > 0)
        {
            StickmanController target = candidates[Random.Range(0, candidates.Count)];
            stickman.InitiatePass(target);
        }

        passing = false;
    }
}
