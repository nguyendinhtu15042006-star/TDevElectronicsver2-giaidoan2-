using TDevElectronics.Data;
using TDevElectronics.Helpers;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Services
{
    /// <summary>Giai đoạn 1: báo giá mẫu + Session. Giai đoạn 2: bảng Quote, QuoteItem.</summary>
    public class QuoteService : IQuoteService
    {
        private const string StatusKey = "quote-status", SeqKey = "quote-seq";
        private readonly IHttpContextAccessor _http;

        public QuoteService(IHttpContextAccessor http) => _http = http;
        private ISession Session => _http.HttpContext!.Session;

        public IReadOnlyList<QuoteView> GetAll() => SampleAdminData.Quotes.Select(Build).ToList();

        public QuoteView? Get(string id)
        {
            var q = SampleAdminData.FindQuote(id);
            return q is null ? null : Build(q);
        }

        public string Submit(RfqViewModel model)
        {
            var n = (Session.GetInt32(SeqKey) ?? 2084) + 1;
            Session.SetInt32(SeqKey, n);
            return "Q-" + n;
        }

        public OpResult UpdateStatus(string id, string status)
        {
            var q = SampleAdminData.FindQuote(id);
            if (q is null) return new(false, "Không tìm thấy báo giá.");
            if (!SampleAdminData.QuoteStatuses.Contains(status)) return new(false, "Trạng thái không hợp lệ.");
            SessionState.Set(Session, StatusKey, q.Id, status);
            return new(true, $"Báo giá {q.Id} chuyển sang {status}.");
        }

        private QuoteView Build(SampleQuote q) =>
            new(q, SessionState.Get(Session, StatusKey).GetValueOrDefault(q.Id, q.Status));
    }
}
