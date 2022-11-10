using System.Collections;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class CoroutineService : MonoBehaviour
    {
        private static IEnumerator WaitForFramesCoroutine(int framesToWait = 1)
        {
            for (var i = 0; i < framesToWait; i++)
                yield return null;
        }
    }
}