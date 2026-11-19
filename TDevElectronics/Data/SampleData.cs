using System.Globalization;

namespace TDevElectronics.Data
{
    /// <summary>
    /// Dữ liệu mẫu cho giai đoạn 1 (chưa có CSDL). Sang giai đoạn 2 file này bị xóa:
    /// SampleProduct / SampleCategory được thay bằng entity Product / Category (EF Core) và CatalogService.
    /// </summary>
    public sealed record PriceTier(int MinQty, decimal Unit);

    public sealed record SpecRow(string Label, string Value);

    public sealed record ColorSwatch(string Name, string Hex);

    /// <summary>Một góc nhìn trong bộ ảnh: (X, Y) là tâm cắt, Zoom là độ phóng to. Zoom = 0 nghĩa là toàn ảnh.</summary>
    public sealed record GalleryView(string Label, double X = 0, double Y = 0, double Zoom = 0)
    {
        public string Css(double aspect)
        {
            if (Zoom <= 0) return "";
            var w = aspect * Zoom;
            var h = Zoom;
            static double Cl(double n) => Math.Max(0, Math.Min(1, n));
            var px = Cl((X * w - .6) / (w - 1.2)) * 100;
            var py = Cl((Y * h - .5) / (h - 1)) * 100;
            return string.Create(CultureInfo.InvariantCulture,
                $"background-size:auto {Zoom * 100:0.#}%;background-position:{px:0.0}% {py:0.0}%");
        }
    }

    public sealed class SampleCategory
    {
        public required string Slug { get; init; }
        public required string Name { get; init; }
        public required string Icon { get; init; }
        public required string Pill { get; init; }
        public required string Title { get; init; }
        public required string Lead { get; init; }
        public required string[] Chips { get; init; }
        /// <summary>Key của sản phẩm làm ảnh đại diện ở đầu trang danh mục.</summary>
        public required string HeroKey { get; init; }
        /// <summary>Hai ngưỡng giá chia 3 nhóm: dưới Low, từ Low đến High, từ High trở lên.</summary>
        public required decimal Low { get; init; }
        public required decimal High { get; init; }
        public required string[] BandNotes { get; init; }
    }

    public sealed class SampleProduct
    {
        /// <summary>Mã dùng cho giỏ hàng, yêu thích (data-k). Giữ nguyên key của bản template.</summary>
        public required string Key { get; init; }
        /// <summary>Mã trên URL: /Product/Detail/{sku}.</summary>
        public required string Sku { get; init; }
        public required string Name { get; init; }
        public required string Brand { get; init; }
        public required string CategorySlug { get; init; }
        public required string Spec { get; init; }
        public required PriceTier[] Tiers { get; init; }
        public string? Icon { get; init; }
        public int Tint { get; init; } = 1;
        public required string Rating { get; init; }
        public int Reviews { get; init; }
        public required string Type { get; init; }
        public int? Stock { get; init; }
        public string Description { get; init; } = "";
        public string OverviewTitle { get; init; } = "About this device";
        public string About { get; init; } = "";
        public string[] Highlights { get; init; } = Array.Empty<string>();
        public SpecRow[] Specs { get; init; } = Array.Empty<SpecRow>();
        public string[] Delivery { get; init; } = Array.Empty<string>();
        public string Warranty { get; init; } = "Manufacturer warranty";
        public GalleryView[] Views { get; init; } = new GalleryView[] { new("Front view") };
        public double Aspect { get; init; } = 1.555;
        public ColorSwatch[] Colors { get; init; } = Array.Empty<ColorSwatch>();
        public string[] Options { get; init; } = Array.Empty<string>();

        public decimal BasePrice => Tiers.OrderBy(t => t.MinQty).First().Unit;
        public bool HasBulk => Tiers.Length > 1;
        public double RatingValue => double.TryParse(Rating, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : 0;
        public bool IsLowStock => Stock is > 0 and <= 10;
        public string StockLabel => Stock is null ? "In stock" : IsLowStock ? $"Only {Stock} left" : $"In stock · {Stock} available";
        public string PriceText => SampleData.Money(BasePrice);
    }

    public static class SampleData
    {
        private static readonly CultureInfo Us = CultureInfo.GetCultureInfo("en-US");
        public static string Money(decimal v) => v.ToString("C2", Us);

        public static IReadOnlyList<SampleCategory> Categories { get; } = new SampleCategory[]
        {
            new() { Slug = "phones", Name = "Phones", Icon = "phone", Pill = "5G ready", HeroKey = "phone", Low = 600, High = 1000,
                Title = "Phones that keep up with you",
                Lead = "Big screens, long days and cameras that make everyday moments look good. Start with your budget.",
                Chips = new[] { "256GB storage", "5G", "Icy blue" },
                BandNotes = new[] { "Everyday phones that do the basics well.", "The sweet spot for most people.", "The best cameras and the biggest screens." } },
            new() { Slug = "laptops", Name = "Laptops", Icon = "laptop", Pill = "Built for work", HeroKey = "laptop", Low = 1000, High = 1500,
                Title = "Power for the way you work",
                Lead = "Light enough for every commute, strong enough for the whole day. Compare ultralights, creator machines and business notebooks.",
                Chips = new[] { "Up to 18h battery", "Team pricing", "Free delivery" },
                BandNotes = new[] { "Reliable notebooks for school, home and everyday work.", "Fast, light machines most teams choose.", "Premium laptops for creators and power users." } },
            new() { Slug = "audio", Name = "Audio", Icon = "headphones", Pill = "Audio", HeroKey = "headphones", Low = 100, High = 250,
                Title = "Hear more. Hear less noise.",
                Lead = "Over-ear comfort, pocket-sized buds or a speaker for the desk. Pick a type below or browse everything.",
                Chips = new[] { "Noise cancelling", "Wireless", "Long battery" },
                BandNotes = new[] { "Great sound that will not stretch your budget.", "Our most popular earbuds and headphones.", "Premium sound with the best noise cancelling." } },
            new() { Slug = "office-furniture", Name = "Office Furniture", Icon = "chair", Pill = "Office furniture", HeroKey = "chair", Low = 150, High = 400,
                Title = "Comfortable desks and chairs for the whole workday",
                Lead = "Ergonomic chairs, standing desks and storage for offices of every size. Pick a type below or browse everything.",
                Chips = new[] { "Ergonomic", "Sit-stand", "Bulk pricing" },
                BandNotes = new[] { "Simple pieces that finish a desk setup.", "The chairs and desks most teams choose.", "Premium seating and desks built to last." } },
            new() { Slug = "printers", Name = "Printers", Icon = "printer", Pill = "Printers", HeroKey = "printer", Low = 200, High = 400,
                Title = "Find the right printer in seconds",
                Lead = "Tell us what you print most and we will narrow it down. Laser for speed, ink tank for low running costs.",
                Chips = new[] { "Wi-Fi", "Duplex", "Ink tank" },
                BandNotes = new[] { "Compact printers for home and small desks.", "All-rounders for busy small offices.", "Fast, large-format and colour laser printers." } },
            new() { Slug = "accessories", Name = "Accessories", Icon = "plug", Pill = "Accessories", HeroKey = "hub", Low = 40, High = 100,
                Title = "Everything that completes your setup",
                Lead = "Hubs, chargers and cables, picked to work with the rest of your kit.",
                Chips = new[] { "Fast charging", "USB-C", "Genuine" },
                BandNotes = new[] { "Small upgrades that make every day easier.", "Chargers, hubs and power banks you will use daily.", "Docks and cameras for a complete setup." } },
        };

        private static readonly Dictionary<string, string> CatName = Categories.ToDictionary(c => c.Slug, c => c.Name);

        // ---------- 7 sản phẩm gốc: có ảnh thật và nội dung chi tiết ----------
        private static SampleProduct[] Core() => new SampleProduct[]
        {
            new() { Key = "laptop", Sku = "macbook-air-m4", Name = "MacBook Air M4", Brand = "Apple", CategorySlug = "laptops",
                Spec = "13.6-inch · 16GB / 256GB · Sky blue", Tiers = new[] { new PriceTier(1, 1099), new PriceTier(10, 1049), new PriceTier(25, 999) },
                Rating = "4.9", Reviews = 128, Type = "Everyday", Stock = 120, Aspect = 1.555, Warranty = "1-year Apple limited warranty",
                Description = "Supercharged for your everyday. The beautifully light 13.6-inch Air brings all-day power to your ideas.",
                OverviewTitle = "Made for more possibilities",
                About = "The M4 chip makes multitasking feel effortless. A brilliant Liquid Retina display and 12MP Center Stage camera keep work and play looking their best.",
                Highlights = new[] { "Up to 18 hours of battery life", "Silent, fanless design at just 1.24 kg", "Built for Apple Intelligence" },
                Specs = new SpecRow[] { new("Display", "13.6″ Liquid Retina"), new("Processor", "Apple M4 · 10-core CPU"), new("Memory", "16GB unified memory"),
                    new("Storage", "256GB SSD"), new("Connectivity", "2 × Thunderbolt 4 · MagSafe"), new("Warranty", "1 year · Apple limited warranty") },
                Colors = new ColorSwatch[] { new("Sky blue", "#B9D3F5"), new("Silver", "#CBD2DB"), new("Starlight", "#D8CDB8"), new("Midnight", "#2B3548") },
                Options = new[] { "16GB / 256GB", "16GB / 512GB" },
                Views = new GalleryView[] { new("Front view"), new("Screen", .39, .28, 2.3), new("Keyboard", .47, .5, 2.4), new("Trackpad & edge", .42, .6, 2.6) } },
            new() { Key = "headphones", Sku = "wh-1000xm5", Name = "Sony WH-1000XM5", Brand = "Sony", CategorySlug = "audio",
                Spec = "Wireless · Noise cancelling · Black", Tiers = new[] { new PriceTier(1, 249) },
                Rating = "4.8", Reviews = 246, Type = "Over-ear", Aspect = 1.555,
                Description = "Industry-leading noise cancelling with a light, comfortable fit for long days and longer flights.",
                OverviewTitle = "Quiet the world around you",
                About = "Two processors and eight microphones work together to cancel noise, while soft-fit cushions keep you comfortable from the first call to the last song.",
                Highlights = new[] { "Up to 30 hours of battery life", "Multipoint Bluetooth: connect two devices at once", "Foldable design with a slim carry case" },
                Specs = new SpecRow[] { new("Type", "Over-ear · wireless"), new("Noise cancelling", "Dual processors · 8 microphones"), new("Battery", "Up to 30 hours"),
                    new("Connectivity", "Bluetooth 5.2 · multipoint"), new("Weight", "About 250 g"), new("Warranty", "Manufacturer warranty") },
                Views = new GalleryView[] { new("Front view"), new("Headband", .48, .16, 2.2), new("Ear cup", .6, .6, 2.4), new("Cushion", .33, .53, 2.4) } },
            new() { Key = "hub", Sku = "anker-565-hub", Name = "Anker 565 USB-C Hub", Brand = "Anker", CategorySlug = "accessories",
                Spec = "11-in-1 · 100W pass-through · Gray", Tiers = new[] { new PriceTier(1, 79), new PriceTier(25, 69) },
                Rating = "4.7", Reviews = 92, Type = "Hub", Aspect = 1.555,
                Description = "Eleven ports in one compact hub, so a single USB-C cable runs your whole desk.",
                OverviewTitle = "One cable. Your whole desk.",
                About = "Plug in displays, storage, network and charging through a single USB-C connection. Power passes through to your laptop so it keeps charging while you work.",
                Highlights = new[] { "11 ports in one slim aluminium body", "100W power pass-through for your laptop", "Ready for dual-monitor desk setups" },
                Specs = new SpecRow[] { new("Ports", "11-in-1"), new("Charging", "100W power pass-through"), new("Colour", "Gray"), new("Warranty", "Manufacturer warranty") },
                Views = new GalleryView[] { new("Front view"), new("Left ports", .3, .37, 3), new("USB-A & SD", .55, .37, 3), new("Cable", .88, .39, 3) } },
            new() { Key = "phone", Sku = "galaxy-s25", Name = "Galaxy S25", Brand = "Samsung", CategorySlug = "phones",
                Spec = "256GB · 5G · Icy blue", Tiers = new[] { new PriceTier(1, 799) },
                Rating = "4.8", Reviews = 174, Type = "Everyday", Aspect = 1.555,
                Description = "A fast, bright everyday phone with a camera that keeps up with you.",
                OverviewTitle = "Fast. Bright. Always ready.",
                About = "Galaxy S25 pairs a vivid 6.2-inch display with a powerful processor and a flexible triple camera, wrapped in a pocketable design.",
                Highlights = new[] { "256GB of storage for photos, apps and files", "5G ready for fast downloads on the go", "All-day battery with fast charging" },
                Specs = new SpecRow[] { new("Display", "6.2″ Dynamic AMOLED 2X"), new("Storage", "256GB"), new("Network", "5G"), new("Colour", "Icy blue"), new("Warranty", "Manufacturer warranty") },
                Views = new GalleryView[] { new("Front & back"), new("Display", .33, .36, 2), new("Camera", .55, .17, 3.2), new("Back panel", .62, .5, 2.2) } },
            new() { Key = "printer", Sku = "hp-laserjet-m404dn", Name = "HP LaserJet Pro M404dn", Brand = "HP", CategorySlug = "printers",
                Spec = "Mono laser · 38 ppm · Duplex · Ethernet", Tiers = new[] { new PriceTier(1, 249), new PriceTier(10, 229) },
                Rating = "4.9", Reviews = 63, Type = "Office", Aspect = 1.654,
                Description = "A fast, dependable mono laser printer for busy small offices.",
                OverviewTitle = "Built for the busy office",
                About = "Print up to 38 pages a minute with automatic two-sided printing and a wired network connection that every desk can share.",
                Highlights = new[] { "38 pages per minute", "Automatic duplex (two-sided) printing", "Gigabit Ethernet for shared offices" },
                Specs = new SpecRow[] { new("Type", "Mono laser"), new("Speed", "38 ppm"), new("Duplex", "Automatic"), new("Connectivity", "Ethernet · USB"), new("Warranty", "Manufacturer warranty") },
                Views = new GalleryView[] { new("Front view"), new("Control panel", .62, .3, 2.6), new("Paper tray", .5, .7, 2.4), new("Output tray", .45, .17, 2.6) } },
            new() { Key = "chair", Sku = "ergohuman-elite", Name = "Ergohuman Elite Mesh Chair", Brand = "Ergohuman", CategorySlug = "office-furniture",
                Spec = "Mesh back · Headrest · 4D armrests", Tiers = new[] { new PriceTier(1, 529), new PriceTier(10, 479) },
                Rating = "4.6", Reviews = 112, Type = "Seating", Stock = 8, Aspect = 1.654,
                Description = "A fully adjustable ergonomic chair that supports you through the whole workday.",
                OverviewTitle = "Support that moves with you",
                About = "A breathable mesh back, adjustable headrest and 4D armrests let you tune the fit to your body, so long days feel easier.",
                Highlights = new[] { "Breathable mesh back", "Adjustable headrest", "4D armrests that move in every direction" },
                Specs = new SpecRow[] { new("Back", "Mesh"), new("Headrest", "Adjustable"), new("Armrests", "4D adjustable"), new("Warranty", "Manufacturer warranty") },
                Views = new GalleryView[] { new("Front view"), new("Backrest", .5, .3, 2.4), new("Armrest", .7, .5, 2.6), new("Base & wheels", .5, .85, 2.6) } },
            new() { Key = "airpods", Sku = "airpods-pro-2", Name = "AirPods Pro 2", Brand = "Apple", CategorySlug = "audio",
                Spec = "USB-C · Active noise cancellation", Tiers = new[] { new PriceTier(1, 199) },
                Rating = "4.9", Reviews = 318, Type = "Earbuds", Aspect = 1.705, Warranty = "1-year Apple limited warranty",
                Description = "Active noise cancellation and rich sound in a pocket-sized case.",
                OverviewTitle = "Sound that adapts to you",
                About = "AirPods Pro 2 block out the world with Active Noise Cancellation and let it back in with Transparency mode, all in a compact USB-C charging case.",
                Highlights = new[] { "Active Noise Cancellation and Transparency mode", "USB-C charging case", "Soft tips in several sizes for a secure fit" },
                Specs = new SpecRow[] { new("Chip", "Apple H2"), new("Noise control", "Active Noise Cancellation · Transparency"), new("Case", "USB-C charging"), new("Warranty", "1 year · Apple limited warranty") },
                Views = new GalleryView[] { new("Front view"), new("Earbuds", .45, .42, 2.6), new("Charging case", .52, .66, 2.4), new("Lid", .52, .25, 2.6) } },
        };

        // ---------- sản phẩm còn lại: biểu tượng + màu nền, nội dung chi tiết sinh từ mô tả ngắn ----------
        private static SampleProduct Extra(string key, string name, string brand, string cat, string spec, decimal price,
            string icon, int tint, string rating, string type, string? sku = null, int? reviews = null)
        {
            var parts = spec.Split(" · ", StringSplitOptions.RemoveEmptyEntries);
            return new SampleProduct
            {
                Key = key, Sku = sku ?? key, Name = name, Brand = brand, CategorySlug = cat, Spec = spec,
                Tiers = new[] { new PriceTier(1, price) }, Icon = icon, Tint = tint, Rating = rating, Type = type,
                Reviews = reviews ?? 40 + key.Length * 37 % 210,
                Description = spec + ".",
                About = spec + ". Backed by the manufacturer warranty and our 30-day return policy.",
                Highlights = parts,
                Specs = new SpecRow[] { new("Brand", brand), new("Category", CatName[cat]), new("Details", spec), new("Warranty", "Manufacturer warranty") },
            };
        }

        private static SampleProduct[] Others() => new SampleProduct[]
        {
            Extra("iphone-16", "iPhone 16", "Apple", "phones", "128GB · A18 · Ultramarine", 799, "phone", 2, "4.8", "Everyday"),
            Extra("iphone-16-pro", "iPhone 16 Pro", "Apple", "phones", "256GB · Titanium · 48MP camera", 999, "phone", 1, "4.9", "Camera"),
            Extra("pixel-9", "Pixel 9", "Google", "phones", "128GB · Tensor G4 · Peony", 699, "phone", 3, "4.7", "Everyday"),
            Extra("pixel-9a", "Pixel 9a", "Google", "phones", "128GB · 7 years of updates", 499, "phone", 2, "4.6", "Value"),
            Extra("galaxy-a56", "Galaxy A56", "Samsung", "phones", "128GB · 5G · Two-day battery", 449, "phone", 1, "4.6", "Value"),
            Extra("galaxy-s25-ultra", "Galaxy S25 Ultra", "Samsung", "phones", "512GB · S Pen · 200MP camera", 1299, "phone", 3, "4.9", "Camera"),
            Extra("galaxy-z-fold6", "Galaxy Z Fold6", "Samsung", "phones", "Folds open to 7.6 inches · 256GB", 1799, "phone", 2, "4.6", "Foldable"),
            Extra("macbook-pro-14", "MacBook Pro 14 M4", "Apple", "laptops", "14.2-inch · 16GB / 512GB · Space black", 1599, "laptop", 2, "4.9", "Creators", sku: "macbook-pro-14-m4"),
            Extra("dell-xps-13", "Dell XPS 13", "Dell", "laptops", "13.4-inch · 16GB / 512GB · Platinum", 1199, "laptop", 1, "4.6", "Everyday"),
            Extra("latitude-7450", "Dell Latitude 7450", "Dell", "laptops", "14-inch · 16GB / 512GB · vPro", 1399, "laptop", 3, "4.7", "Business"),
            Extra("thinkpad-x1", "ThinkPad X1 Carbon", "Lenovo", "laptops", "14-inch · 32GB / 1TB · 1.1 kg", 1649, "laptop", 2, "4.8", "Business"),
            Extra("galaxy-book4-pro", "Galaxy Book4 Pro", "Samsung", "laptops", "14-inch AMOLED · 16GB / 512GB", 1449, "laptop", 1, "4.5", "Creators"),
            Extra("surface-laptop-7", "Surface Laptop 7", "Microsoft", "laptops", "13.8-inch · 16GB / 256GB · Snapdragon", 999, "laptop", 3, "4.5", "Everyday"),
            Extra("chromebook-plus", "Acer Chromebook Plus", "Acer", "laptops", "14-inch · 8GB / 128GB · Silver", 399, "laptop", 1, "4.4", "Budget"),
            Extra("airpods-max", "AirPods Max", "Apple", "audio", "Spatial audio · USB-C · 20h battery", 549, "headphones", 2, "4.7", "Over-ear"),
            Extra("galaxy-buds3-pro", "Galaxy Buds3 Pro", "Samsung", "audio", "ANC · 24-bit audio · Silver", 249, "headphones", 1, "4.6", "Earbuds"),
            Extra("wf-1000xm5", "Sony WF-1000XM5", "Sony", "audio", "Noise cancelling · 8h battery", 299, "headphones", 3, "4.8", "Earbuds", sku: "sony-wf-1000xm5"),
            Extra("pixel-buds-pro-2", "Pixel Buds Pro 2", "Google", "audio", "ANC · Fits small ears · Hazel", 229, "headphones", 2, "4.5", "Earbuds"),
            Extra("soundcore-space-one", "Soundcore Space One", "Anker", "audio", "ANC · 40h battery · Midnight", 99, "headphones", 1, "4.5", "Over-ear"),
            Extra("srs-xb100", "Sony SRS-XB100", "Sony", "audio", "Waterproof speaker · 16h battery", 59, "headphones", 3, "4.6", "Speaker"),
            Extra("flexispot-e7", "Flexispot E7 Standing Desk", "Flexispot", "office-furniture", "Electric sit-stand · 140×70cm · Dual motor", 449, "desk", 1, "4.8", "Desk"),
            Extra("hbada-e3", "HBADA E3 Ergonomic Chair", "HBADA", "office-furniture", "Lumbar support · Mesh back · Tilt lock", 179, "chair", 3, "4.5", "Seating"),
            Extra("ikea-idasen", "IKEA IDÅSEN Standing Desk", "IKEA", "office-furniture", "Sit/stand · 160×80cm · Memory presets", 399, "desk", 2, "4.6", "Desk"),
            Extra("hirsh-cabinet", "Hirsh 3-Drawer Cabinet", "Hirsh", "office-furniture", "Steel · Lockable · A4 files", 129, "desk", 3, "4.4", "Storage"),
            Extra("ergotron-lx", "Ergotron LX Monitor Arm", "Ergotron", "office-furniture", "Single arm · Up to 32-inch · Clamp mount", 149, "laptop", 1, "4.8", "Setup"),
            Extra("solemate-footrest", "Kensington SoleMate Footrest", "Kensington", "office-furniture", "Adjustable height · Memory foam · Non-slip", 59, "chair", 2, "4.5", "Comfort"),
            Extra("meeting-table", "Conference Table 6ft", "Flash Furniture", "office-furniture", "Seats 6–8 · Cable ports · Walnut", 349, "desk", 3, "4.5", "Meeting"),
            Extra("brother-mfc-l2750dw", "Brother MFC-L2750DW", "Brother", "printers", "Mono laser · Print, scan, copy, fax · Wi-Fi", 299, "printer", 1, "4.7", "All-in-one"),
            Extra("canon-g3770", "Canon PIXMA G3770", "Canon", "printers", "Ink tank · Wi-Fi · Print, scan, copy", 219, "printer", 3, "4.5", "Ink tank"),
            Extra("epson-l3250", "Epson EcoTank L3250", "Epson", "printers", "Ink tank · Wi-Fi · 5,760 dpi", 199, "printer", 2, "4.7", "Ink tank"),
            Extra("hp-m454dw", "HP Color LaserJet M454dw", "HP", "printers", "Colour laser · 28 ppm · Duplex · Wi-Fi", 499, "printer", 1, "4.6", "Colour laser"),
            Extra("brother-hl-l2370dw", "Brother HL-L2370DW", "Brother", "printers", "Compact mono laser · Duplex · Wi-Fi", 169, "printer", 3, "4.5", "Compact"),
            Extra("epson-l15150", "Epson EcoTank L15150", "Epson", "printers", "A3 · Ink tank · Duplex · Wi-Fi", 749, "printer", 2, "4.6", "A3"),
            Extra("brother-ql-820nwb", "Brother QL-820NWB", "Brother", "printers", "Label printer · Wi-Fi · Auto-cutter", 189, "printer", 1, "4.8", "Labels"),
            Extra("power-bank-737", "Anker 737 Power Bank", "Anker", "accessories", "24,000mAh · 140W · Smart display", 109, "bolt", 1, "4.7", "Power", sku: "anker-737-powerbank"),
            Extra("nano-65w", "Anker Nano 65W Charger", "Anker", "accessories", "GaN · 2 USB-C + 1 USB-A", 35, "plug", 2, "4.8", "Power"),
            Extra("magsafe-charger", "MagSafe Charger", "Apple", "accessories", "25W · 1m USB-C cable", 39, "bolt", 3, "4.6", "Power"),
            Extra("charging-cube", "Anker 3-in-1 Cube", "Anker", "accessories", "Phone, watch and earbuds", 99, "plug", 1, "4.5", "Power"),
            Extra("samsung-45w", "Samsung 45W Charger", "Samsung", "accessories", "Super Fast Charging 2.0", 39, "plug", 2, "4.6", "Power"),
            Extra("usbc-cable", "Belkin USB-C Cable", "Belkin", "accessories", "240W · 2m · Braided black", 25, "plug", 3, "4.7", "Cable"),
            Extra("tb-dock", "Dell Thunderbolt Dock", "Dell", "accessories", "Dual 4K · 130W · 8 ports", 299, "laptop", 1, "4.5", "Hub"),
            Extra("hero13", "GoPro HERO13", "GoPro", "accessories", "5.3K · Waterproof · HyperSmooth", 399, "camera", 3, "4.7", "Camera", sku: "gopro-hero13", reviews: 52),
        };

        public static IReadOnlyList<SampleProduct> Products { get; } = Core().Concat(Others()).ToArray();

        public static IReadOnlyList<string> Brands { get; } =
            Products.GroupBy(p => p.Brand).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).Select(g => g.Key).ToArray();

        public static SampleCategory? FindCategory(string? slugOrName)
        {
            if (string.IsNullOrWhiteSpace(slugOrName)) return null;
            return Categories.FirstOrDefault(c =>
                c.Slug.Equals(slugOrName, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(slugOrName, StringComparison.OrdinalIgnoreCase));
        }

        public static string CategoryName(string slug) => CatName.TryGetValue(slug, out var n) ? n : slug;

        /// <summary>Tìm theo mã trên URL (sku) hoặc theo key của template (laptop, phone...).</summary>
        public static SampleProduct? Find(string? skuOrKey)
        {
            if (string.IsNullOrWhiteSpace(skuOrKey)) return null;
            return Products.FirstOrDefault(p => p.Sku.Equals(skuOrKey, StringComparison.OrdinalIgnoreCase))
                ?? Products.FirstOrDefault(p => p.Key.Equals(skuOrKey, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyList<SampleProduct> InCategory(string slug) =>
            Products.Where(p => p.CategorySlug == slug).ToArray();

        public static int CountIn(string slug) => Products.Count(p => p.CategorySlug == slug);

        /// <summary>Gợi ý "You might also like": cùng danh mục trước, rồi tới các danh mục khác.</summary>
        public static IReadOnlyList<SampleProduct> Related(SampleProduct p, int take = 4) =>
            Products.Where(x => x.Key != p.Key)
                    .OrderByDescending(x => x.CategorySlug == p.CategorySlug)
                    .ThenByDescending(x => x.RatingValue)
                    .Take(take).ToArray();
    }
}
