using System.Collections;
using UnityEngine;

public enum MarkerType
{
    NULL = 0,
    X = 1,
    O = 2
}

public class GameInfo
{
    private static GameInfo instance = null;

    private MarkerType m_PlayerMarker = MarkerType.NULL;
    private MarkerType m_AIMarker = MarkerType.NULL;
    private bool m_PlayerFirst = false;

    // ///////////
    // Constructor
    // ///////////

    private GameInfo()
    {
        
    }

    // ///////////////////
    // Getters and Setters
    // ///////////////////

    /// <summary>
    /// Returns current instance of GameInfo
    /// </summary>
    /// <returns>Instance</returns>
    public static GameInfo GetInstance()
    {
        if (instance == null)
        {
            instance = new GameInfo();
        }
        return instance;
    }

    /// <summary>
    /// Sets player marker type
    /// </summary>
    /// <param name="newMarker">Player MarkerType</param>
    public void SetPlayerMarker(MarkerType newMarker)
    {
        m_PlayerMarker = newMarker;

        if (m_AIMarker == MarkerType.NULL || m_AIMarker == newMarker)
        {
            if (newMarker == MarkerType.O)
            {
                m_AIMarker = MarkerType.X;
            }
            else
            {
                m_AIMarker = MarkerType.O;
            }
        }
    }

    /// <summary>
    /// Retruns player marker type
    /// </summary>
    /// <returns>Player MarkerType</returns>
    public MarkerType GetPlayerMarker()
    {
        return m_PlayerMarker;
    }

    /// <summary>
    /// Returns AI marker type
    /// </summary>
    /// <returns>AI MarkerType</returns>
    public MarkerType GetAIMarker()
    {
        return m_AIMarker;
    }

    /// <summary>
    /// Set player to go first
    /// </summary>
    /// <param name="newBool">Is player first</param>
    public void SetPlayerFirst(bool newBool)
    {
        m_PlayerFirst = newBool;
    }

    /// <summary>
    /// Returns if player is going first or not
    /// </summary>
    /// <returns>Is player first</returns>
    public bool GetPlayerFirst()
    {
        return m_PlayerFirst;
    }


}
