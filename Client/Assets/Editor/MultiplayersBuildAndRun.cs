using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
	[MenuItem("Tools/Run Multiplayer/1 Players")]
	static void PerformWin64Build1()
	{
		PerformWin64Build(1, false);
	}

	[MenuItem("Tools/Run Multiplayer/2 Players")]
	static void PerformWin64Build2()
	{
		PerformWin64Build(2);
	}

	[MenuItem("Tools/Run Multiplayer/3 Players")]
	static void PerformWin64Build3()
	{
		PerformWin64Build(3);
	}

	[MenuItem("Tools/Run Multiplayer/4 Players")]
	static void PerformWin64Build4()
	{
		PerformWin64Build(4);
	}

	static void PerformWin64Build(int playerCount, bool play = true)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
			BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

		for (int i = 1; i <= playerCount; i++)
		{
			BuildOptions options = play ? BuildOptions.AutoRunPlayer : BuildOptions.None;

			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Win64/" + GetProjectName() + "/" + GetProjectName() + ".exe",
				BuildTarget.StandaloneWindows64, options);
		}
	}

	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		//return s[s.Length - 2];
		return "GenshinImpact";
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
