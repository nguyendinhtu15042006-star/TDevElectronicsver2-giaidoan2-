namespace TDevElectronics.Models.Enums
{
    /// <summary>Trạng thái báo giá: Under review, Quoted, Accepted, Expired, Archived.</summary>
    public enum QuoteStatus { UnderReview, Quoted, Accepted, Expired, Archived }

    public static class QuoteStatusInfo
    {
        public static string DisplayName(this QuoteStatus s) => s switch
        {
            QuoteStatus.UnderReview => "Under review",
            QuoteStatus.Quoted => "Quoted",
            QuoteStatus.Accepted => "Accepted",
            QuoteStatus.Expired => "Expired",
            QuoteStatus.Archived => "Archived",
            _ => s.ToString()
        };
    }
}
