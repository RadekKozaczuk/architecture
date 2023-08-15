using System.Collections;
using System.Collections.Generic;
using Common.Config;
using Common.Enums;
using Common.Systems;
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    class LevelSceneReferenceHolder : MonoBehaviour
    {
        // todo: add assertion that checks if spawn points are not duplicated

        internal Transform GetSpawnPoint(PlayerId playerId) => _spawnPoints[(int)playerId].transform;

        [SerializeField]
        SpawnPointView[] _spawnPoints;

        [SerializeField]
        internal List<Collider2D> Colliders;

        [SerializeField]
        internal Transform LightContainer;

        [SerializeField]
        internal Transform EnemySpawnPointContainer;

        [SerializeField]
        internal Transform EnemyContainer;

		static readonly SceneConfig _config;

		void Awake() {
			if (IsSceneOnCustomActivationList(out float duration, out SceneConfig.ActivationMode mode)) {
				GameObject[] objects = gameObject.scene.GetRootGameObjects();

				if (mode == SceneConfig.ActivationMode.OverTime) {
					GameStateSystem.ActivateRoots_OverTime(ActivateRoots_OverTime(objects, duration));
					return;
				}

				if (mode == SceneConfig.ActivationMode.StateChange)
					GameStateSystem.ActivateRoots_StateChange(ActivateRoots_StateChange);
			}
		}

		internal void CopyLightReferences(List<Light> list)
        {
            list.Clear();

            for (int i = 0; i < LightContainer.childCount; i++)
                list.Add(LightContainer.GetChild(i).GetComponent<Light>());
        }

		/// <summary>
		/// Instantly activates all root objects (except reference holder itself).
		/// </summary>
		void ActivateRoots_StateChange() {
			GameObject[] roots = gameObject.scene.GetRootGameObjects();

			// first is the level reference holder itself so we can ignore it
			for (int i = 1; i < roots.Length; i++)
				roots[i].SetActive(true);
		}

		/// <summary>
		/// Activates all root objects (except reference holder itself) over a given number of frames.
		/// </summary>
		static IEnumerator ActivateRoots_OverTime(IReadOnlyList<GameObject> roots, float duration) {
			int frameLimit = (int)(Application.targetFrameRate * duration);

			int rootsPerFrame = frameLimit / roots.Count + 1;
			int left = roots.Count - 1;

			// todo: add code validator that checks if this condition is true on every scene
			// todo: if scene has a reference holder then it should be active and first in hierarchy
			int i = 1; // first is the level reference holder itself so we can ignore it
			while (left > 0)
				for (int j = 0; j < (left < rootsPerFrame ? left : rootsPerFrame); j++) {
					roots[i++].SetActive(true);
					left--;
					yield return new WaitForEndOfFrame();
				}
		}

		/// <summary>
		/// If at least one scene is present on the <see cref="Common.Config.SceneConfig"/>'s ExtendedLoadingScenes
		/// then ALL scenes are will be loaded with lower priority.
		/// </summary>
		bool IsSceneOnCustomActivationList(out float duration, out SceneConfig.ActivationMode mode) {
			foreach (SceneConfig.ExtActivation activation in _config.CustomActivation)
				if (gameObject.scene.buildIndex == (int)activation.Level) {
					duration = activation.ActivationTime;
					mode = activation.When;
					return true;
				}

			duration = default;
			mode = default;
			return false;
		}
	}
}