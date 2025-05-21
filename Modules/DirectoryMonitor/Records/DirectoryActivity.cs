using CoffeeTime.Modules.DirectoryMonitor.Enums;

namespace CoffeeTime.Modules.DirectoryMonitor.Records;

public record DirectoryActivity(DirectoryActivityType ActivityType, string Path, string? OldPath = null);