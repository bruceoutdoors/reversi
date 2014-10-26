// BGAssert class by longshot: http://forum.unity3d.com/threads/assert-class-for-debugging.59010/

using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class BGAssert
{
	[Conditional("UNITY_EDITOR")]
	static public void Assert( bool condition, string assertString )
	{
		#if UNITY_EDITOR
		if(!condition)
		{
			StackTrace myTrace = new StackTrace(true);
			StackFrame myFrame = myTrace.GetFrame(1);
			string assertInformation = "Filename: " + myFrame.GetFileName() + "\nMethod: " + myFrame.GetMethod() + "\nLine: " + myFrame.GetFileLineNumber();
			
			UnityEngine.Debug.Break();
			
			if(UnityEditor.EditorUtility.DisplayDialog("Assert!", assertString + "\n" + assertInformation,"Ok"))
			{
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(myFrame.GetFileName(),myFrame.GetFileLineNumber());
				UnityEngine.Debug.Log(assertInformation);
			}
		}
		#endif
	}
}