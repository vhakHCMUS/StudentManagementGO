using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.Sqlite;
using StudentManagementGO.Models;
using System.Globalization;

namespace StudentManagementGO.Services
{
    public class ScoreService
    {
        private readonly string _connectionString;
        private readonly string _csvFilePath;
        private readonly ILogger<ScoreService> _logger;

        public ScoreService(IConfiguration configuration, ILogger<ScoreService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=scores.db";
            _csvFilePath = configuration["CsvFilePath"] ?? "diem_thi_thpt_2024.csv";
            _logger = logger;
            
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Create database and table if not exists
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS StudentScores (
                    RegistrationNumber TEXT PRIMARY KEY,
                    Math REAL NULL,
                    Literature REAL NULL,
                    ForeignLanguage REAL NULL,
                    Physics REAL NULL,
                    Chemistry REAL NULL,
                    Biology REAL NULL,
                    History REAL NULL,
                    Geography REAL NULL,
                    CivicEducation REAL NULL,
                    ForeignLanguageCode TEXT NULL
                )";
            command.ExecuteNonQuery();
        }

        public async Task<bool> IsDataLoaded()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM StudentScores";
            
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task LoadDataFromCsv()
        {
            if (await IsDataLoaded())
            {
                _logger.LogInformation("Data already loaded, skipping CSV import");
                return;
            }

            _logger.LogInformation("Loading data from CSV file: {CsvFilePath}", _csvFilePath);

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                // Begin transaction for better performance
                using var transaction = connection.BeginTransaction();

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    MissingFieldFound = null
                };

                using var reader = new StreamReader(_csvFilePath);
                using var csv = new CsvReader(reader, csvConfig);

                // Prepare insert command
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO StudentScores 
                    (RegistrationNumber, Math, Literature, ForeignLanguage, Physics, Chemistry, Biology, History, Geography, CivicEducation, ForeignLanguageCode)
                    VALUES 
                    (@sbd, @toan, @ngu_van, @ngoai_ngu, @vat_li, @hoa_hoc, @sinh_hoc, @lich_su, @dia_li, @gdcd, @ma_ngoai_ngu)";

                // Add parameters
                insertCmd.Parameters.Add("@sbd", SqliteType.Text);
                insertCmd.Parameters.Add("@toan", SqliteType.Real);
                insertCmd.Parameters.Add("@ngu_van", SqliteType.Real);
                insertCmd.Parameters.Add("@ngoai_ngu", SqliteType.Real);
                insertCmd.Parameters.Add("@vat_li", SqliteType.Real);
                insertCmd.Parameters.Add("@hoa_hoc", SqliteType.Real);
                insertCmd.Parameters.Add("@sinh_hoc", SqliteType.Real);
                insertCmd.Parameters.Add("@lich_su", SqliteType.Real);
                insertCmd.Parameters.Add("@dia_li", SqliteType.Real);
                insertCmd.Parameters.Add("@gdcd", SqliteType.Real);
                insertCmd.Parameters.Add("@ma_ngoai_ngu", SqliteType.Text);

                int counter = 0;
                
                // Skip header
                csv.Read();
                csv.ReadHeader();
                
                // Read and process data
                while (csv.Read())
                {
                    counter++;
                    
                    // Extract registration number (mandatory)
                    var sbd = csv.GetField("sbd");
                    
                    // For each field, try to get value or set NULL if missing
                    double? toan = TryParseDouble(csv.GetField("toan"));
                    double? ngu_van = TryParseDouble(csv.GetField("ngu_van"));
                    double? ngoai_ngu = TryParseDouble(csv.GetField("ngoai_ngu"));
                    double? vat_li = TryParseDouble(csv.GetField("vat_li"));
                    double? hoa_hoc = TryParseDouble(csv.GetField("hoa_hoc"));
                    double? sinh_hoc = TryParseDouble(csv.GetField("sinh_hoc"));
                    double? lich_su = TryParseDouble(csv.GetField("lich_su"));
                    double? dia_li = TryParseDouble(csv.GetField("dia_li"));
                    double? gdcd = TryParseDouble(csv.GetField("gdcd"));
                    var ma_ngoai_ngu = csv.GetField("ma_ngoai_ngu");

                    // Set parameters
                    insertCmd.Parameters["@sbd"].Value = sbd;
                    SetParameterValue(insertCmd.Parameters["@toan"], toan);
                    SetParameterValue(insertCmd.Parameters["@ngu_van"], ngu_van);
                    SetParameterValue(insertCmd.Parameters["@ngoai_ngu"], ngoai_ngu);
                    SetParameterValue(insertCmd.Parameters["@vat_li"], vat_li);
                    SetParameterValue(insertCmd.Parameters["@hoa_hoc"], hoa_hoc);
                    SetParameterValue(insertCmd.Parameters["@sinh_hoc"], sinh_hoc);
                    SetParameterValue(insertCmd.Parameters["@lich_su"], lich_su);
                    SetParameterValue(insertCmd.Parameters["@dia_li"], dia_li);
                    SetParameterValue(insertCmd.Parameters["@gdcd"], gdcd);
                    insertCmd.Parameters["@ma_ngoai_ngu"].Value = ma_ngoai_ngu == "" ? DBNull.Value : ma_ngoai_ngu;

                    // Execute the command
                    await insertCmd.ExecuteNonQueryAsync();

                    // Log progress every 10,000 records
                    if (counter % 10000 == 0)
                    {
                        _logger.LogInformation("Imported {Counter} records", counter);
                    }
                }

                // Commit the transaction
                transaction.Commit();
                
                _logger.LogInformation("Successfully imported {Counter} records total", counter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data from CSV");
                throw;
            }
        }

        private double? TryParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
                
            return null;
        }
        
        private void SetParameterValue(SqliteParameter parameter, double? value)
        {
            parameter.Value = value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public async Task<StudentScore?> GetStudentScoreByRegistrationNumber(string registrationNumber)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM StudentScores 
                WHERE RegistrationNumber = @registrationNumber";
            command.Parameters.AddWithValue("@registrationNumber", registrationNumber);

            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new StudentScore
                {
                    RegistrationNumber = reader["RegistrationNumber"].ToString(),
                    Math = reader["Math"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Math")) : null,
                    Literature = reader["Literature"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Literature")) : null,
                    ForeignLanguage = reader["ForeignLanguage"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("ForeignLanguage")) : null,
                    Physics = reader["Physics"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Physics")) : null,
                    Chemistry = reader["Chemistry"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Chemistry")) : null,
                    Biology = reader["Biology"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Biology")) : null,
                    History = reader["History"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("History")) : null,
                    Geography = reader["Geography"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("Geography")) : null,
                    CivicEducation = reader["CivicEducation"] != DBNull.Value ? (double?)reader.GetDouble(reader.GetOrdinal("CivicEducation")) : null,
                    ForeignLanguageCode = reader["ForeignLanguageCode"]?.ToString()
                };
            }
            
            return null;
        }

        // Get statistics for subjects based on score levels
        public async Task<Dictionary<string, List<int>>> GetSubjectStatistics()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var statistics = new Dictionary<string, List<int>>
            {
                { "Math", new List<int> { 0, 0, 0, 0 } },
                { "Literature", new List<int> { 0, 0, 0, 0 } },
                { "ForeignLanguage", new List<int> { 0, 0, 0, 0 } },
                { "Physics", new List<int> { 0, 0, 0, 0 } },
                { "Chemistry", new List<int> { 0, 0, 0, 0 } },
                { "Biology", new List<int> { 0, 0, 0, 0 } },
                { "History", new List<int> { 0, 0, 0, 0 } },
                { "Geography", new List<int> { 0, 0, 0, 0 } },
                { "CivicEducation", new List<int> { 0, 0, 0, 0 } }
            };

            // Get statistics for Math
            foreach (var subject in statistics.Keys)
            {
                var command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT 
                        COUNT(CASE WHEN {subject} >= 8 THEN 1 END) as Level1,
                        COUNT(CASE WHEN {subject} >= 6 AND {subject} < 8 THEN 1 END) as Level2,
                        COUNT(CASE WHEN {subject} >= 4 AND {subject} < 6 THEN 1 END) as Level3,
                        COUNT(CASE WHEN {subject} < 4 THEN 1 END) as Level4
                    FROM StudentScores
                    WHERE {subject} IS NOT NULL";

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    statistics[subject][0] = reader.GetInt32(0); // Level 1: >=8
                    statistics[subject][1] = reader.GetInt32(1); // Level 2: 6-7.99
                    statistics[subject][2] = reader.GetInt32(2); // Level 3: 4-5.99
                    statistics[subject][3] = reader.GetInt32(3); // Level 4: <4
                }
            }

            return statistics;
        }

        // Get top 10 students for Group A (Math, Physics, Chemistry)
        public async Task<List<GroupATopStudent>> GetTopGroupAStudents(int limit = 10)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    RegistrationNumber, 
                    Math, 
                    Physics, 
                    Chemistry,
                    (IFNULL(Math, 0) + IFNULL(Physics, 0) + IFNULL(Chemistry, 0)) as TotalScore
                FROM 
                    StudentScores
                WHERE 
                    Math IS NOT NULL AND 
                    Physics IS NOT NULL AND 
                    Chemistry IS NOT NULL
                ORDER BY 
                    TotalScore DESC
                LIMIT @limit";
            
            command.Parameters.AddWithValue("@limit", limit);

            using var reader = await command.ExecuteReaderAsync();
            
            var topStudents = new List<GroupATopStudent>();
            
            while (await reader.ReadAsync())
            {
                topStudents.Add(new GroupATopStudent
                {
                    RegistrationNumber = reader.GetString(0),
                    Math = reader.GetDouble(1),
                    Physics = reader.GetDouble(2),
                    Chemistry = reader.GetDouble(3),
                    TotalScore = reader.GetDouble(4)
                });
            }

            return topStudents;
        }
    }
} 