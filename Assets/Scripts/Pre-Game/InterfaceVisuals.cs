using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceVisuals : MonoBehaviour {

    // /////////
    // Instances
    // /////////

    PlayerData playerData;
    GameInfo gameInfo;

    // //////////
    // References
    // //////////

    [Header("References")]

    [SerializeField]
    private Camera m_currentCamera;

    public Transform m_startPos;
    public Transform m_targetPos;
    public Transform m_endPos;

    public GameObject m_xMarker;
    public GameObject m_oMarker;
    public GameObject m_noMarker;

    public Button m_circleButton;
    public Button m_crossButton;

    [SerializeField]
    private TweenFunction tween;

    [SerializeField]
    private Animator m_pickUIAnimator;

    [SerializeField]
    private Animator m_whiteSwipe;

    // //////////
    // Variables
    // //////////

    [Header("Variables")]

    private Color m_BackgroundColorGoTo = Color.gray;
    private Vector3 m_backgroundVel = Vector3.zero;

    public float m_BackgroundColorAnimationDamp = 1.0f;
    [Range(0.0f, 2.0f)]
    public float m_BackgroundColorBrightness = 1.0f;

    public float m_AnimationDuration = 1.0f;

    public EasingFunction.Ease m_InStyle = EasingFunction.Ease.EaseOutElastic;
    public EasingFunction.Ease m_OutStyle = EasingFunction.Ease.EaseOutExpo;

    private Transform currentIn;

    private bool b_isPanel2 = false;

    private void Start()
    {
        playerData = PlayerData.GetInstance();
        gameInfo = GameInfo.GetInstance();

        SwapVisual(m_noMarker);
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

    void SwapVisual(GameObject selectedObject)
    {

        if (currentIn != null)
        {
            tween.TweenPositionAndRotation(currentIn.transform, m_endPos.position, m_endPos.eulerAngles, m_OutStyle, m_AnimationDuration);
            StartCoroutine(DelayedDestroy(currentIn.gameObject, m_AnimationDuration));
        }

        if (b_isPanel2)
        {
            return;
        }

        if (selectedObject != null)
        {
            GameObject newObject = Instantiate(selectedObject) as GameObject;

            newObject.transform.position = m_startPos.position;
            newObject.transform.rotation = m_startPos.rotation;
            newObject.transform.localScale = new Vector3(2, 2, 2);

            currentIn = newObject.transform;

            tween.TweenPositionAndRotation(currentIn, m_targetPos.position, m_targetPos.eulerAngles, m_InStyle, m_AnimationDuration);
        }
    }

    public void OnPointerEnter(int marker)
    {
        switch (marker)
        {
            case 0: // Circle

                m_BackgroundColorGoTo = ExtendedFunctions.GetColorOfGameObject(m_oMarker);
                SwapVisual(m_oMarker);

                break;

            case 1: // Cross

                m_BackgroundColorGoTo = ExtendedFunctions.GetColorOfGameObject(m_xMarker);
                SwapVisual(m_xMarker);

                break;
        }
    }

    public void OnPointerExit()
    {
        if (currentIn == null)
            return;
        else
        {
        }

    }

    public void OnCircleClicked()
    {
        gameInfo.SetPlayerMarker(MarkerType.O);
        RemovePanel1();
    }

    public void OnCrossClicked()
    {
        gameInfo.SetPlayerMarker(MarkerType.X);
        RemovePanel1();
    }

    public void OnYouClicked()
    {
        gameInfo.SetPlayerFirst(true);
        m_whiteSwipe.SetTrigger("WipeIn");
    }

    public void OnAIClicked()
    {
        gameInfo.SetPlayerFirst(false);
        m_whiteSwipe.SetTrigger("WipeIn");
    }

    private void RemovePanel1()
    {
        m_pickUIAnimator.SetTrigger("ToPanel2");
        SwapVisual(null);
        b_isPanel2 = true;
    }

    private IEnumerator DelayedDestroy(GameObject targetObject, float t)
    {
        GameObject targetLocal = targetObject;

        yield return new WaitForSeconds(t + 0.5f);

        Destroy(targetObject);
    }

}
