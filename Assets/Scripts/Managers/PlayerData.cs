using UnityEngine;

public class PlayerData {

    private static PlayerData instance = null;

    private static bool B_WIPE_ALL_DATA = true;

    public enum SaveEventMessages
    {
        SaveSuccess,
        SaveOverrideSuccess,
        SaveSlotEmpty,
        UndocumentedBehaviour
    }

    // ////
    // Keys
    // ////

    private const string K_HIGHSCORE = "highScoreKey";
    private const string K_SAVE1 = "save1Key";
    private const string K_SAVE2 = "save2Key";

    // /////////
    // Variables
    // /////////

    private int m_Score;
    private int m_HighScore;
    private bool m_ShowHighScoreAlert;
    private SaveData m_ActiveSave = null;

    // //////
    // Events
    // //////

    public delegate void HighScoreEventAction();
    public delegate void ScoreEventAction();
    public static event HighScoreEventAction OnHighScoreChanged;
    public static event ScoreEventAction OnScoreChanged;

    // Save 1
    private SaveData m_Save1;

    // Save 2
    private SaveData m_Save2;

    // ///////////
    // Constructor
    // ///////////

    private PlayerData()
    {
        // Getting local data...

        // Loading high score
        m_HighScore = PlayerPrefs.GetInt(K_HIGHSCORE);

        // Saves
        string[] temp;

        // Loading saved data 1
        temp = PlayerPrefsX.GetStringArray(K_SAVE1);

        // Extracting saved data 1
        if (SaveData.StringArrayToSaveData(temp) == null) 
        {
            Debug.Assert(true, "Save 1 is corrupted!");
        }
        else
        {
            m_Save1 = SaveData.StringArrayToSaveData(temp);
        }

        // Loading saved data 2
        temp = PlayerPrefsX.GetStringArray(K_SAVE2);

        // Extracting saved data 2
        if (SaveData.StringArrayToSaveData(temp) == null)
        {
            Debug.Assert(true, "Save 1 is corrupted!");
        }
        else
        {
            m_Save2 = SaveData.StringArrayToSaveData(temp);
        }
    }

    // ///////////////////
    // Getters and Setters
    // ///////////////////

    /// <summary>
    /// Gets current instance of PlayerData
    /// </summary>
    /// <returns></returns>
    public static PlayerData GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerData();
        }
        return instance;
    }

    /// <summary>
    /// Returns current saved data in the first slot
    /// </summary>
    /// <returns>Save data</returns>
    public SaveData GetSavedData1()
    {
        // Checks if save slot 1 exists
        if (m_Save1 == null)
        {
            // Creates new slot if empty
            m_Save1 = new SaveData();
        }

        return m_Save1;
    }

    /// <summary>
    /// Returns current saved data in the second slot
    /// </summary>
    /// <returns>Save data</returns>
    public SaveData GetSavedData2()
    {
        // Checks if save slot 2 exists
        if (m_Save2 == null)
        {
            // Creates new slot if empty
            m_Save2 = new SaveData();
        }

        return m_Save2;
    }

    /// <summary>
    /// Loads the given save
    /// </summary>
    /// <param name="toLoad"></param>
    public bool LoadSave(SaveData toLoad)
    {
        if (toLoad.GetSaveID() == GetSavedData1().GetSaveID() || toLoad.GetSaveID() == GetSavedData2().GetSaveID())
        {
            ActiveSave = toLoad;
        }

        return true;
    }

    public bool SaveSave(int slot)
    {
        switch(slot)
        {
            case 0:
                m_Save1 = ActiveSave;
                break;
            case 1:
                m_Save2 = ActiveSave;
                break;
        }

        SaveEventMessages result = SetSaveData();

        Debug.Log(result.ToString());

        if (result == SaveEventMessages.SaveSuccess)
            return true;

        return false;
    }

    /// <summary>
    /// Currently used save
    /// </summary>
    public SaveData ActiveSave
    {
        get
        {
            if (m_ActiveSave == null)
            {
                m_ActiveSave = new SaveData();
            }

            return m_ActiveSave;
        }
        set
        {
            m_ActiveSave = value;
        }
    }

    /// <summary>
    /// Returns the current score
    /// </summary>
    /// <returns>Score</returns>
    public int GetCurrentScore()
    {
        return m_Score;
    }

    /// <summary>
    /// Returns the current high score
    /// </summary>
    /// <returns>High score</returns>
    public int GetHighScore()
    {
        m_ShowHighScoreAlert = true;

        return m_HighScore;
    }

    /// <summary>
    /// Sets the current active score. Also, updates the high score
    /// </summary>
    /// <param name="newScore"></param>
    public void SetCurrentScore(int newScore)
    {
        if (newScore > m_HighScore)
        {
            m_HighScore = newScore;
            OnHighScoreChanged();
        }

        m_Score = newScore;
        OnScoreChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SaveEventMessages OverrideSaveData()
    {
        if (m_ActiveSave == null)
        {
            return SaveEventMessages.SaveSlotEmpty;
        }

        if (m_Save1.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            m_Save1 = m_ActiveSave;
            return SaveEventMessages.SaveOverrideSuccess;
        }
        else if (m_Save2.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            m_Save2 = m_ActiveSave;
            return SaveEventMessages.SaveOverrideSuccess;
        }

        return SaveEventMessages.UndocumentedBehaviour;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SaveEventMessages SetSaveData()
    {
        if (m_ActiveSave == null)
        {
            return SaveEventMessages.SaveSlotEmpty;
        }

        if (OverrideSaveData() != SaveEventMessages.SaveOverrideSuccess)
        {
            return SaveEventMessages.SaveOverrideSuccess;
        }

        string[] temp;

        if (m_Save1.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            temp = SaveData.SaveDataToStringArray(m_Save1);

            ExtendedFunctions.PrintArray(temp);

            PlayerPrefsX.SetStringArray(K_SAVE1, temp);
        }
        else if (m_Save2.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            temp = SaveData.SaveDataToStringArray(m_Save2);
            PlayerPrefsX.SetStringArray(K_SAVE2, temp);
        }

        PlayerPrefs.SetInt(K_HIGHSCORE, m_HighScore);

        return SaveEventMessages.SaveSuccess;
    }

}
