namespace StudentManagementGO.Models
{
    public class ScoreSearchViewModel
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public StudentScore? StudentScore { get; set; }
        public bool IsDataLoaded { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
} 