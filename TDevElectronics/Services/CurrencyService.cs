using System.Globalization;

namespace TDevElectronics.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly decimal _rate;

        public CurrencyService(IConfiguration config)
        {
            var r = config.GetValue<decimal?>("ExchangeRate:UsdToVnd") ?? 25000m;
            _rate = r > 0 ? r : 25000m;
        }

        public decimal UsdToVndRate => _rate;
        public decimal ToVnd(decimal usd) => Math.Round(usd * _rate, 0, MidpointRounding.AwayFromZero);
        public decimal ToUsd(decimal vnd) => Math.Round(vnd / _rate, 2, MidpointRounding.AwayFromZero);
        public string FormatVnd(decimal usd) => ToVnd(usd).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " ₫";
        public string FormatUsd(decimal usd) => usd.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
    }
}
