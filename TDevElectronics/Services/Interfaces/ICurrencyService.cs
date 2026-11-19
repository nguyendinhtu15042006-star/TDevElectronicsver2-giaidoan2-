namespace TDevElectronics.Services
{
    /// <summary>Quy đổi VND/USD theo ExchangeRate trong appsettings.json (giai đoạn 2 đọc bảng SiteSetting).</summary>
    public interface ICurrencyService
    {
        decimal UsdToVndRate { get; }
        decimal ToVnd(decimal usd);
        decimal ToUsd(decimal vnd);
        string FormatVnd(decimal usd);
        string FormatUsd(decimal usd);
    }
}
