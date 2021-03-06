using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
	[MenuItem("Tools/Run Multiplayer/2 Players")]
	static void PerformMac64Build2()
	{
		PerformMac64Build(2);
	}

	[MenuItem("Tools/Run Multiplayer/3 Players")]
	static void PerformMac64Build3()
	{
		PerformMac64Build(3);
	}

	[MenuItem("Tools/Run Multiplayer/4 Players")]
	static void PerformMac64Build4()
	{
		PerformMac64Build(4);
	}

	static void PerformMac64Build(int playerCount)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
			BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
		

		for (int i = 1; i <= playerCount; i++)
		{
			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Mac64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".app",
				BuildTarget.StandaloneOSX, BuildOptions.AutoRunPlayer);
		}
	}

	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
	}
}
