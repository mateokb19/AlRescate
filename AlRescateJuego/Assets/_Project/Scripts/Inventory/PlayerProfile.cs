public static class PlayerProfile
{
    public static int TotalPulls => SaveSystem.Current.totalPulls;
    public static int PityLegendary => SaveSystem.Current.pityLegendaryCounter;
    public static int PityEpic => SaveSystem.Current.pityEpicCounter;

    public static void RegisterPull(int n = 1)
    {
        SaveSystem.Current.totalPulls += n;
    }
}
