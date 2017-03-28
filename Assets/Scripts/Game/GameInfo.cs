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

    private GameInfo()
    {
        
    }

    public static GameInfo GetInstance()
    {
        if (instance == null)
        {
            instance = new GameInfo();
        }
        return instance;
    }

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

    public MarkerType GetPlayerMarker()
    {
        return m_PlayerMarker;
    }

    public MarkerType GetAIMarker()
    {
        return m_AIMarker;
    }

    public void SetPlayerFirst(bool newBool)
    {
        m_PlayerFirst = newBool;
    }

    public bool GetPlayerFirst()
    {
        return m_PlayerFirst;
    }
}
