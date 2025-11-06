using UnityEngine;

public static class GameRules
{
    public enum GameMode { Normal, PlayerChanceDecreases }
    public static GameMode CurrentMode = GameMode.Normal;

    public static int TotalPasses = 0;
    public static int PassesUntilZero = 10;
    public static StickmanController PlayerRef;

    public static float CurrentPlayerChance()
    {
        float t = Mathf.Clamp01((float)TotalPasses / Mathf.Max(1, PassesUntilZero));
        return 1f - t;
    }

    public static void Reset()
    {
        TotalPasses = 0;
    }
}