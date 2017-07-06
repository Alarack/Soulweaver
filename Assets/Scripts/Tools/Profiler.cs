#if DEBUG
#undef DEBUG
#endif

using UnityEngine;
using System.Diagnostics;

/// <summary>
/// This class provides wrappers around Unity's profiler that will only run at debug-time
/// </summary>
public static class Profiler {
    [Conditional("DEBUG")]
    public static void BeginSample(string name, Object targetObject = null) {
        UnityEngine.Profiling.Profiler.BeginSample(name, targetObject);
    }

    [Conditional("DEBUG")]
    public static void NextSample(string name, Object targetObject = null) {
        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.BeginSample(name, targetObject);
    }


    [Conditional("DEBUG")]
    public static void EndSample() {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}