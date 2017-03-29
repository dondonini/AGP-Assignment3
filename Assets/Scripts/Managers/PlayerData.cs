using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    private static PlayerData instance = null;

    ////////////
    // Keys
    ////////////

    private const string K_HIGHSCORE = "highScoreKey";
    private const string K_SAVE1 = "save1Key";
    private const string K_SAVE2 = "save2Key";

    private int m_Score;
    private int m_HighScore;

    // Save 1
    private string m_Save1Name;
    private int m_Save1HighScore;

    // Save 2
    private string m_Save2Name;
    private int m_Save2HighScore;

    private PlayerData()
    {
        // Getting local data...

        // High score
        m_HighScore = PlayerPrefs.GetInt(K_HIGHSCORE);

        // Saves
        string[] temp;

        temp = PlayerPrefsX.GetStringArray(K_SAVE1);

        if (temp.Length > 2)
        {
            Debug.Assert(true, "Save 1 is corrupted! Reformatting data...");
            PlayerPrefsX.SetStringArray(K_SAVE1, new string[2]);
        }

        for (int i = 0; i < temp.Length; i++)
        {

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


    private SaveData ExtractSavedData(string[] newData)
    {
        if (newData.Length > 2)
        {
            return null;
        }

        // Creating savedata instance
        SaveData newSaveData = new SaveData();

        // Getting save name
        newSaveData.m_saveName = newData[0];

        // Getting save highscore
        newSaveData.m_highStreak = int.Parse(newData[1]);

    }
}
