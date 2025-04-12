namespace StudentManagementGO.Models
{
    public class StudentScore
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public double? Math { get; set; } // Toan
        public double? Literature { get; set; } // Ngu Van
        public double? ForeignLanguage { get; set; } // Ngoai Ngu
        public double? Physics { get; set; } // Vat Li
        public double? Chemistry { get; set; } // Hoa Hoc
        public double? Biology { get; set; } // Sinh Hoc
        public double? History { get; set; } // Lich Su
        public double? Geography { get; set; } // Dia Li
        public double? CivicEducation { get; set; } // GDCD
        public string? ForeignLanguageCode { get; set; } // Ma Ngoai Ngu
    }
} 