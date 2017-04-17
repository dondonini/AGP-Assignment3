using UnityEngine;

public class SaveData
{

    public const int Length = 4;

    public string m_saveName { get; set; }
    public int m_TotalWins { get; set; }
    public int m_TotalLosses { get; set; }
    private int m_SaveID;

    // ////////////
    // Constructors
    // ////////////

    public SaveData()
    {
        m_saveName = "No-name";

        m_TotalWins = 0;
        m_TotalLosses = 0;

        m_SaveID = GenerateUniqueID();
    }

    public SaveData(string newName)
    {
        m_saveName = newName;

        m_TotalWins = 0;
        m_TotalLosses = 0;

        m_SaveID = GenerateUniqueID();
    }

    public SaveData(string newName, int newID)
    {
        m_saveName = newName;

        m_TotalWins = 0;
        m_TotalLosses = 0;

        m_SaveID = newID;
    }

    public SaveData(string newName, int newWins, int newLosses, int newID)
    {
        m_saveName = newName;

        m_TotalWins = newWins;
        m_TotalLosses = newLosses;

        m_SaveID = newID;
    }

    // ///////////////////
    // Getters and Setters
    // ///////////////////

    /// <summary>
    /// Returns the save ID
    /// </summary>
    /// <returns></returns>
    public int GetSaveID()
    {
        return m_SaveID;
    }

    /// <summary>
    /// Convert array to SaveData
    /// </summary>
    /// <param name="newData">Save array</param>
    /// <returns>Save data</returns>
    public static SaveData StringArrayToSaveData(string[] newData)
    {
        if (newData.Length > Length || newData.Length == 0)
        {
            return null;
        }

        // Creating savedata instance
        SaveData newSaveData = new SaveData(newData[0]);

        // Getting save name
        newSaveData.m_saveName = newData[0];

        // Getting save total wins
        newSaveData.m_TotalWins = int.Parse(newData[1]);

        // Getting save total losses
        newSaveData.m_TotalLosses = int.Parse(newData[2]);

        return newSaveData;
    }

    /// <summary>
    /// Convert SaveData to array
    /// </summary>
    /// <param name="newData">Save data</param>
    /// <returns>Save array</returns>
    public static string[] SaveDataToStringArray(SaveData newData)
    {
        string[] tempData = new string[Length];

        tempData[0] = newData.m_saveName;

        tempData[1] = newData.m_TotalWins.ToString();

        tempData[2] = newData.m_TotalLosses.ToString();

        return tempData;
    }

    // ///////////////////
    // Unique ID Generator
    // ///////////////////

    /// <summary>
    /// Generates a unique ID based on time
    /// </summary>
    /// <returns>Unique ID</returns>
    private static int GenerateUniqueID()
    {
        string tempID = "";

        float currentTime = Time.time;

        // Setting "unique" random seed
        Random.InitState((int)currentTime);

        // Generating unique ID
        for (int i = 0; i < 20; i++)
        {
            int randomNum = Random.Range(0, 9);

            tempID += randomNum.ToString();
        }

        // Checks if ID is too long
        if (tempID.Length > 5)
        {
            Debug.Log("Generated ID: " + tempID);
            Debug.Log("ID is too long... weird... REGENERATING");

            // Recursion
            return GenerateUniqueID();
        }

        // Converting ID to int
        int IDConversion = int.Parse(tempID);

        Debug.Log("Generated ID: " + IDConversion);

        return IDConversion;
    }
}
