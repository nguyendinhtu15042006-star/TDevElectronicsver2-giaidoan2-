using System.Text.RegularExpressions;

namespace TDevElectronics.Helpers
{
    /// <summary>Một quốc gia trong danh sách giao hàng (mã ISO 3166-1 alpha-2, tên tiếng Anh và tiếng Việt).</summary>
    public sealed record Country(string Code, string NameEn, string NameVi)
    {
        public string Name(string? lang) => string.Equals(lang, "vi", StringComparison.OrdinalIgnoreCase) ? NameVi : NameEn;
    }

    /// <summary>Tỉnh, bang hoặc vùng (Code dùng để lưu, Name dùng để hiển thị).</summary>
    public sealed record Region(string Code, string Name);

    /// <summary>Quy tắc mã bưu chính của một quốc gia.</summary>
    public sealed record PostalRule(Regex Pattern, string Hint, bool Required, string Label);

    /// <summary>
    /// Dữ liệu địa chỉ cho Checkout (Phần việc C, giai đoạn 1): 237 quốc gia, 34 tỉnh/thành Việt Nam,
    /// bang của Mỹ / Canada / Úc, kiểm tra mã bưu chính và số điện thoại.
    /// Quy tắc giống hệt phần kiểm tra phía trình duyệt trong app.js để hai phía luôn cho cùng kết quả.
    /// </summary>
    public static class CountryData
    {
        public const string DefaultCountry = "VN";

        public static readonly IReadOnlyList<Country> All = new List<Country>
        {
            new("AF", "Afghanistan", "Afghanistan"),
            new("AX", "Åland Islands", "Quần đảo Åland"),
            new("AL", "Albania", "Albania"),
            new("DZ", "Algeria", "Algeria"),
            new("AS", "American Samoa", "Samoa thuộc Mỹ"),
            new("AD", "Andorra", "Andorra"),
            new("AO", "Angola", "Angola"),
            new("AI", "Anguilla", "Anguilla"),
            new("AG", "Antigua & Barbuda", "Antigua và Barbuda"),
            new("AR", "Argentina", "Argentina"),
            new("AM", "Armenia", "Armenia"),
            new("AW", "Aruba", "Aruba"),
            new("AU", "Australia", "Úc"),
            new("AT", "Austria", "Áo"),
            new("AZ", "Azerbaijan", "Azerbaijan"),
            new("BS", "Bahamas", "Bahamas"),
            new("BH", "Bahrain", "Bahrain"),
            new("BD", "Bangladesh", "Bangladesh"),
            new("BB", "Barbados", "Barbados"),
            new("BY", "Belarus", "Belarus"),
            new("BE", "Belgium", "Bỉ"),
            new("BZ", "Belize", "Belize"),
            new("BJ", "Benin", "Benin"),
            new("BM", "Bermuda", "Bermuda"),
            new("BT", "Bhutan", "Bhutan"),
            new("BO", "Bolivia", "Bolivia"),
            new("BQ", "Caribbean Netherlands", "Ca-ri-bê Hà Lan"),
            new("BA", "Bosnia & Herzegovina", "Bosnia và Herzegovina"),
            new("BW", "Botswana", "Botswana"),
            new("BR", "Brazil", "Brazil"),
            new("BN", "Brunei", "Brunei"),
            new("BG", "Bulgaria", "Bulgaria"),
            new("BF", "Burkina Faso", "Burkina Faso"),
            new("BI", "Burundi", "Burundi"),
            new("CV", "Cape Verde", "Cape Verde"),
            new("KH", "Cambodia", "Campuchia"),
            new("CM", "Cameroon", "Cameroon"),
            new("CA", "Canada", "Canada"),
            new("KY", "Cayman Islands", "Quần đảo Cayman"),
            new("CF", "Central African Republic", "Cộng hòa Trung Phi"),
            new("TD", "Chad", "Chad"),
            new("CL", "Chile", "Chile"),
            new("CN", "China", "Trung Quốc"),
            new("CO", "Colombia", "Colombia"),
            new("KM", "Comoros", "Comoros"),
            new("CG", "Congo - Brazzaville", "Congo - Brazzaville"),
            new("CD", "Congo - Kinshasa", "Congo - Kinshasa"),
            new("CK", "Cook Islands", "Quần đảo Cook"),
            new("CR", "Costa Rica", "Costa Rica"),
            new("CI", "Côte d’Ivoire", "Côte d’Ivoire"),
            new("HR", "Croatia", "Croatia"),
            new("CW", "Curaçao", "Curaçao"),
            new("CY", "Cyprus", "Síp"),
            new("CZ", "Czechia", "Séc"),
            new("DK", "Denmark", "Đan Mạch"),
            new("DJ", "Djibouti", "Djibouti"),
            new("DM", "Dominica", "Dominica"),
            new("DO", "Dominican Republic", "Cộng hòa Dominica"),
            new("EC", "Ecuador", "Ecuador"),
            new("EG", "Egypt", "Ai Cập"),
            new("SV", "El Salvador", "El Salvador"),
            new("GQ", "Equatorial Guinea", "Guinea Xích Đạo"),
            new("ER", "Eritrea", "Eritrea"),
            new("EE", "Estonia", "Estonia"),
            new("SZ", "Eswatini", "Eswatini"),
            new("ET", "Ethiopia", "Ethiopia"),
            new("FK", "Falkland Islands", "Quần đảo Falkland"),
            new("FO", "Faroe Islands", "Quần đảo Faroe"),
            new("FJ", "Fiji", "Fiji"),
            new("FI", "Finland", "Phần Lan"),
            new("FR", "France", "Pháp"),
            new("GF", "French Guiana", "Guiana thuộc Pháp"),
            new("PF", "French Polynesia", "Polynesia thuộc Pháp"),
            new("GA", "Gabon", "Gabon"),
            new("GM", "Gambia", "Gambia"),
            new("GE", "Georgia", "Georgia"),
            new("DE", "Germany", "Đức"),
            new("GH", "Ghana", "Ghana"),
            new("GI", "Gibraltar", "Gibraltar"),
            new("GR", "Greece", "Hy Lạp"),
            new("GL", "Greenland", "Greenland"),
            new("GD", "Grenada", "Grenada"),
            new("GP", "Guadeloupe", "Guadeloupe"),
            new("GU", "Guam", "Guam"),
            new("GT", "Guatemala", "Guatemala"),
            new("GG", "Guernsey", "Guernsey"),
            new("GN", "Guinea", "Guinea"),
            new("GW", "Guinea-Bissau", "Guinea-Bissau"),
            new("GY", "Guyana", "Guyana"),
            new("HT", "Haiti", "Haiti"),
            new("VA", "Vatican City", "Thành Vatican"),
            new("HN", "Honduras", "Honduras"),
            new("HK", "Hong Kong SAR China", "Đặc khu Hành chính Hồng Kông, Trung Quốc"),
            new("HU", "Hungary", "Hungary"),
            new("IS", "Iceland", "Iceland"),
            new("IN", "India", "Ấn Độ"),
            new("ID", "Indonesia", "Indonesia"),
            new("IQ", "Iraq", "Iraq"),
            new("IE", "Ireland", "Ireland"),
            new("IM", "Isle of Man", "Đảo Man"),
            new("IL", "Israel", "Israel"),
            new("IT", "Italy", "Italy"),
            new("JM", "Jamaica", "Jamaica"),
            new("JP", "Japan", "Nhật Bản"),
            new("JE", "Jersey", "Jersey"),
            new("JO", "Jordan", "Jordan"),
            new("KZ", "Kazakhstan", "Kazakhstan"),
            new("KE", "Kenya", "Kenya"),
            new("KI", "Kiribati", "Kiribati"),
            new("KR", "South Korea", "Hàn Quốc"),
            new("KW", "Kuwait", "Kuwait"),
            new("KG", "Kyrgyzstan", "Kyrgyzstan"),
            new("LA", "Laos", "Lào"),
            new("LV", "Latvia", "Latvia"),
            new("LB", "Lebanon", "Li-băng"),
            new("LS", "Lesotho", "Lesotho"),
            new("LR", "Liberia", "Liberia"),
            new("LY", "Libya", "Libya"),
            new("LI", "Liechtenstein", "Liechtenstein"),
            new("LT", "Lithuania", "Litva"),
            new("LU", "Luxembourg", "Luxembourg"),
            new("MO", "Macao SAR China", "Đặc khu Hành chính Macao, Trung Quốc"),
            new("MG", "Madagascar", "Madagascar"),
            new("MW", "Malawi", "Malawi"),
            new("MY", "Malaysia", "Malaysia"),
            new("MV", "Maldives", "Maldives"),
            new("ML", "Mali", "Mali"),
            new("MT", "Malta", "Malta"),
            new("MH", "Marshall Islands", "Quần đảo Marshall"),
            new("MQ", "Martinique", "Martinique"),
            new("MR", "Mauritania", "Mauritania"),
            new("MU", "Mauritius", "Mauritius"),
            new("YT", "Mayotte", "Mayotte"),
            new("MX", "Mexico", "Mexico"),
            new("FM", "Micronesia", "Micronesia"),
            new("MD", "Moldova", "Moldova"),
            new("MC", "Monaco", "Monaco"),
            new("MN", "Mongolia", "Mông Cổ"),
            new("ME", "Montenegro", "Montenegro"),
            new("MS", "Montserrat", "Montserrat"),
            new("MA", "Morocco", "Ma-rốc"),
            new("MZ", "Mozambique", "Mozambique"),
            new("MM", "Myanmar (Burma)", "Myanmar (Miến Điện)"),
            new("NA", "Namibia", "Namibia"),
            new("NR", "Nauru", "Nauru"),
            new("NP", "Nepal", "Nepal"),
            new("NL", "Netherlands", "Hà Lan"),
            new("NC", "New Caledonia", "New Caledonia"),
            new("NZ", "New Zealand", "New Zealand"),
            new("NI", "Nicaragua", "Nicaragua"),
            new("NE", "Niger", "Niger"),
            new("NG", "Nigeria", "Nigeria"),
            new("NU", "Niue", "Niue"),
            new("NF", "Norfolk Island", "Đảo Norfolk"),
            new("MK", "North Macedonia", "Bắc Macedonia"),
            new("MP", "Northern Mariana Islands", "Quần đảo Bắc Mariana"),
            new("NO", "Norway", "Na Uy"),
            new("OM", "Oman", "Oman"),
            new("PK", "Pakistan", "Pakistan"),
            new("PW", "Palau", "Palau"),
            new("PS", "Palestinian Territories", "Lãnh thổ Palestine"),
            new("PA", "Panama", "Panama"),
            new("PG", "Papua New Guinea", "Papua New Guinea"),
            new("PY", "Paraguay", "Paraguay"),
            new("PE", "Peru", "Peru"),
            new("PH", "Philippines", "Philippines"),
            new("PN", "Pitcairn Islands", "Quần đảo Pitcairn"),
            new("PL", "Poland", "Ba Lan"),
            new("PT", "Portugal", "Bồ Đào Nha"),
            new("PR", "Puerto Rico", "Puerto Rico"),
            new("QA", "Qatar", "Qatar"),
            new("RE", "Réunion", "Réunion"),
            new("RO", "Romania", "Romania"),
            new("RU", "Russia", "Nga"),
            new("RW", "Rwanda", "Rwanda"),
            new("BL", "St. Barthélemy", "St. Barthélemy"),
            new("SH", "St. Helena", "St. Helena"),
            new("KN", "St. Kitts & Nevis", "St. Kitts và Nevis"),
            new("LC", "St. Lucia", "St. Lucia"),
            new("MF", "St. Martin", "St. Martin"),
            new("PM", "St. Pierre & Miquelon", "Saint Pierre và Miquelon"),
            new("VC", "St. Vincent & Grenadines", "St. Vincent và Grenadines"),
            new("WS", "Samoa", "Samoa"),
            new("SM", "San Marino", "San Marino"),
            new("ST", "São Tomé & Príncipe", "São Tomé và Príncipe"),
            new("SA", "Saudi Arabia", "Ả Rập Xê-út"),
            new("SN", "Senegal", "Senegal"),
            new("RS", "Serbia", "Serbia"),
            new("SC", "Seychelles", "Seychelles"),
            new("SL", "Sierra Leone", "Sierra Leone"),
            new("SG", "Singapore", "Singapore"),
            new("SX", "Sint Maarten", "Sint Maarten"),
            new("SK", "Slovakia", "Slovakia"),
            new("SI", "Slovenia", "Slovenia"),
            new("SB", "Solomon Islands", "Quần đảo Solomon"),
            new("SO", "Somalia", "Somalia"),
            new("ZA", "South Africa", "Nam Phi"),
            new("SS", "South Sudan", "Nam Sudan"),
            new("ES", "Spain", "Tây Ban Nha"),
            new("LK", "Sri Lanka", "Sri Lanka"),
            new("SD", "Sudan", "Sudan"),
            new("SR", "Suriname", "Suriname"),
            new("SJ", "Svalbard & Jan Mayen", "Svalbard và Jan Mayen"),
            new("SE", "Sweden", "Thụy Điển"),
            new("CH", "Switzerland", "Thụy Sĩ"),
            new("TW", "Taiwan", "Đài Loan"),
            new("TJ", "Tajikistan", "Tajikistan"),
            new("TZ", "Tanzania", "Tanzania"),
            new("TH", "Thailand", "Thái Lan"),
            new("TL", "Timor-Leste", "Timor-Leste"),
            new("TG", "Togo", "Togo"),
            new("TK", "Tokelau", "Tokelau"),
            new("TO", "Tonga", "Tonga"),
            new("TT", "Trinidad & Tobago", "Trinidad và Tobago"),
            new("TN", "Tunisia", "Tunisia"),
            new("TR", "Türkiye", "Thổ Nhĩ Kỳ"),
            new("TM", "Turkmenistan", "Turkmenistan"),
            new("TC", "Turks & Caicos Islands", "Quần đảo Turks và Caicos"),
            new("TV", "Tuvalu", "Tuvalu"),
            new("UG", "Uganda", "Uganda"),
            new("UA", "Ukraine", "Ukraina"),
            new("AE", "United Arab Emirates", "Các Tiểu Vương quốc Ả Rập Thống nhất"),
            new("GB", "United Kingdom", "Vương quốc Anh"),
            new("US", "United States", "Hoa Kỳ"),
            new("UY", "Uruguay", "Uruguay"),
            new("UZ", "Uzbekistan", "Uzbekistan"),
            new("VU", "Vanuatu", "Vanuatu"),
            new("VE", "Venezuela", "Venezuela"),
            new("VN", "Vietnam", "Việt Nam"),
            new("VG", "British Virgin Islands", "Quần đảo Virgin thuộc Anh"),
            new("VI", "U.S. Virgin Islands", "Quần đảo Virgin thuộc Hoa Kỳ"),
            new("WF", "Wallis & Futuna", "Wallis và Futuna"),
            new("EH", "Western Sahara", "Tây Sahara"),
            new("YE", "Yemen", "Yemen"),
            new("ZM", "Zambia", "Zambia"),
            new("ZW", "Zimbabwe", "Zimbabwe"),
            new("XK", "Kosovo", "Kosovo"),
        };

        private static readonly Dictionary<string, Country> ByCode =
            All.ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);

        /// <summary>34 tỉnh/thành Việt Nam (sau sắp xếp lại đơn vị hành chính).</summary>
        public static readonly IReadOnlyList<Region> VietnamProvinces = new List<string>
        {
            "Hà Nội",
            "Thành phố Hồ Chí Minh",
            "Hải Phòng",
            "Đà Nẵng",
            "Cần Thơ",
            "Huế",
            "An Giang",
            "Bắc Ninh",
            "Cà Mau",
            "Cao Bằng",
            "Đắk Lắk",
            "Điện Biên",
            "Đồng Nai",
            "Đồng Tháp",
            "Gia Lai",
            "Hà Tĩnh",
            "Hưng Yên",
            "Khánh Hòa",
            "Lai Châu",
            "Lâm Đồng",
            "Lạng Sơn",
            "Lào Cai",
            "Nghệ An",
            "Ninh Bình",
            "Phú Thọ",
            "Quảng Ngãi",
            "Quảng Ninh",
            "Quảng Trị",
            "Sơn La",
            "Tây Ninh",
            "Thái Nguyên",
            "Thanh Hóa",
            "Tuyên Quang",
            "Vĩnh Long"
        }.Select(n => new Region(n, n)).ToList();

        public static readonly IReadOnlyList<Region> UsStates = new List<Region>
        {
            new("AL", "Alabama"),
            new("AK", "Alaska"),
            new("AZ", "Arizona"),
            new("AR", "Arkansas"),
            new("CA", "California"),
            new("CO", "Colorado"),
            new("CT", "Connecticut"),
            new("DE", "Delaware"),
            new("DC", "District of Columbia"),
            new("FL", "Florida"),
            new("GA", "Georgia"),
            new("HI", "Hawaii"),
            new("ID", "Idaho"),
            new("IL", "Illinois"),
            new("IN", "Indiana"),
            new("IA", "Iowa"),
            new("KS", "Kansas"),
            new("KY", "Kentucky"),
            new("LA", "Louisiana"),
            new("ME", "Maine"),
            new("MD", "Maryland"),
            new("MA", "Massachusetts"),
            new("MI", "Michigan"),
            new("MN", "Minnesota"),
            new("MS", "Mississippi"),
            new("MO", "Missouri"),
            new("MT", "Montana"),
            new("NE", "Nebraska"),
            new("NV", "Nevada"),
            new("NH", "New Hampshire"),
            new("NJ", "New Jersey"),
            new("NM", "New Mexico"),
            new("NY", "New York"),
            new("NC", "North Carolina"),
            new("ND", "North Dakota"),
            new("OH", "Ohio"),
            new("OK", "Oklahoma"),
            new("OR", "Oregon"),
            new("PA", "Pennsylvania"),
            new("RI", "Rhode Island"),
            new("SC", "South Carolina"),
            new("SD", "South Dakota"),
            new("TN", "Tennessee"),
            new("TX", "Texas"),
            new("UT", "Utah"),
            new("VT", "Vermont"),
            new("VA", "Virginia"),
            new("WA", "Washington"),
            new("WV", "West Virginia"),
            new("WI", "Wisconsin"),
            new("WY", "Wyoming")
        };

        public static readonly IReadOnlyList<Region> CanadaProvinces = new List<Region>
        {
            new("AB", "Alberta"),
            new("BC", "British Columbia"),
            new("MB", "Manitoba"),
            new("NB", "New Brunswick"),
            new("NL", "Newfoundland and Labrador"),
            new("NS", "Nova Scotia"),
            new("NT", "Northwest Territories"),
            new("NU", "Nunavut"),
            new("ON", "Ontario"),
            new("PE", "Prince Edward Island"),
            new("QC", "Quebec"),
            new("SK", "Saskatchewan"),
            new("YT", "Yukon")
        };

        public static readonly IReadOnlyList<Region> AustraliaStates = new List<Region>
        {
            new("ACT", "Australian Capital Territory"),
            new("NSW", "New South Wales"),
            new("NT", "Northern Territory"),
            new("QLD", "Queensland"),
            new("SA", "South Australia"),
            new("TAS", "Tasmania"),
            new("VIC", "Victoria"),
            new("WA", "Western Australia")
        };

        /// <summary>Các nước bắt buộc nhập tỉnh/bang/vùng.</summary>
        private static readonly HashSet<string> RegionRequired = new(StringComparer.OrdinalIgnoreCase)
        { "VN", "US", "CA", "AU", "JP", "BR", "IN", "CN", "MX", "TH", "MY", "ID", "PH", "KR" };

        private static Regex Rx(string p) => new(p, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Dictionary<string, PostalRule> Postal = BuildPostalRules();

        private static Dictionary<string, PostalRule> BuildPostalRules()
        {
            var d = new Dictionary<string, PostalRule>(StringComparer.OrdinalIgnoreCase)
            {
                ["VN"] = new(Rx(@"^\d{6}$"), "6 digits, e.g. 700000", false, "Postal code (optional)"),
                ["US"] = new(Rx(@"^\d{5}(-\d{4})?$"), "e.g. 94105", true, "ZIP code"),
                ["CA"] = new(Rx(@"^[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$"), "A1A 1A1", true, "Postal code"),
                ["AU"] = new(Rx(@"^\d{4}$"), "e.g. 2000", true, "Postcode"),
                ["GB"] = new(Rx(@"^[A-Za-z]{1,2}\d[A-Za-z\d]? ?\d[A-Za-z]{2}$"), "e.g. SW1A 1AA", true, "Postcode"),
                ["JP"] = new(Rx(@"^\d{3}-?\d{4}$"), "123-4567", true, "Postal code"),
                ["NL"] = new(Rx(@"^\d{4} ?[A-Za-z]{2}$"), "1234 AB", true, "Postal code"),
                ["BR"] = new(Rx(@"^\d{5}-?\d{3}$"), "12345-678", true, "Postal code (CEP)"),
                ["SE"] = new(Rx(@"^\d{3} ?\d{2}$"), "123 45", true, "Postal code"),
                ["PL"] = new(Rx(@"^\d{2}-\d{3}$"), "12-345", true, "Postal code"),
                ["PT"] = new(Rx(@"^\d{4}-\d{3}$"), "1234-567", true, "Postal code"),
                ["IN"] = new(Rx(@"^\d{6}$"), "6 digits", true, "PIN code"),
                ["CN"] = new(Rx(@"^\d{6}$"), "6 digits", true, "Postal code"),
                ["TW"] = new(Rx(@"^\d{3}(\d{2,3})?$"), "e.g. 100", true, "Postal code"),
            };
            void Group(string codes, string pattern, string hint)
            {
                foreach (var c in codes.Split(' '))
                    if (!d.ContainsKey(c)) d[c] = new(Rx(pattern), hint, true, "Postal code");
            }
            Group("DE FR ES IT TH KR MY ID MX TR FI EE GR HR RS UA", @"^\d{5}$", "5 digits");
            Group("AT BE CH DK NO NZ ZA HU LU BG PH CY", @"^\d{4}$", "4 digits");
            Group("SG RU BY KZ", @"^\d{6}$", "6 digits");
            return d;
        }

        private static readonly Regex PhoneVn = Rx(@"^(?:\+84|84|0)(?:[35789]\d{8}|2\d{9})$");
        private static readonly Regex PhoneNanp = Rx(@"^(?:\+?1)?[2-9]\d{9}$");
        private static readonly Regex PhoneAny = Rx(@"^\+?\d{7,15}$");

        public static bool IsKnownCountry(string? code) => !string.IsNullOrWhiteSpace(code) && ByCode.ContainsKey(code);

        public static Country? Find(string? code) =>
            !string.IsNullOrWhiteSpace(code) && ByCode.TryGetValue(code, out var c) ? c : null;

        /// <summary>Danh sách quốc gia, Việt Nam đứng đầu rồi xếp theo tên.</summary>
        public static IEnumerable<Country> Sorted(string? lang)
        {
            var comparer = StringComparer.Create(
                System.Globalization.CultureInfo.GetCultureInfo(string.Equals(lang, "vi", StringComparison.OrdinalIgnoreCase) ? "vi-VN" : "en-US"),
                ignoreCase: true);
            return All.Where(c => c.Code == DefaultCountry)
                .Concat(All.Where(c => c.Code != DefaultCountry).OrderBy(c => c.Name(lang), comparer));
        }

        /// <summary>Danh sách tỉnh/bang cố định của nước đó (rỗng nếu người dùng tự nhập).</summary>
        public static IReadOnlyList<Region> Regions(string? code) => (code ?? "").ToUpperInvariant() switch
        {
            "VN" => VietnamProvinces,
            "US" => UsStates,
            "CA" => CanadaProvinces,
            "AU" => AustraliaStates,
            _ => Array.Empty<Region>(),
        };

        public static bool IsRegionRequired(string? code) => !string.IsNullOrEmpty(code) && RegionRequired.Contains(code);

        public static PostalRule? PostalRuleFor(string? code) =>
            !string.IsNullOrEmpty(code) && Postal.TryGetValue(code, out var r) ? r : null;

        /// <summary>Kiểm tra mã bưu chính. Nước không có quy tắc thì chấp nhận mọi giá trị.</summary>
        public static bool IsValidPostalCode(string? code, string? value)
        {
            var v = (value ?? "").Trim();
            var rule = PostalRuleFor(code);
            if (rule is null) return true;
            if (v.Length == 0) return !rule.Required;
            return rule.Pattern.IsMatch(v);
        }

        /// <summary>Kiểm tra số điện thoại (bỏ khoảng trắng, dấu gạch, dấu chấm, ngoặc trước khi so khớp).</summary>
        public static bool IsValidPhone(string? code, string? value)
        {
            var v = Regex.Replace(value ?? "", @"[\s\-\.\(\)]", "");
            if (v.Length == 0) return false;
            return (code ?? "").ToUpperInvariant() switch
            {
                "VN" => PhoneVn.IsMatch(v),
                "US" or "CA" => PhoneNanp.IsMatch(v),
                _ => PhoneAny.IsMatch(v),
            };
        }
    }
}
