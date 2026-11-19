namespace TDevElectronics.Services
{
    /// <summary>Xuất báo cáo. Giai đoạn 1 xuất CSV (mở được bằng Excel); giai đoạn 2 thêm ClosedXML (xlsx) và QuestPDF (pdf).</summary>
    public interface IExportService
    {
        byte[] ToCsv(IEnumerable<string> headers, IEnumerable<IEnumerable<object?>> rows);
    }
}
