using System.Globalization;
using System.Text;

namespace TDevElectronics.Services
{
    public class ExportService : IExportService
    {
        public byte[] ToCsv(IEnumerable<string> headers, IEnumerable<IEnumerable<object?>> rows)
        {
            var sb = new StringBuilder();
            sb.Append(string.Join(",", headers.Select(Cell))).Append("\r\n");
            foreach (var r in rows) sb.Append(string.Join(",", r.Select(Cell))).Append("\r\n");
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private static string Cell(object? v)
        {
            var s = v switch
            {
                null => "",
                DateTime d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => v.ToString() ?? ""
            };
            // Chống CSV injection: ô bắt đầu bằng = + - @ bị Excel hiểu là công thức
            if (s.Length > 0 && "=+-@".Contains(s[0])) s = "'" + s;
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }
    }
}
