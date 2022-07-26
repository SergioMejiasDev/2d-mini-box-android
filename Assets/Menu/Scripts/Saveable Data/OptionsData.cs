using System;

/// <summary>
/// Clase con todas las variables de las opciones que pueden ser guardadas.
/// </summary>
[Serializable]
public class OptionsData
{
    public string activeLanguage;
    public string activeRegion;
    public bool firstTimeLanguage;
    public int gameVolume;
}