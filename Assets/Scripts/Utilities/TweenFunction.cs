﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TweenFunction : MonoBehaviour {

    /// <summary>
    /// Tween position of selected transform from current position to end position.
    /// </summary>
    /// <param name="transform">Active transform</param>
    /// <param name="endPos">End position</param>
    /// <param name="easeStyle">Easing style</param>
    /// <param name="duration">Duration length</param>
    public void TweenPosition(Transform transform, Vector3 endPos, EasingFunction.Ease easeStyle, float duration)
    {
        StartCoroutine(PositionThread(transform, endPos, easeStyle, duration));
    }

    /// <summary>
    /// Tween rotation of selected transform from current rotation to end position.
    /// </summary>
    /// <param name="transform">Active transform</param>
    /// <param name="endRot">End rotation</param>
    /// <param name="easeStyle">Easing style</param>
    /// <param name="duration">Duration length</param>
    public void TweenRotation(Transform transform, Vector3 endRot, EasingFunction.Ease easeStyle, float duration)
    {
        StartCoroutine(RotationThread(transform, endRot, easeStyle, duration));
    }

    /// <summary>
    /// Tween position and rotation of selected transform from current transform to end transform.
    /// </summary>
    /// <param name="transform">Active transform</param>
    /// <param name="endPos">End position</param>
    /// <param name="endRot">End rotation</param>
    /// <param name="easeStyle">Easing style</param>
    /// <param name="duration">Duration length</param>
    public void TweenPositionAndRotation(Transform transform, Vector3 endPos, Vector3 endRot, EasingFunction.Ease easeStyle, float duration)
    {
        StartCoroutine(PositionRotationThread(transform, endPos, endRot, easeStyle, duration));
    }

    // ///////
    // Threads
    // ///////

    private IEnumerator PositionThread(Transform transform, Vector3 endPos, EasingFunction.Ease easeStyle, float duration)
    {
        // Local Info
        Transform currentTransform = transform;
        Vector3 currentEndPos = endPos;
        EasingFunction.Ease currentEaseStyle = easeStyle;
        float currentDuration = duration;

        Vector3 currentStartPos = transform.position;

        for (float t = 0; t < currentDuration; t += Time.deltaTime)
        {
            Position(currentTransform, currentStartPos, currentEndPos, currentEaseStyle, currentDuration, t);
            yield return new WaitForEndOfFrame();
        }

        Position(currentTransform, currentStartPos, currentEndPos, currentEaseStyle, currentDuration, currentDuration);
    }

    private IEnumerator RotationThread(Transform transform, Vector3 endRot, EasingFunction.Ease easeStyle, float duration)
    {
        // Local Info
        Transform currentTransform = transform;
        Vector3 currentEndRot = endRot;
        EasingFunction.Ease currentEaseStyle = easeStyle;
        float currentDuration = duration;

        Vector3 currentStartRot = transform.eulerAngles;

        for (float t = 0; t < currentDuration; t += Time.deltaTime)
        {
            Rotation(currentTransform, currentStartRot, currentEndRot, currentEaseStyle, currentDuration, t);
            yield return new WaitForEndOfFrame();
        }

        Rotation(currentTransform, currentStartRot, currentEndRot, currentEaseStyle, currentDuration, currentDuration);
    }

    private IEnumerator PositionRotationThread(Transform transform, Vector3 endPos, Vector3 endRot, EasingFunction.Ease easeStyle, float duration)
    {
        // Local Info
        Transform currentTransform = transform;
        Vector3 currentEndPos = endPos;
        Vector3 currentEndRot = endRot;
        EasingFunction.Ease currentEaseStyle = easeStyle;
        float currentDuration = duration;

        Vector3 currentStartPos = transform.position;
        Vector3 currentStartRot = transform.eulerAngles;

        for (float t = 0; t < currentDuration; t += Time.deltaTime)
        {
            Position(currentTransform, currentStartPos, currentEndPos, currentEaseStyle, currentDuration, t);
            Rotation(currentTransform, currentStartRot, currentEndRot, currentEaseStyle, currentDuration, t);
            yield return new WaitForEndOfFrame();
        }

        Position(currentTransform, currentStartPos, currentEndPos, currentEaseStyle, currentDuration, currentDuration);
        Rotation(currentTransform, currentStartRot, currentEndRot, currentEaseStyle, currentDuration, currentDuration);
    }

    // //////
    // Helper
    // //////

    private void Position(Transform t, Vector3 startPos, Vector3 endPos, EasingFunction.Ease easeStyle, float duration, float time)
    {
        float p = EasingFunction.GetEasingFunction(easeStyle)(0.0f, duration, time / duration) / duration;

        t.position = Vector3.LerpUnclamped(startPos, endPos, p);
    }

    private void Rotation(Transform t, Vector3 startRot, Vector3 endRot, EasingFunction.Ease easeStyle, float duration, float time)
    {
        float p = EasingFunction.GetEasingFunction(easeStyle)(0.0f, duration, time / duration) / duration;

        t.eulerAngles = Vector3.LerpUnclamped(startRot, endRot, p);
    }

    private void Rotation(Transform t, Quaternion startRot, Quaternion endRot, EasingFunction.Ease easeStyle, float duration, float time)
    {
        float p = EasingFunction.GetEasingFunction(easeStyle)(0.0f, duration, time / duration) / duration;

        t.rotation = Quaternion.LerpUnclamped(startRot, endRot, p);
    }

    private void Scale(Transform t, Vector3 startScale, Vector3 endScale, EasingFunction.Ease easeStyle, float duration, float time)
    {
        float p = EasingFunction.GetEasingFunction(easeStyle)(0.0f, duration, time / duration) / duration;

        t.localScale = Vector3.LerpUnclamped(startScale, endScale, p);
    }

    private void Scale(Transform t, float startScale, float endScale, EasingFunction.Ease easeStyle, float duration, float time)
    {
        //TODO: Make scalable tween using one float value
    }
}
