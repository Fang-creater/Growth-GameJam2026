using UnityEngine;

namespace Regrowth
{
    public static class LevelProgress
    {
        private const string PrefKey = "MaxUnlockedLevelBuildIndex";

        // Äã Build Settings Àï Level1 ÊÇ 3£¨MainMenu0, LevelSelect1, Loading2, Level1=3£©
        public const int FirstLevelBuildIndex = 3;

        public static int GetMaxUnlocked()
        {
            return PlayerPrefs.GetInt(PrefKey, FirstLevelBuildIndex);
        }

        public static void UnlockBuildIndex(int buildIndex)
        {
            int cur = GetMaxUnlocked();
            if (buildIndex > cur)
            {
                PlayerPrefs.SetInt(PrefKey, buildIndex);
                PlayerPrefs.Save();
            }
        }
    }
}