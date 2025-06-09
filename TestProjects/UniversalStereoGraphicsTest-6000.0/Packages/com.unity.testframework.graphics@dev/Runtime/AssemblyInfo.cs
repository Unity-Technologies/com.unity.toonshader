using System.Runtime.CompilerServices;
using System.ComponentModel;

[assembly: InternalsVisibleTo("UnityEditor.TestTools.Graphics")]
[assembly: InternalsVisibleTo("UnityEngine.TestTools.Graphics.Tests")]
[assembly: InternalsVisibleTo("UnityEditor.TestTools.Graphics.Tests")]
[assembly: InternalsVisibleTo("Unity.Testing.VisualEffectGraph.PerformanceRuntimeTests")]


namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit{}
}
