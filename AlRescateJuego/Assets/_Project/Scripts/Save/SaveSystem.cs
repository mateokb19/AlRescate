using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string PREFS_KEY = "SaveData_v1";
    private static readonly string JSON_PATH =
        Path.Combine(Application.persistentDataPath, "savedata.json");

    public static SaveData Current { get; private set; } = new SaveData();

    public static void Save()
    {
        string json = JsonUtility.ToJson(Current, true);
        PlayerPrefs.SetString(PREFS_KEY, json);
        PlayerPrefs.Save();
        try { File.WriteAllText(JSON_PATH, json); }
        catch (System.Exception ex)
        { Debug.LogWarning($"[SaveSystem] No se pudo escribir JSON: {ex.Message}"); }
        Debug.Log("[SaveSystem] Guardado. Gemas=" + Current.gems);
    }

    public static void Load()
    {
        try
        {
            string json = null;
            if (File.Exists(JSON_PATH)) json = File.ReadAllText(JSON_PATH);
            else if (PlayerPrefs.HasKey(PREFS_KEY)) json = PlayerPrefs.GetString(PREFS_KEY);

            if (!string.IsNullOrEmpty(json))
            {
                Current = JsonUtility.FromJson<SaveData>(json);
                if (Current == null) Current = new SaveData();
            }
            else { Current = new SaveData(); }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[SaveSystem] Error al cargar: " + ex.Message);
            Current = new SaveData();
        }
        Debug.Log("[SaveSystem] Carga completada. Gemas=" + Current.gems);
    }

    public static void Reset() { Current = new SaveData(); Save(); }
}
