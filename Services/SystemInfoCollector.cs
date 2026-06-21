using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using SupportSystemInfoTool.Models;

namespace SupportSystemInfoTool.Services;

public static class SystemInfoCollector
{
    public static SystemReport Collect()
    {
        var (totalRam, availableRam) = GetMemoryInfo();

        return new SystemReport
        {
            GeneratedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            MachineName = Environment.MachineName,
            OsName = RuntimeInformation.OSDescription,
            OsVersion = Environment.OSVersion.VersionString,
            Architecture = RuntimeInformation.OSArchitecture.ToString(),
            Uptime = FormatUptime(Environment.TickCount64),
            CpuName = GetCpuName(),
            ProcessorCount = Environment.ProcessorCount,
            TotalRam = FormatBytes(totalRam),
            AvailableRam = FormatBytes(availableRam),
            Disks = GetDiskInfo(),
            Networks = GetNetworkInfo(),
            InstalledPrograms = GetInstalledPrograms(15)
        };
    }

    private static string GetCpuName()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Bilinmiyor (yalnizca Windows desteklenir)";

        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                return obj["Name"]?.ToString()?.Trim() ?? "Bilinmiyor";
            }
        }
        catch
        {
            return "Alinamadi";
        }

        return "Bilinmiyor";
    }

    private static (ulong Total, ulong Available) GetMemoryInfo()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return (0, 0);

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

            foreach (var obj in searcher.Get())
            {
                var totalKb = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                var freeKb = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                return (totalKb * 1024, freeKb * 1024);
            }
        }
        catch
        {
            return (0, 0);
        }

        return (0, 0);
    }

    private static IReadOnlyList<DiskInfo> GetDiskInfo()
    {
        var disks = new List<DiskInfo>();

        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
        {
            var total = (ulong)drive.TotalSize;
            var free = (ulong)drive.AvailableFreeSpace;
            var usedPercent = total == 0 ? 0 : (double)(total - free) / total * 100;

            disks.Add(new DiskInfo
            {
                Name = $"{drive.Name} ({drive.VolumeLabel})",
                DriveType = drive.DriveType.ToString(),
                TotalSize = FormatBytes(total),
                FreeSpace = FormatBytes(free),
                UsedPercent = $"{usedPercent:F1}%"
            });
        }

        return disks;
    }

    private static IReadOnlyList<NetworkInfo> GetNetworkInfo()
    {
        var adapters = new List<NetworkInfo>();

        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()
                     .Where(n => n.OperationalStatus != OperationalStatus.Unknown))
        {
            var ips = nic.GetIPProperties().UnicastAddresses
                .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .ToList();

            adapters.Add(new NetworkInfo
            {
                AdapterName = nic.Name,
                Status = nic.OperationalStatus.ToString(),
                MacAddress = FormatMacAddress(nic.GetPhysicalAddress().GetAddressBytes()),
                IpAddresses = ips
            });
        }

        return adapters;
    }

    private static IReadOnlyList<string> GetInstalledPrograms(int maxCount)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return ["Yalnizca Windows desteklenir"];

        var programs = new List<string>();
        var paths = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (var path in paths)
        {
            using var key = Registry.LocalMachine.OpenSubKey(path);
            if (key is null) continue;

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);
                var displayName = subKey?.GetValue("DisplayName")?.ToString();
                if (string.IsNullOrWhiteSpace(displayName)) continue;
                if (programs.Contains(displayName)) continue;

                programs.Add(displayName);
            }
        }

        return programs.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).Take(maxCount).ToList();
    }

    public static string FormatReport(SystemReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("========================================");
        sb.AppendLine("   SISTEM BILGI RAPORU");
        sb.AppendLine("   Yazilim Destek Araci");
        sb.AppendLine("========================================");
        sb.AppendLine();
        sb.AppendLine($"Olusturulma:     {report.GeneratedAt}");
        sb.AppendLine($"Bilgisayar Adi:  {report.MachineName}");
        sb.AppendLine();
        sb.AppendLine("--- ISLETIM SISTEMI ---");
        sb.AppendLine($"OS:              {report.OsName}");
        sb.AppendLine($"Surum:           {report.OsVersion}");
        sb.AppendLine($"Mimari:          {report.Architecture}");
        sb.AppendLine($"Calisma Suresi:  {report.Uptime}");
        sb.AppendLine();
        sb.AppendLine("--- DONANIM ---");
        sb.AppendLine($"Islemci:         {report.CpuName}");
        sb.AppendLine($"Cekirdek:        {report.ProcessorCount}");
        sb.AppendLine($"Toplam RAM:      {report.TotalRam}");
        sb.AppendLine($"Bos RAM:         {report.AvailableRam}");
        sb.AppendLine();
        sb.AppendLine("--- DISKLER ---");

        foreach (var disk in report.Disks)
        {
            sb.AppendLine($"  {disk.Name}");
            sb.AppendLine($"    Tur: {disk.DriveType} | Toplam: {disk.TotalSize} | Bos: {disk.FreeSpace} | Dolu: {disk.UsedPercent}");
        }

        sb.AppendLine();
        sb.AppendLine("--- AG ADAPTORLERI ---");

        foreach (var net in report.Networks)
        {
            sb.AppendLine($"  {net.AdapterName} ({net.Status})");
            sb.AppendLine($"    MAC: {net.MacAddress}");
            sb.AppendLine($"    IP:  {(net.IpAddresses.Count > 0 ? string.Join(", ", net.IpAddresses) : "Yok")}");
        }

        sb.AppendLine();
        sb.AppendLine($"--- YUKLU PROGRAMLAR (ilk {report.InstalledPrograms.Count}) ---");

        foreach (var program in report.InstalledPrograms)
            sb.AppendLine($"  - {program}");

        sb.AppendLine();
        sb.AppendLine("========================================");
        sb.AppendLine("Rapor sonu. Destek ekibine bu dosyayi gonderebilirsiniz.");
        sb.AppendLine("========================================");

        return sb.ToString();
    }

    private static string FormatUptime(long tickCountMs)
    {
        var uptime = TimeSpan.FromMilliseconds(tickCountMs);
        return $"{(int)uptime.TotalDays} gun, {uptime.Hours} saat, {uptime.Minutes} dakika";
    }

    private static string FormatBytes(ulong bytes)
    {
        if (bytes == 0) return "Bilinmiyor";

        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = bytes;
        var order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:F2} {sizes[order]}";
    }

    private static string FormatMacAddress(byte[] bytes)
    {
        if (bytes.Length == 0) return "Yok";
        return string.Join(":", bytes.Select(b => b.ToString("X2")));
    }
}
