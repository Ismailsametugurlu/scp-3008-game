using UnityEngine;
using UnityEditor;

// Player'a PlayerInteractor ekler: Tools > SCP3008 > Player Interactor Ekle
public static class PlayerInteractorSetupEditor
{
    [MenuItem("Tools/SCP3008/Player Interactor Ekle")]
    public static void AddInteractor()
    {
        PlayerStatsController stats = Object.FindFirstObjectByType<PlayerStatsController>();
        if (stats == null) { Debug.LogError("[PlayerInteractorSetup] Sahnede PlayerStatsController yok."); return; }

        GameObject player = stats.gameObject;
        if (player.GetComponent<PlayerInteractor>() != null)
        {
            Debug.Log("[PlayerInteractorSetup] Player'da zaten PlayerInteractor var.");
            return;
        }

        player.AddComponent<PlayerInteractor>();
        EditorUtility.SetDirty(player);
        Debug.Log("[PlayerInteractorSetup] PlayerInteractor eklendi.");
    }
}
