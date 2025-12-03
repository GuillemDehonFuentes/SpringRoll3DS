using UnityEditor;
using UnityEngine;

public class LocalizationTableEditor : EditorWindow
{
    private LocalizationTable table;
    private Vector2 scroll;

    private string newID = "";

    [MenuItem("Tools/Localization Table Editor")]
    public static void OpenWindow()
    {
        GetWindow<LocalizationTableEditor>("Localization Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Table (Multi-Idioma)", EditorStyles.boldLabel);

        table = (LocalizationTable)EditorGUILayout.ObjectField("Tabla", table, typeof(LocalizationTable), false);

        if (table == null)
        {
            if (GUILayout.Button("Crear nueva tabla"))
                CreateTable();
            return;
        }

        DrawLanguageSection();
        GUILayout.Space(10);

        DrawNewEntrySection();
        GUILayout.Space(10);

        DrawEntriesList();
    }

    private void CreateTable()
    {
        // Abrimos panel para escoger carpeta y nombre del asset
        string path = EditorUtility.SaveFilePanelInProject(
            "Guardar tabla de localización",
            "NewLocalizationTable",
            "asset",
            "Elige una ubicación para guardar la tabla"
        );

        // Si el usuario cancela
        if (string.IsNullOrEmpty(path))
            return;

        // Crear asset
        table = ScriptableObject.CreateInstance<LocalizationTable>();
        AssetDatabase.CreateAsset(table, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = table;
    }

    // -------------------------------
    // IDIOMAS
    // -------------------------------
    private void DrawLanguageSection()
    {
        GUILayout.Label("Idiomas", EditorStyles.boldLabel);

        for (int i = 0; i < table.languages.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            table.languages[i] = (Language)EditorGUILayout.EnumPopup(table.languages[i]);

            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                RemoveLanguage(i);
                return;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Añadir idioma"))
            AddLanguage();
    }

    private void AddLanguage()
    {
        table.languages.Add(Language.English);

        // A cada entrada ya existente le añadimos un texto vacío para ese nuevo idioma
        foreach (var entry in table.entries)
            entry.texts.Add("");
    }

    private void RemoveLanguage(int index)
    {
        table.languages.RemoveAt(index);

        foreach (var entry in table.entries)
        {
            if (entry.texts.Count > index)
                entry.texts.RemoveAt(index);
        }
    }

    // -------------------------------
    // NUEVA ENTRADA
    // -------------------------------
    private void DrawNewEntrySection()
    {
        GUILayout.Label("Crear nuevo ID", EditorStyles.boldLabel);

        newID = EditorGUILayout.TextField("ID:", newID);

        GUI.enabled = !string.IsNullOrEmpty(newID) && table.languages.Count > 0;

        if (GUILayout.Button("Añadir ID"))
        {
            var entry = new LocalizationEntry();
            entry.ID = newID;

            // Generamos un texto vacío por idioma
            foreach (var lang in table.languages)
                entry.texts.Add("");

            table.entries.Add(entry);
            EditorUtility.SetDirty(table);
            newID = "";
        }

        GUI.enabled = true;

        if (table.languages.Count == 0)
            EditorGUILayout.HelpBox("Añade al menos un idioma antes de crear IDs.", MessageType.Info);
    }

    // -------------------------------
    // LISTA DE ENTRADAS
    // -------------------------------
    private void DrawEntriesList()
    {
        GUILayout.Label("Entradas", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < table.entries.Count; i++)
        {
            var entry = table.entries[i];

            EditorGUILayout.BeginVertical("box");

            entry.ID = EditorGUILayout.TextField("ID:", entry.ID);

            // Un campo por idioma
            for (int langIndex = 0; langIndex < table.languages.Count; langIndex++)
            {
                var lang = table.languages[langIndex];
                entry.texts[langIndex] = EditorGUILayout.TextField(lang.ToString(), entry.texts[langIndex]);
            }

            if (GUILayout.Button("Eliminar ID"))
            {
                table.entries.RemoveAt(i);
                EditorUtility.SetDirty(table);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }
}
