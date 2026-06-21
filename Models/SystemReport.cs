namespace SupportSystemInfoTool.Models;

public sealed class SystemReport
{
    public required string GeneratedAt { get; init; }
    public required string MachineName { get; init; }
    public required string OsName { get; init; }
    public required string OsVersion { get; init; }
    public required string Architecture { get; init; }
    public required string Uptime { get; init; }
    public required string CpuName { get; init; }
    public required int ProcessorCount { get; init; }
    public required string TotalRam { get; init; }
    public required string AvailableRam { get; init; }
    public required IReadOnlyList<DiskInfo> Disks { get; init; }
    public required IReadOnlyList<NetworkInfo> Networks { get; init; }
    public required IReadOnlyList<string> InstalledPrograms { get; init; }
}

public sealed class DiskInfo
{
    public required string Name { get; init; }
    public required string DriveType { get; init; }
    public required string TotalSize { get; init; }
    public required string FreeSpace { get; init; }
    public required string UsedPercent { get; init; }
}

public sealed class NetworkInfo
{
    public required string AdapterName { get; init; }
    public required string Status { get; init; }
    public required string MacAddress { get; init; }
    public required IReadOnlyList<string> IpAddresses { get; init; }
}
