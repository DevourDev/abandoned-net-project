using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevourDev.Mono.SceneManagement
{
    public class DevourCustomSceneTranslation : DevourCustomSceneTranslationBase
    {
        [SerializeField] private float _destroyDelay = 5;
        [SerializeField] private ThreadPriority _loadingNextScenePriority;


        protected ThreadPriority LoadingNextScenePriority => _loadingNextScenePriority;

 
        protected override IEnumerator CustomTranslator(int buildIndex, Action callback = null)
        {
            ActionBeforeLoading?.Invoke();

            Application.backgroundLoadingPriority = LoadingNextScenePriority;
            yield return new WaitForFixedUpdate();
            var endSceneAR = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
            endSceneAR.allowSceneActivation = false;
            endSceneAR.priority = 1;
            endSceneAR.completed += (x) => ActionAfterLoading?.Invoke();
            while (true)
            {
                float progress = endSceneAR.progress;

                ActionWhileLoading?.Invoke(progress);
                if (progress > 0.85f)
                {
                    break;
                }
                yield return null;
            }
            endSceneAR.allowSceneActivation = true;
            callback?.Invoke();
            Destroy(gameObject, _destroyDelay);
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
        }

       
    }
}
