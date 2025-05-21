using System;
using System.IO;
using System.Text.Json;
using CoffeeTime.Interfaces;

namespace CoffeeTime.Services;

/// <summary>
/// Handles saving and loading JSON data files exclusively in the user's ApplicationData folder 
/// under the "CoffeeTime" subdirectory.
/// </summary>
public class SaveDataService : ISaveDataService
{
    #region Private fields and properties

    private readonly string _appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CoffeeTime"
    );
    
    #endregion
    
    public SaveDataService()
    {
        CreateAppDataFolderIfNotExists();
    }
    
    #region Private functions and methods

    private void CreateAppDataFolderIfNotExists()
    {
        if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
    }
    
    #endregion
    
    #region Public functions and methods
    
    // Functions
    /// <summary>
    /// Loads a JSON file from the ApplicationData\CoffeeTime folder and deserializes it to the specified type.
    /// Returns <c>default</c> if the directory or file does not exist.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="relativePath">The relative path to the JSON file within the CoffeeTime folder.</param>
    /// <returns>The deserialized object or <c>default</c> if not found.</returns>
    public T? LoadJson<T>(string relativePath)
    {
        if (!Directory.Exists(_appDataPath)) return default;
        
        var fullPath = Path.Combine(_appDataPath, relativePath);
        return !File.Exists(fullPath) ? default : JsonSerializer.Deserialize<T>(File.ReadAllText(fullPath));
    }
    
    // Methods
    /// <summary>
    /// Serializes the given object as JSON and saves it to the ApplicationData\CoffeeTime folder.
    /// Does nothing if the application data directory does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="data">The object to serialize.</param>
    /// <param name="relativePath">The relative path to the JSON file within the CoffeeTime folder.</param>
    public void SaveJson<T>(T data, string relativePath)
    {
        CreateAppDataFolderIfNotExists();
        var fullPath = Path.Combine(_appDataPath, relativePath);
        var fileInfo = new FileInfo(fullPath);
        fileInfo.Directory?.Create();
        File.WriteAllText(fullPath, JsonSerializer.Serialize(data));
    }
    
    #endregion
}