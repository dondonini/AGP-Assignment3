public class SaveData {

    public const int Length = 5;

    public string m_saveName { get; set; }
    public int m_highStreak { get; set; }
    public int m_WinStreak { get; set; }
    public int m_TotalWins { get; set; }
    public int m_TotalLosses { get; set; }
    private int m_SaveID;

    public int GetSaveID()
    {
        return m_SaveID;
    }

    public void SetSaveID(int newID)
    {
        m_SaveID = newID;
    }

    public static SaveData StringArrayToSaveData(string[] newData)
    {
        if (newData.Length > Length || newData.Length == 0)
        {
            return null;
        }

        // Creating savedata instance
        SaveData newSaveData = new SaveData();

        // Getting save name
        newSaveData.m_saveName = newData[0];

        // Getting save highscore
        newSaveData.m_highStreak = int.Parse(newData[1]);

        // Getting save win streak
        newSaveData.m_WinStreak = int.Parse(newData[2]);

        // Getting save total wins
        newSaveData.m_TotalWins = int.Parse(newData[3]);

        // Getting save total losses
        newSaveData.m_TotalLosses = int.Parse(newData[4]);

        return newSaveData;
    }

    public static string[] SaveDataToStringArray(SaveData newData)
    {
        string[] tempData = new string[Length];

        tempData[0] = newData.m_saveName;

        tempData[1] = newData.m_highStreak.ToString();

        tempData[2] = newData.m_WinStreak.ToString();

        tempData[3] = newData.m_TotalWins.ToString();

        tempData[4] = newData.m_TotalLosses.ToString();

        return tempData;
    }
}
