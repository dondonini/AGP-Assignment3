using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveVerdict : MonoBehaviour {

    // /////////
    // Instances
    // /////////

    private PlayerData playerData;
    private GameInfo gameInfo;

    // ///////////////////
    [Header("References")]
    // ///////////////////

    public Animator m_whiteWipe;
    public GameObject m_oMarker;
    public GameObject m_xMarker;
    public Transform m_markerDisplayPoint;
    [SerializeField]
    private Camera m_currentCamera;

    [SerializeField]
    private Text m_congratsMessage;
    [SerializeField]
    private Text m_statsTitle;
    [SerializeField]
    private Text m_winStats;
    [SerializeField]
    private Text m_lossStats;

    // //////////////////
    [Header("Variables")]
    // //////////////////

    public float m_BackgroundColorAnimationDamp = 1.0f;
    [Range(0.0f, 2.0f)]
    public float m_BackgroundColorBrightness = 1.0f;

    private Color m_BackgroundColorGoTo = Color.gray;
    private Vector3 m_backgroundVel = Vector3.zero;

    // Use this for initialization
    void Start () {
        playerData = PlayerData.GetInstance();
        gameInfo = GameInfo.GetInstance();

        GameObject currentMarker;

        switch (gameInfo.GetLatestWinner())
        {
            case GameOutcome.Tie:
                m_BackgroundColorGoTo = new Color
                    (
                        ExtendedFunctions.Convert8ToFloat(50),
                        ExtendedFunctions.Convert8ToFloat(50),
                        ExtendedFunctions.Convert8ToFloat(50)
                    );

                m_congratsMessage.text = "TIE GAME!";

                break;

            case GameOutcome.PlayerWon:

                currentMarker = GetMarker(gameInfo.GetPlayerMarker());

                m_BackgroundColorGoTo = ExtendedFunctions.GetColorOfGameObject(currentMarker);

                SetupMarkerDisplay(currentMarker);

                m_congratsMessage.text = "YOU WON!";

                break;

            case GameOutcome.AIWon:

                currentMarker = GetMarker(gameInfo.GetAIMarker());

                m_BackgroundColorGoTo = ExtendedFunctions.GetColorOfGameObject(currentMarker);

                SetupMarkerDisplay(currentMarker);

                m_congratsMessage.text = "YOU LOSS!";

                break;
        }

        m_statsTitle.text = playerData.ActiveSave.m_saveName + "'s stats";

        m_winStats.text = "Total wins\n" + playerData.ActiveSave.m_TotalWins;

        m_lossStats.text = "Total losses\n" + playerData.ActiveSave.m_TotalLosses;

    }
	

	// Update is called once per frame
	void Update () {
        // Changes background dynamically
        if (m_BackgroundColorBrightness > 1.0f)
        {
            Color goalColor = Color.Lerp(m_BackgroundColorGoTo, Color.white, m_BackgroundColorBrightness - 1.0f);

            m_currentCamera.backgroundColor = ExtendedFunctions.ColorSmoothDamp
            (
                m_currentCamera.backgroundColor,
                goalColor,
                ref m_backgroundVel,
                m_BackgroundColorAnimationDamp
            );
        }
        else
        {
            m_currentCamera.backgroundColor = ExtendedFunctions.ColorSmoothDamp
            (
                m_currentCamera.backgroundColor,
                m_BackgroundColorGoTo * m_BackgroundColorBrightness,
                ref m_backgroundVel,
                m_BackgroundColorAnimationDamp
            );
        }
    }

    private GameObject GetMarker(MarkerType mType)
    {
        switch(mType)
        {
            case MarkerType.O:
                return m_oMarker;

            case MarkerType.X:
                return m_oMarker;

            default:
                return null;
        }
    }

    private void SetupMarkerDisplay(GameObject marker)
    {
        GameObject newMarker = Instantiate(marker) as GameObject;

        newMarker.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);

        newMarker.transform.position = m_markerDisplayPoint.position;
        newMarker.transform.SetParent(m_markerDisplayPoint);
    }

    public void OnToMainMenuClicked()
    {
        m_whiteWipe.SetTrigger("WipeIn");
    }
}
