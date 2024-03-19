using AutoSharp;
using AutoSharp.Core;

using System;
using System.Collections.Generic;

/// <summary>
/// Thw <see cref="Coroutine"/> controller.
/// </summary>
internal class CoroutineController
{
    private readonly List<Coroutine> coroutines = new List<Coroutine>();

    /// <summary>
    /// Append <paramref name="flow"/> to <see cref="Coroutine"/> with <paramref name="coroutineName"/>.<para />
    /// This will start a new <see cref="Coroutine"/> with <paramref name="coroutineName"/> if <see cref="CoroutineController"/>
    /// doesn't contain any <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
    /// </summary>
    /// <param name="coroutineName">The name of <see cref="Coroutine"/>.</param>
    /// <param name="flow">The <see cref="Flow"/> to append.</param>
    /// <returns>The <see cref="Coroutine"/> instance append the <paramref name="flow"/>.</returns>
    /// <exception cref="ArgumentNullException">Argument <paramref name="coroutineName"/> can not be null.</exception>
    public Coroutine AppendCoroutine(string coroutineName, Flow flow)
    {
        if (coroutineName == null)
        {
            throw new ArgumentNullException($"Argument {nameof(coroutineName)} can not be null.");
        }
        var coroutine = coroutines.Find(it => it.name == coroutineName);
        if (coroutine != null)
        {
            coroutine.AddFLow(flow);
            return coroutine;
        }
        else
        {
            return CreateCoroutine(coroutineName, flow);
        }
    }

    /// <summary>
    /// Append <paramref name="flow"/> to <paramref name="coroutine"/> with <paramref name="flow"/>.<para />
    /// This will start a new <see cref="Coroutine"/> with <paramref name="coroutine"/>'s name if <paramref name="coroutine"/>
    /// doesn't exists in the <see cref="CoroutineController"/>.
    /// </summary>
    /// <param name="coroutine">The target <see cref="Coroutine"/>.</param>
    /// <param name="flow">The <see cref="Flow"/> to append.</param>
    /// <returns>The <see cref="Coroutine"/> instance append <paramref name="flow"/>.</returns>
    /// <exception cref="ArgumentNullException">Argument <paramref name="coroutine"/> can not be null.</exception>
    public Coroutine AppendCoroutine(Coroutine coroutine, Flow flow)
    {
        if (coroutine == null)
        {
            throw new ArgumentNullException($"Argument {nameof(coroutine)} can not be null.");
        }
        if (coroutines.Contains(coroutine))
        {
            coroutine.AddFLow(flow);
            return coroutine;
        }
        else
        {
            return CreateCoroutine(coroutine.name, flow);
        }
    }

    /// <summary>
    /// Get the first <see cref="Coroutine"/> by <paramref name="coroutineName"/>.
    /// </summary>
    /// <param name="coroutineName">The name of <see cref="Coroutine"/>.</param>
    /// <returns>The first <see cref="Coroutine"/> instance with <paramref name="coroutineName"/>.</returns>
    /// <exception cref="ArgumentNullException">Argument <paramref name="coroutineName"/> can not be null.</exception>
    public Coroutine GetCoroutine(string coroutineName)
    {
        if (coroutineName == null)
        {
            throw new ArgumentNullException($"Argument {nameof(coroutineName)} can not be null.");
        }
        return coroutines.Find(it => it.name == coroutineName);
    }

    /// <summary>
    /// Get <see cref="Coroutine"/> by <paramref name="coroutineName"/>.
    /// </summary>
    /// <param name="coroutineName">The name of <see cref="Coroutine"/>.</param>
    /// <returns>The <see cref="Coroutine"/> instances with <paramref name="coroutineName"/>.</returns>
    /// <exception cref="ArgumentNullException">Argument <paramref name="coroutineName"/> can not be null.</exception>
    public Coroutine[] GetAllCoroutines(string coroutineName)
    {
        if (coroutineName == null)
        {
            throw new ArgumentNullException($"Argument {nameof(coroutineName)} can not be null.");
        }
        return coroutines.FindAll(it => it.name == coroutineName).ToArray();
    }

    /// <summary>
    /// Stop <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
    /// </summary>
    /// <param name="coroutineName">The name of target <see cref="Coroutine"/> to stop.</param>
    /// <returns>
    /// <see langword="true"/> if target <see cref="Coroutine"/> is successfully stopped; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if target <see cref="Coroutine"/> was not found in the <see cref="CoroutineController"/>.
    /// </returns>
    public bool StopCoroutine(string coroutineName)
    {
        var coroutine = GetCoroutine(coroutineName);
        return StopCoroutine(coroutine);
    }

    /// <summary>
    /// Stop <paramref name="coroutine"/>.
    /// </summary>
    /// <param name="coroutine">The target <see cref="Coroutine"/> to stop.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="coroutine"/> is successfully stopped; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if <paramref name="coroutine"/> was not found in the <see cref="CoroutineController"/>.
    /// </returns>
    public bool StopCoroutine(Coroutine coroutine)
    {
        if (coroutines.Remove(coroutine))
        {
            coroutine.Stop();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Stop all <see cref="Coroutine"/> with <paramref name="coroutineName"/>.
    /// </summary>
    /// <param name="coroutineName"></param>
    /// <returns>The number of <see cref="Coroutine"/> stopped from the <see cref="CoroutineController"/>.</returns>
    public int StopAllCoroutines(string coroutineName)
    {
        var cnt = 0;
        foreach (var coroutine in coroutines.FindAll(it => it.name == coroutineName))
        {
            StopCoroutine(coroutine);
            cnt++;
        }
        return cnt;
    }

    /// <summary>
    /// Stop all <see cref="Coroutine"/>.
    /// </summary>
    /// <returns>The number of <see cref="Coroutine"/> stopped from the <see cref="CoroutineController"/>.</returns>
    public int StopAllCoroutines()
    {
        foreach (var coroutine in coroutines)
        {
            coroutine.Stop();
        }
        return coroutines.Count;
        // the stopped coroutine wil be clear in next tick, so we don't need to clear the coroutines here.
    }

    /// <summary>
    /// Start a new anonymous <see cref="Coroutine"/> with <paramref name="flow"/>.
    /// </summary>
    /// <param name="flow">The <see cref="Flow"/> of <see cref="Coroutine"/>.</param>
    /// <returns>The created <see cref="Coroutine"/> instance.</returns>
    public Coroutine StartCoroutine(Flow flow)
    {
        return CreateCoroutine(null, flow);
    }

    internal void Tick()
    {
        // Tick all coroutines in parallel.
        for (var i = 0; i < coroutines.Count; i++)
        {
            var coroutine = coroutines[i];
            if (coroutine.Tick()) // The coroutine was finished.
            {
                coroutines.RemoveAt(i);
                i--; // fix element offset
            }
        }
    }

    private Coroutine CreateCoroutine(string coroutineName, Flow flow)
    {
        var coroutine = new Coroutine(coroutineName);
        coroutine.AddFLow(flow);
        //
        coroutines.Add(coroutine);
        return coroutine;
    }
}
