using UnityEngine;

// Two buttons drawn over the framework test scene: "Next test" (the tests are cycled with a tap; Space does the same on a keyboard)
// and "Exit" (back to the game; Escape does the same). Uses the immediate-mode GUI, so it works with touch and mouse.
public class FrameworkTestOverlay : MonoBehaviour
{
    private GUIStyle m_style;

    private void OnGUI()
    {
        if (m_style == null)
        {
            m_style = new GUIStyle(GUI.skin.button);
        }
        m_style.fontSize = Mathf.Max(14, Screen.height / 28);
        float w = Screen.height * 0.36f;
        float h = Screen.height * 0.09f;
        float x = Screen.width - w - Screen.height * 0.02f;
        float y = Screen.height * 0.02f;
        string label = "Next test (" + (FrameworkTestScene.m_currentStateId + 1) + "/" + FrameworkTestScene.TestCount + ")";
        if (GUI.Button(new Rect(x, y, w, h), label, m_style))
        {
            FrameworkTestScene.NextTest();
        }
        if (GUI.Button(new Rect(x, y + h * 1.2f, w, h), "Exit", m_style))
        {
            FrameworkTestScene.Exit();
        }
    }
}
