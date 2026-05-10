using UnityEngine;

namespace MalumMenu;

public class AboutTab : ITab
{
    public string name => "About";

    private static readonly Color ButtonNormal = new Color(0.176f, 0.306f, 0.702f);
    private static readonly Color ButtonHover  = new Color(0.176f, 0.518f, 0.702f);

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawText();
        GUILayout.Space(15);
        DrawButtons();

        GUILayout.EndVertical();
    }

    private void DrawText()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            richText = true,
            wordWrap = true,
            fontSize = 14
        };

        GUILayout.Label("<color=#34ebab><b>Credits</b></color>", style);

        GUILayout.Space(5);

        GUILayout.Label(
            "<color=#eb5934>F1xx00rs -- Developer</color>\n\n" +
            "<color=#eb8034>K3razz -- Contributor</color>\n" +
            "<color=#eb8034>ZHKglaz -- Contributor</color>\n",
            style
        );
    }

    private void DrawButtons()
    {
        GUILayout.Label(
            "<color=#FFFFFF><b>Links</b></color>",
            new GUIStyle(GUI.skin.label) { richText = true }
        );

        Color oldColor = GUI.backgroundColor;

        DrawButton("Discord", "https://discord.gg/cFXmk78w5X");
        DrawButton("GitHub", "https://github.com/f1xx00rs/MalumMenu-Unofficial");

        GUILayout.Label(
            "<color=#FFFFFF><b>Developers</b></color>",
            new GUIStyle(GUI.skin.label) { richText = true }
        );

        DrawButton("f1xx00rs", "https://github.com/f1xx00rs");
        DrawButton("K3razz", "https://github.com/K3razz");
        DrawButton("ZHKglaz", "https://github.com/ZHKglaz");

        GUI.backgroundColor = oldColor;
    }

    private void DrawButton(string text, string url)
    {
        Rect rect = GUILayoutUtility.GetRect(0, 30, GUILayout.ExpandWidth(true));

        bool hover = rect.Contains(Event.current.mousePosition);

        GUI.backgroundColor = hover ? ButtonHover : ButtonNormal;

        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13
        };

        style.normal.textColor = Color.white;
        style.hover.textColor = Color.white;

        if (GUI.Button(rect, text, style))
        {
            Application.OpenURL(url);
        }
    }
}