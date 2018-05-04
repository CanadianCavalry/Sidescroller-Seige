﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes {

	private static Dictionary<string, string> parameters;

	public static void Load(string sceneName, Dictionary<string, string> parameters = null) {
		Scenes.parameters = parameters;
		SceneManager.LoadScene(sceneName);
	}

	public static void Load(string sceneName, string paramKey, string paramValue) {
		if (parameters == null) {
			Scenes.parameters = new Dictionary<string, string> ();
		}

		setParam(paramKey, paramValue);
		SceneManager.LoadScene(sceneName);
	}

	public static Dictionary<string, string> getSceneParameters() {
		return parameters;
	}

	public static string getParam(string paramKey) {
		if (parameters == null) return "";
		return parameters[paramKey];
	}

	public static void setParam(string paramKey, string paramValue) {
		if (parameters == null) {
			Scenes.parameters = new Dictionary<string, string> ();
		}

		if (Scenes.parameters.ContainsKey (paramKey)) {
			Scenes.parameters.Remove(paramKey);
		} 

		Scenes.parameters.Add(paramKey, paramValue);
	}
}
