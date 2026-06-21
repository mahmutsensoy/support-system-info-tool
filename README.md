# Sistem Bilgi Toplayıcı (Support System Info Tool)

Yazılım destek ekibi için hızlı sistem raporu oluşturan basit .NET konsol uygulaması.

**Geliştirici:** [mahmutsensoy](https://github.com/mahmutsensoy)

Kullanıcıdan gelen *"bilgisayarım yavaş"*, *"uygulama açılmıyor"* veya *"sistem bilgilerimi gönderin"* taleplerinde tek komutla teşhis verisi toplar.

## Ekran görüntüleri

> PNG dosyalarını `docs/images/` klasörüne ekledikten sonra aşağıdaki görseller GitHub'da görünür.

![Uygulama konsol çıktısı](docs/images/console-output.png)

![Oluşturulan rapor dosyası](docs/images/report-file.png)

*Ekran görüntüsü alma adımları: [docs/images/README.md](docs/images/README.md)*

## Ne işe yarar?

- İşletim sistemi, RAM, disk ve ağ bilgilerini tek raporda toplar
- Yüklü programların kısa bir listesini çıkarır
- Raporu hem ekranda gösterir hem `.txt` dosyasına kaydeder
- Destek ekibine gönderilmeye hazır, okunabilir çıktı üretir

## Toplanan bilgiler

| Kategori | Detay |
|----------|-------|
| Sistem | Bilgisayar adı, OS sürümü, mimari, çalışma süresi |
| Donanım | İşlemci adı, çekirdek sayısı, toplam/boş RAM |
| Disk | Tüm sürücüler, toplam/boş alan, doluluk yüzdesi |
| Ağ | Adaptör adı, durum, MAC, IP adresleri |
| Yazılım | Yüklü programlar (ilk 15, alfabetik) |

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (veya .NET 8+ — `TargetFramework` değerini güncelleyin)
- Windows (WMI ve kayıt defteri sorguları Windows'a özeldir)

## Nasıl çalıştırılır?

```bash
# Projeyi klonlayın
git clone https://github.com/mahmutsensoy/support-system-info-tool.git
cd support-system-info-tool

# Çalıştırın
dotnet run

# Veya derleyip exe oluşturun
dotnet publish -c Release -r win-x64 --self-contained false
```

Uygulama çalıştığında rapor ekrana yazdırılır ve `sistem-raporu_YYYYMMDD_HHmmss.txt` dosyası oluşturulur.

## Örnek çıktı

Tam örnek rapor: [docs/samples/ornek-rapor.txt](docs/samples/ornek-rapor.txt)

```
========================================
   SISTEM BILGI RAPORU
   Yazilim Destek Araci
========================================

Olusturulma:     2026-06-21 14:30:00
Bilgisayar Adi:  DESKTOP-ORNEK01

--- ISLETIM SISTEMI ---
OS:              Microsoft Windows 10.0.26200
Surum:           Microsoft Windows NT 10.0.26200.0
Mimari:          X64
Calisma Suresi:  2 gun, 5 saat, 12 dakika

--- DONANIM ---
Islemci:         Intel(R) Core(TM) i7-11800H @ 2.30GHz
Cekirdek:        16
Toplam RAM:      16,00 GB
Bos RAM:         8,24 GB

--- DISKLER ---
  C:\ (Windows)
    Tur: Fixed | Toplam: 476,94 GB | Bos: 120,50 GB | Dolu: 74,7%
...
```

## Kullanılan teknolojiler

- C# / .NET 10
- `System.Management` (WMI — CPU ve RAM)
- `System.Net.NetworkInformation` (ağ bilgisi)
- `Microsoft.Win32.Registry` (yüklü programlar)

## Proje yapısı

```
support-system-info-tool/
├── Program.cs                      # Giriş noktası
├── Models/
│   └── SystemReport.cs             # Rapor veri modelleri
├── Services/
│   └── SystemInfoCollector.cs      # Bilgi toplama ve formatlama
├── docs/
│   ├── images/                     # README ekran görüntüleri
│   │   ├── console-output.png      # (siz ekleyeceksiniz)
│   │   └── report-file.png         # (siz ekleyeceksiniz)
│   └── samples/
│       └── ornek-rapor.txt         # Örnek rapor çıktısı
└── README.md
```

## Destek senaryoları

| Talep | Bu araç ne sağlar? |
|-------|-------------------|
| "Bilgisayarım yavaş" | RAM ve disk doluluk oranı |
| "İnternet yok" | Ağ adaptör durumu ve IP |
| "Hangi programlar yüklü?" | Yüklü program listesi |
| "Sistem bilgilerimi gönderin" | Tek dosyalık tam rapor |

## Lisans

MIT — özgürce kullanabilir, fork edebilir ve geliştirebilirsiniz.
