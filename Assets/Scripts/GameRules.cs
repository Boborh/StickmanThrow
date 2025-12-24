using UnityEngine;

public static class GameRules
{
    public enum GameMode { Normal, PlayerChanceDecreases }
    public static GameMode CurrentMode = GameMode.Normal;

    public static int TotalPasses = 0;                 // incremented on ReceiveBall
    public static StickmanController PlayerRef;

    // OMEGA SETTINGS
    public static int GuaranteedPlayerPasses = 5;      // EXACTLY N
    public static int GuaranteedWithinTotalPasses = 15; // X
    public static int PlayerPassesReceived = 0;        // incremented when player receives

    public static void Reset()
    {
        TotalPasses = 0;
        PlayerPassesReceived = 0;
    }

    private static int NextReceiveNumber => TotalPasses + 1;

    private static bool IsNextReceiveWithinWindow()
    {
        return NextReceiveNumber <= GuaranteedWithinTotalPasses;
    }

    // Deadline for k-th player receive: ceil(k*X/N)
    private static int DeadlineForKthReceive(int k)
    {
        return Mathf.CeilToInt((k * (float)GuaranteedWithinTotalPasses) / GuaranteedPlayerPasses);
    }

    // If player is "ahead of schedule", temporarily disallow to avoid streaks.
    // maxAllowedByNow grows stepwise from 0..N across the window.
    private static int MaxAllowedByNow()
    {
        // Example: N=5, X=10
        // NextReceiveNumber: 1..10  -> maxAllowed: 0,1,1,2,2,3,3,4,4,5
        float ratio = (NextReceiveNumber * (float)GuaranteedPlayerPasses) / GuaranteedWithinTotalPasses;
        return Mathf.FloorToInt(ratio);
    }

    public static bool MustForceNextPassToPlayer()
    {
        if (CurrentMode != GameMode.PlayerChanceDecreases) return false;
        if (PlayerRef == null) return false;
        if (!IsNextReceiveWithinWindow()) return false;
        if (PlayerPassesReceived >= GuaranteedPlayerPasses) return false;

        int k = PlayerPassesReceived + 1;
        int deadline = DeadlineForKthReceive(k);

        // Force only when we are at/after the deadline for the next required receive.
        return NextReceiveNumber >= deadline;
    }

    public static bool CanBotsPassToPlayer()
    {
        if (CurrentMode != GameMode.PlayerChanceDecreases) return true;

        if (!IsNextReceiveWithinWindow()) return false;

        // Never exceed exact quota
        if (PlayerPassesReceived >= GuaranteedPlayerPasses) return false;

        // If we're behind schedule, allow (and possibly force)
        if (MustForceNextPassToPlayer()) return true;

        // If we're ahead of schedule, disallow to prevent streaks
        return PlayerPassesReceived < MaxAllowedByNow();
    }

    public static bool IsAllowedTarget(StickmanController candidate)
    {
        if (candidate == null) return false;
        if (CurrentMode != GameMode.PlayerChanceDecreases) return true;

        if (candidate == PlayerRef)
            return CanBotsPassToPlayer();

        return true;
    }
}
