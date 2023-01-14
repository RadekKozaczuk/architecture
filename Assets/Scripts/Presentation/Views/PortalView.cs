using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PortalView : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (!col.TryGetComponent(out PlayerView _))
                return;

            if (CommonData.CurrentLevel.HasValue)
            {
                SceneManager.UnloadSceneAsync(CommonData.CurrentLevel.Value);
                SceneManager.LoadSceneAsync(CommonData.CurrentLevel.Value + 1, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.UnloadSceneAsync(Constants.HubScene);
                SceneManager.LoadSceneAsync(Constants.Level0Scene, LoadSceneMode.Additive);
            }
        }
    }
}