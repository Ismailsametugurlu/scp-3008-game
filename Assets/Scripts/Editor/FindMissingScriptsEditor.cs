using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Sahnedeki "missing script" (eksik/silinmiş component) uyarılarını bulup Console'a listeler.
// Tools > SCP3008 > Eksik Scriptleri Bul — log satırına tıklayınca ilgili GameObject Hierarchy'de seçilir.
public static class FindMissingScriptsEditor
{
    [MenuItem("Tools/SCP3008/Eksik Scriptleri Bul")]
    public static void FindMissingScripts()
    {
        int count = 0;

        foreach (GameObject go in GetAllSceneObjects())
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    count++;
                    Debug.LogWarning($"[EksikScript] {GetPath(go)} (component sırası: {i})", go);
                }
            }
        }

        if (count == 0)
            Debug.Log("[EksikScript] Sahnede eksik script bulunamadı.");
        else
            Debug.LogWarning($"[EksikScript] Toplam {count} eksik script bulundu (yukarıdaki satırlara tıkla, Hierarchy'de seçilir).");
    }

    private static IEnumerable<GameObject> GetAllSceneObjects()
    {
        foreach (GameObject root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                yield return t.gameObject;
    }

    private static string GetPath(GameObject go)
    {
        string path = go.name;
        for (Transform t = go.transform.parent; t != null; t = t.parent)
            path = t.name + "/" + path;
        return path;
    }
}
