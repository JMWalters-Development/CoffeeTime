namespace CoffeeTime.Interfaces;

public interface ISaveDataService
{
    T? LoadJson<T>(string relativePath);
    void SaveJson<T>(T data, string relativePath);
}