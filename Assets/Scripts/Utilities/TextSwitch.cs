using UnityEngine;
using UnityEngine.UI;

public class TextSwitch : MonoBehaviour {

    [SerializeField]
    private Text m_Text1;
    [SerializeField]
    private Text m_Text2;

    public void SwitchText()
    {
        m_Text1.text = m_Text2.text;
        m_Text1.color = m_Text2.color;
    }
}
