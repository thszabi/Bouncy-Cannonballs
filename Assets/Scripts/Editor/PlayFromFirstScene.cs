#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class PlayFromStartingSceneUtil
    {
        private const string PLAY_FROM_FIRST_MENU_STR = "Edit/Always Start From Scene 0 &p";

        private static bool PlayFromFirstScene
        {
            get => EditorPrefs.HasKey(PLAY_FROM_FIRST_MENU_STR) && EditorPrefs.GetBool(PLAY_FROM_FIRST_MENU_STR);
            set => EditorPrefs.SetBool(PLAY_FROM_FIRST_MENU_STR, value);
        }
 
        [MenuItem(PLAY_FROM_FIRST_MENU_STR, false, 150)]
        private static void PlayFromFirstSceneCheckMenu() 
        {
            PlayFromFirstScene = !PlayFromFirstScene;
            Menu.SetChecked(PLAY_FROM_FIRST_MENU_STR, PlayFromFirstScene);
 
            ShowNotifyOrLog(PlayFromFirstScene ? "Play from scene 0" : "Play from current scene");
        }
 
        // The menu won't be gray out, we use this validate method for update check state
        [MenuItem(PLAY_FROM_FIRST_MENU_STR, true)]
        private static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(PLAY_FROM_FIRST_MENU_STR, PlayFromFirstScene);
            return true;
        }
 
        // This method is called before any Awake. It's the perfect callback for this feature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadFirstSceneAtGameBegins()
        {
            if(!PlayFromFirstScene)
                return;
 
            if(EditorBuildSettings.scenes.Length  == 0)
            {
                Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
                return;
            }
 
            foreach(GameObject go in Object.FindObjectsOfType<GameObject>())
                go.SetActive(false);
         
            SceneManager.LoadScene(0);
        }

        private static void ShowNotifyOrLog(string msg)
        {
            if(Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            else
                Debug.Log(msg); // When there's no scene view opened, we just print a log
        }
    }
}
#endif