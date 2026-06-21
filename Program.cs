using SupportSystemInfoTool.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = "Sistem Bilgi Toplayici";

Console.WriteLine();
Console.WriteLine("  Sistem bilgileri toplaniyor...");
Console.WriteLine();

var report = SystemInfoCollector.Collect();
var content = SystemInfoCollector.FormatReport(report);

Console.WriteLine(content);

var fileName = $"sistem-raporu_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
var outputPath = Path.Combine(Environment.CurrentDirectory, fileName);

await File.WriteAllTextAsync(outputPath, content, System.Text.Encoding.UTF8);

Console.WriteLine();
Console.WriteLine($"  Rapor kaydedildi: {outputPath}");
Console.WriteLine();
Console.WriteLine("  Cikmak icin bir tusa basin...");
Console.ReadKey(intercept: true);
