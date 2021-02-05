using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DrimiaInteractive.RtlHelperSystem.EditorUtilities
{
	public class MonobehaviourOnBuildBroadcastMessage : IProcessSceneWithReport
	{
		public int callbackOrder
		{
			get { return 0; }
		}

		public void OnProcessScene(Scene scene, BuildReport report)
		{
			var rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject go in rootGameObjects)
			{
				if (go)
				{
					go.gameObject.BroadcastMessage("OnBuild", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}