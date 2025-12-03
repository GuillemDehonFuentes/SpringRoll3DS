using System;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English,
    French,
    German,
    Italian,
    Spanish,
    Dutch,
    Portuguese,
    Russian,
    Japanese
}

[Serializable]
public class LocalizationEntry
{
    public string ID;
    public List<string> texts = new List<string>();
}

public class LocalizationTable : ScriptableObject
{
    public List<Language> languages = new List<Language>();
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();

    public Language currentLanguage = Language.English;

    public string GetText(string id)
    {
        var entry = entries.Find(e => e.ID == id);

        if (entry == null)
            return null;

        int index = languages.IndexOf(currentLanguage);
        if (index < 0 || index >= entry.texts.Count)
            return null;

        return entry.texts[index];
    }
}
