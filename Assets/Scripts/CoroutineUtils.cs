using System;
using System.Collections;
using UnityEngine;

public static class CorutineHelper
{
    // Functions ---------
    public static void ActionAfterTime(this MonoBehaviour obj, float time, Action action)
    {
        obj.StartCoroutine(I_ActionAfterTime(time, action));
    }
    public static void ActionAfterFrames(this MonoBehaviour obj, Action action, int frames = 1)
    {
        obj.StartCoroutine(I_ActionAfterFrames(action, frames));
    }
    public static void ActionWaitUntilCondition(this MonoBehaviour obj, Func<bool> condition, Action action)
    {
        obj.StartCoroutine(I_ActionWaitUntilCondition(condition, action));
    }
    public static void ActionWaitUntilConditionStep(this MonoBehaviour obj, Func<bool> condition, float stepTime, Action action)
    {
        obj.StartCoroutine(I_ActionWaitUntilConditionStep(condition, stepTime, action));
    }
    public static void ActionAfterReturnedNull(this MonoBehaviour obj, Action action)
    {
        obj.StartCoroutine(I_ActionAfterReturnedNull(action));
    }
    public static void ActionOnEndOfFrame(this MonoBehaviour obj, Action action)
    {
        obj.StartCoroutine(I_ActionOnEndOfFrame(action));
    }

    // Coroutines ---------
    private static IEnumerator I_ActionAfterTime(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    private static IEnumerator I_ActionAfterFrames(Action action, int frames = 1)
    {
        if (frames != 0)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        action();
    }

    private static IEnumerator I_ActionWaitUntilCondition(Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);

        action();
    }

    private static IEnumerator I_ActionWaitUntilConditionStep(Func<bool> condition, float stepTime, Action action)
    {
        while (!condition())
        {
            yield return new WaitForSeconds(stepTime);
        }

        action();
    }

    private static IEnumerator I_ActionAfterReturnedNull(Action action)
    {
        yield return null;
        action();
    }

    private static IEnumerator I_ActionOnEndOfFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }
}


