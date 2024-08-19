using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static string ReplaceFirst(this string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    public static void ClearChildren(this Transform o)
    {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject.Destroy(o.GetChild(i).gameObject);
        }
    }

    public static bool Similar(this Quaternion quatA, Quaternion value, float acceptableRange)
    {
        return 1 - Mathf.Abs(Quaternion.Dot(quatA, value)) < acceptableRange;
    }

    public static Color Hue(this Color color, float amount)
    {
        float s, v;
        Color.RGBToHSV(color, out _, out s, out v);
        color = Color.HSVToRGB(amount, s, v);
        return color;
    }

    public static Color Saturation(this Color color, float amount)
    {
        float h, v;
        Color.RGBToHSV(color, out h, out _, out v);
        color = Color.HSVToRGB(h, amount, v);
        return color;
    }

    public static Color Value(this Color color, float amount)
    {
        float h, s;
        Color.RGBToHSV(color, out h, out s, out _);
        color = Color.HSVToRGB(h, s, amount);
        return color;
    }

    public static string ToTimeString(this double time)
    {
        if (time < 0) return "--:--.---";
        int hours = (int)(time / 60 / 60);
        return hours > 0 ? string.Format("{0}:{1,2:D2}:{2,2:D2}.{3,3:D3}", hours, (int)(time / 60 % 60), (int)(time % 60), (int)(time * 1000 % 1000))
            : string.Format("{0}:{1,2:D2}.{2,3:D3}", (int)(time / 60 % 60), (int)(time % 60), (int)(time * 1000 % 1000));
    }

    public static string ToTimeString(this float time)
    {
        if (time < 0) return "--:--.---";
        int hours = (int)(time / 60 / 60);
        return hours > 0 ? string.Format("{0}:{1,2:D2}:{2,2:D2}.{3,3:D3}", hours, (int)(time / 60 % 60), (int)(time % 60), (int)(time * 1000 % 1000))
            : string.Format("{0}:{1,2:D2}.{2,3:D3}", (int)(time / 60 % 60), (int)(time % 60), (int)(time * 1000 % 1000));
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static Rect GetWorldBorder(this Camera camera, float z)
    {
        float depth = z - camera.transform.position.z;
        var upperRightScreen = new Vector3(Screen.width, Screen.height, depth);
        var lowerLeftScreen = new Vector3(0, 0, depth);

        //Corner locations in world coordinates
        var upperRight = camera.ScreenToWorldPoint(upperRightScreen);
        var lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);

        return new Rect(lowerLeft, upperRight - lowerLeft);
    }
}