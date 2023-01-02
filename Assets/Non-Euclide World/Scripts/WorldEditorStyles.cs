using UnityEngine;

public static class WorldEditorStyles
{
    public static GUIStyle HelpBoxStyle
    {
        get
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.fixedHeight = 0;
            return style;
        }
    }

    public static GUIStyle MainLayerLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle("Button");
            style.fixedHeight = 0;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.black;
            style.normal.background = CreatePixelOneTexture(new Color(75 / 255f, 240 / 255f, 159 / 255f));

            return style;
        }
    }

    private static Texture2D CreatePixelOneTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(1, 1, color);
        texture.Apply();

        return texture;
    }
}