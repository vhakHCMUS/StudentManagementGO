namespace StudentManagementGO.Models
{
    public class StatisticsViewModel
    {
        public Dictionary<string, List<int>> SubjectStatistics { get; set; } = new Dictionary<string, List<int>>();
        public List<GroupATopStudent> TopGroupAStudents { get; set; } = new List<GroupATopStudent>();
        
        // Dictionary mapping DB column names to display names
        public Dictionary<string, string> SubjectNames = new Dictionary<string, string>
        {
            { "Math", "Mathematics" },
            { "Literature", "Literature" },
            { "ForeignLanguage", "Foreign Language" },
            { "Physics", "Physics" },
            { "Chemistry", "Chemistry" },
            { "Biology", "Biology" },
            { "History", "History" },
            { "Geography", "Geography" },
            { "CivicEducation", "Civic Education" }
        };
        
        // Score level names
        public List<string> ScoreLevelNames = new List<string>
        {
            "Excellent (>=8)",
            "Good (6-7.99)",
            "Average (4-5.99)",
            "Below Average (<4)"
        };
    }
} 