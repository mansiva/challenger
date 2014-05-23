using System;
using UnityEngine;

/// <summary>
/// This class is used to assign a variable depending on the resulting validation
/// applied to its pretended new value. This is to be used in properties to avoid
/// code duplication.
/// </summary>
public static class CoreValidator
{
    /// <summary>
    /// Clamps newValue between min and max and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Clamp(ref int curValue, int newValue, int min, int max)
    {
        if(curValue != newValue)
        {
            int clamped = Mathf.Clamp(newValue, min, max);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Clamps newValue between min and max and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Clamp(ref float curValue, float newValue, float min, float max)
    {
        if(curValue != newValue)
        {
            float clamped = Mathf.Clamp(newValue, min, max);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates the highest number between newValue and floor and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Floor(ref int curValue, int newValue, int floor)
    {
        if(curValue != newValue)
        {
            int clamped = Mathf.Max(newValue, floor);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates the highest number between newValue and floor and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Floor(ref float curValue, float newValue, float floor)
    {
        if(curValue != newValue)
        {
            float clamped = Mathf.Max(newValue, floor);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates the lowest number between newValue and ceil and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Ceil(ref int curValue, int newValue, int ceil)
    {
        if(curValue != newValue)
        {
            int clamped = Mathf.Min(newValue, ceil);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates the lowest number between newValue and ceil and assigns it to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Ceil(ref float curValue, float newValue, float ceil)
    {
        if(curValue != newValue)
        {
            float clamped = Mathf.Min(newValue, ceil);
            if(curValue != clamped)
            {
                curValue = clamped;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Assigns newValue to curValue if it is different.
    /// </summary>
    /// <returns><c>true</c> if curValue has changed, <c>false</c> otherwise.</returns>
    public static bool Different<T>(ref T curValue, T newValue)
    {
        if(!object.Equals(curValue, newValue))
        {
            curValue = newValue;
            return true;
        }
        return false;
    }
}
