using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    private static PlayerData instance = null;

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

    // Constructor
    private PlayerData()
    {
        // Getting local data...

        // High score
        m_HighScore = PlayerPrefs.GetInt(K_HIGHSCORE);

        // Saves
        string[] temp;

        temp = PlayerPrefsX.GetStringArray(K_SAVE1);

        // Extracting saved data 1
        if (SaveData.StringArrayToSaveData(temp) == null) 
        {
            Debug.Assert(true, "Save 1 is corrupted! Reformatting data...");
            PlayerPrefsX.SetStringArray(K_SAVE1, new string[2]);
        }
        else
        {
            m_Save1 = SaveData.StringArrayToSaveData(temp);
        }

        temp = PlayerPrefsX.GetStringArray(K_SAVE2);

        // Extracting saved data 2
        if (SaveData.StringArrayToSaveData(temp) == null)
        {
            Debug.Assert(true, "Save 1 is corrupted! Reformatting data...");
            PlayerPrefsX.SetStringArray(K_SAVE2, new string[2]);
        }
        else
        {
            m_Save2 = SaveData.StringArrayToSaveData(temp);
        }
    }

    public static PlayerData GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerData();
        }
        return instance;
    }

    public SaveData GetSavedData1()
    {
        return m_Save1;
    }

    public SaveData GetSavedData2()
    {
        return m_Save2;
    }

    public SaveData ActiveSave
    {
        get
        {
            return m_ActiveSave;
        }
        set
        {
            m_ActiveSave = value;
        }
    }

    public int GetCurrentScore()
    {
        return m_Score;
    }

    public int GetHighScore()
    {
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

        if (m_Save1.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            PlayerPrefsX.SetStringArray(K_SAVE1, SaveData.SaveDataToStringArray(m_Save1));
        }
        else if (m_Save2.GetSaveID() == m_ActiveSave.GetSaveID())
        {
            PlayerPrefsX.SetStringArray(K_SAVE2, SaveData.SaveDataToStringArray(m_Save2));
        }

        return SaveEventMessages.SaveSuccess;
    }

}
