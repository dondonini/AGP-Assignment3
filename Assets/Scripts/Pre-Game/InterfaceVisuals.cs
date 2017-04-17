using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceVisuals : MonoBehaviour {

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

    // //////////
    // Variables
    // //////////

    [Header("Variables")]

    private Color m_BackgroundColorGoTo = Color.gray;
    private Vector3 m_backgroundVel = Vector3.zero;

    public float m_BackgroundColorAnimationDamp = 1.0f;
    [Range(0.0f, 2.0f)]
    public float m_BackgroundColorBrightness = 1.0f;

    public EasingFunction.Ease m_InStyle = EasingFunction.Ease.EaseOutElastic;
    public EasingFunction.Ease m_OutStyle = EasingFunction.Ease.EaseOutExpo;

    private Transform currentIn;
    private Transform currentOut;

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

    public void OnPointerEnter(int marker)
    {
        switch (marker)
        {
            case 0: // Circle

                break;

            case 1: // Cross

                break;
        }
    }

    public void OnPointerExit()
    {
        if (currentIn == null)
            return;
        else
        {
            currentOut = currentIn;
        }

    }


}
