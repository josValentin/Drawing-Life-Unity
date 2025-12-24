using UnityEngine;
using UnityEngine.UI;

public static class ColorExtensions
{
    public static string ToHex(this Color color)
    {
        return
            ((byte)(color.r * 255)).ToString("X2") +
            ((byte)(color.g * 255)).ToString("X2") +
            ((byte)(color.b * 255)).ToString("X2") +
            ((byte)(color.a * 255)).ToString("X2");
    }

    public static void SetColorIgnoringAlpha(this SpriteRenderer spr, Color color)
    {
        color.a = spr.color.a;
        spr.color = color;
    }

    public static Color AlphaSet(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static void SetAlpha(this Image img, float alpha)
    {
        img.color = img.color.AlphaSet(alpha);
    }

    public static void SetAlpha(this SpriteRenderer spr, float alpha)
    {
        spr.color = spr.color.AlphaSet(alpha);
    }

    public static Color SetH(this Color color, float value)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return Color.HSVToRGB(value, s, v);
    }

    public static Color SetV(this Color color, float value)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return Color.HSVToRGB(h, s, value);
    }

    public static Color SetS(this Color color, float value)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return Color.HSVToRGB(h, value, v);
    }

    public static (float h, float s, float v) GetValuesHSV(this Color color, float value)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return (h, s, v);
    }
}
