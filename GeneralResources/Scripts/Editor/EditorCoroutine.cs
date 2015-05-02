using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace Serif
{
    //Just a simple way to spread execution accross multiple "frames" within the editor (e.g. printing to console between steps in an editor script)
    public static class EditorCoroutine
    {
        static List<IEnumerator> currentCoroutines = new List<IEnumerator>();
        static bool isAttached = false;

        public static void StartCoroutine(IEnumerator coroutine)
        {
#if UNITY_EDITOR
            CheckAttachment();
            currentCoroutines.Add(coroutine);
#endif
        }

        private static void CheckAttachment()
        {
            if (!isAttached)
            {
                isAttached = true;
                EditorApplication.update += RunEditorFrame;
            }
        }

        private static void RunEditorFrame()
        {
            for (int i = 0; i < currentCoroutines.Count; i++)
            {
                IEnumerator coroutine = currentCoroutines[i];

                if (!coroutine.MoveNext())
                    currentCoroutines.RemoveAt(i--);
            }
        }
    }
}
