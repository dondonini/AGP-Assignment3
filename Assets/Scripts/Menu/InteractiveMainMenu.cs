using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveMainMenu : MonoBehaviour {

    // /////////
    // Instances
    // /////////

    private PlayerData playerData;

    // ///////////////////
    [Header("References")]
    // ///////////////////

    [SerializeField]
    private Text m_winStats;

    [SerializeField]
    private Text m_lossStats;

    [SerializeField]
    private SceneChange m_sceneChange;

    [SerializeField]
    private Animator m_menuNavAnimator;

    [SerializeField]
    private Animator m_wipeSwipe;

    [SerializeField]
    private Animator m_savesInfoAnimator;

    [SerializeField]
    private Camera m_currentCamera;

    // //////////////////
    [Header("Variables")]
    // //////////////////

    private Color m_BackgroundColorGoTo = Color.gray;
    private Vector3 m_backgroundVel = Vector3.zero;

    public float m_BackgroundColorAnimationDamp = 1.0f;
    [Range(0.0f, 2.0f)]
    public float m_BackgroundColorBrightness = 1.0f;

    private void Start()
    {
        playerData = PlayerData.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
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

    public void OnPlayClicked()
    {
        m_wipeSwipe.SetTrigger("WipeIn");
        m_sceneChange.SetSceneByNameDelayed("PreGame", 1.0f);
    }

    public void OnLoadSaveMenuClicked()
    {
        m_winStats.text = playerData.GetSavedData1().m_TotalWins.ToString();
        m_lossStats.text = playerData.GetSavedData1().m_TotalLosses.ToString();

        m_menuNavAnimator.SetTrigger("ShowLS");
        m_savesInfoAnimator.SetTrigger("ShowSaves");
    }

    public void OnQuitMenuClicked()
    {
        m_menuNavAnimator.SetTrigger("ShowQuit");
    }

    public void OnQuitToMainClicked()
    {
        m_menuNavAnimator.SetTrigger("HideQuit");
    }

    public void OnLoadSaveToMainClicked()
    {
        m_menuNavAnimator.SetTrigger("HideLS");
        m_savesInfoAnimator.SetTrigger("HideSaves");
    }

    public void OnLoadClicked()
    {
        if (playerData.LoadSave(playerData.GetSavedData1()))
        {
            StartCoroutine(SuccessIndicator());
        }
        else
        {
            StartCoroutine(UnsuccessIndicator());
        }
    }

    public void OnSaveClicked()
    {
        if (playerData.SaveSave(0))
        {
            StartCoroutine(SuccessIndicator());
        }
        else
        {
            StartCoroutine(UnsuccessIndicator());
        }
    }

    IEnumerator SuccessIndicator()
    {
        Color previousColor = m_BackgroundColorGoTo;
        float duration = 1.0f;

        m_BackgroundColorGoTo = Color.green;

        yield return new WaitForSeconds(duration);

        m_BackgroundColorGoTo = previousColor;
    }

    IEnumerator UnsuccessIndicator()
    {
        Color previousColor = m_BackgroundColorGoTo;
        float duration = 1.0f;

        m_BackgroundColorGoTo = Color.red;

        yield return new WaitForSeconds(duration);

        m_BackgroundColorGoTo = previousColor;
    }
}
