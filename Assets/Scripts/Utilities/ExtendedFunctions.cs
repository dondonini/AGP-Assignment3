using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom function just for me (Edisson Flores).
/// </summary>
public class ExtendedFunctions {

    /// <summary>
    /// Smooth Damp function for Color
    /// </summary>
    /// <param name="current">The current color.</param>
    /// <param name="target">The color we are trying to reach.</param>
    /// <param name="velocity">The current velocity, this value is modified by the function every time you call it.</param>
    /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
    /// <returns></returns>
    public static Color ColorSmoothDamp(Color current, Color target, ref Vector3 velocity, float smoothTime)
    {
        Vector3 c = ColorToVector3(current);
        Vector3 t = ColorToVector3(target);

        Vector3 temp = Vector3.SmoothDamp(c, t, ref velocity, smoothTime);

        return Vector3ToColor(temp);
    }

    /// <summary>
    /// Convert Color to Vector3
    /// </summary>
    /// <param name="newColor">Color</param>
    /// <returns>Vector3 conversion</returns>
    public static Vector3 ColorToVector3(Color newColor)
    {
        return new Vector3(newColor.r, newColor.g, newColor.b);
    }

    /// <summary>
    /// Convert Vector3 to Color
    /// </summary>
    /// <param name="newVector3">Vector3</param>
    /// <returns>Color conversion</returns>
    public static Color Vector3ToColor(Vector3 newVector3)
    {
        return new Color(newVector3.x, newVector3.y, newVector3.z);
    }

    /// <summary>
    /// Convert 8-bit variable to a float
    /// </summary>
    /// <param name="new8Bit">8-bit variable</param>
    /// <returns>Float conversion</returns>
    public static float Convert8ToFloat(float new8Bit)
    {
        return new8Bit / 255.0f;
    }

    /// <summary>
    /// Collects the color of the gameobject
    /// </summary>
    /// <param name="newObject">The game object.</param>
    /// <returns>Extracted color from object.</returns>
    public static Color GetColorOfGameObject(GameObject newObject)
    {
        return newObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
    }
}
