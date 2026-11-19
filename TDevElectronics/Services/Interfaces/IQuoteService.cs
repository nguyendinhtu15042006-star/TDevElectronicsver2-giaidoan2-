using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Services
{
    public sealed record QuoteView(SampleQuote Quote, string Status);

    public interface IQuoteService
    {
        IReadOnlyList<QuoteView> GetAll();
        QuoteView? Get(string id);
        string Submit(RfqViewModel model);                 // trả về mã báo giá mới, ví dụ Q-2085
        OpResult UpdateStatus(string id, string status);
    }
}
