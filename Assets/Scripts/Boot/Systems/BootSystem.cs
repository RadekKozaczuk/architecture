using System.Linq;
using System.Threading.Tasks;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boot.Systems
{
    static class BootSystem
    {
        internal static async Task LoadScenes(params int[] scenes)
        {
            var asyncOperations = new AsyncOperation[scenes.Length];
            asyncOperations[0] = SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Additive);

            for (int i = 1 ; i < scenes.Length ; i++)
                asyncOperations[i] = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);

            await AwaitAsyncOperations(asyncOperations);

            // wait a frame so every Awake and Start method is called
            await Utils.WaitForNextFrame();

            // Radek: this hack is necessary because for some reason without waiting for another frame
            // GameObject.FindWithTag("MyTag") will not found the object
            await Utils.WaitForNextFrame();
        }

        internal static async Task UnloadScenes(params int[] scenes)
        {
            int operationCount = scenes.Length;
            var asyncOperations = new AsyncOperation[operationCount];

            for (int i = 0 ; i < scenes.Length ; i++)
                asyncOperations[i] = SceneManager.UnloadSceneAsync(scenes[i]);

            await AwaitAsyncOperations(asyncOperations);

            // wait a frame so every Awake and Start method is called
            await Utils.WaitForNextFrame();

            // Radek: this hack is necessary because for some reason without waiting for another frame
            // GameObject.FindWithTag("MyTag") will not found the object
            await Utils.WaitForNextFrame();
        }

        /// <summary>
        /// Waits until all operations are either done or have progress greater equal 0.9.
        /// </summary>
        static async Task AwaitAsyncOperations(params AsyncOperation[] operations)
        {
            while (operations.All(t => t.isDone))
                await Task.Delay(1);
        }
    }
}