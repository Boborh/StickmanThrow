using UnityEngine;

public static class GameRules
{
    public enum GameMode { Normal, PlayerChanceDecreases }
    public static GameMode CurrentMode = GameMode.Normal;

    public static int PassesBeforeDecay = 10; //number of passes with full chance
    public static int TotalPasses = 0;
    public static int PassesUntilZero = 15;
    public static StickmanController PlayerRef;

    public static float CurrentPlayerChance()
    {
        //if in Normal mode, no decay at all
        if (CurrentMode == GameMode.Normal)
            return 1f;

        //for omega mode (PlayerChanceDecreases):

        //before decay threshold: full chance
        if (TotalPasses <= PassesBeforeDecay)
            return 1f;

        //after threshold: start decaying from 1 -> 0
        Debug.Log("Decay started");
        int effectiveRange = Mathf.Max(1, PassesUntilZero - PassesBeforeDecay);
        float t = Mathf.Clamp01((float)(TotalPasses - PassesBeforeDecay) / effectiveRange);

        return 1f - t;
    }

    public static void Reset()
    {
        TotalPasses = 0;
    }
}