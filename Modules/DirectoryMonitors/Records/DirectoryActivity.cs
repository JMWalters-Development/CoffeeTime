using CoffeeTime.Modules.DirectoryMonitors.Enums;

namespace CoffeeTime.Modules.DirectoryMonitors.Records;

public record DirectoryActivity(
    DirectoryActivityType ActivityType,
    string FullPath,
    string? Name,
    string? OldFullPath = null,
    string? OldName = null);