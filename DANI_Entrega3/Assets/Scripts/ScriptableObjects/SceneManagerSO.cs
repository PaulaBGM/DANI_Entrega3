using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SceneManagerSO", menuName = "Scriptable Objects/SceneManagerSO")]

public class SceneManagerSo : ScriptableObject
{
    [Header("Scene References")]
    public SceneAsset mainMenuScene;
    public SceneAsset inGameScene;

    // Método auxiliar para obtener el nombre de la escena
    public string MainMenuSceneName => mainMenuScene != null ? mainMenuScene.name : "";
    public string InGameSceneName => inGameScene != null ? inGameScene.name : "";
}
