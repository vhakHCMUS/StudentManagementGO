# G-Scores - Student Score Management System

G-Scores is a web application for managing and analyzing student exam scores. The application provides an intuitive interface to search for individual student scores by registration number, view detailed statistics across subjects, and identify top-performing students.

![G-Scores Dashboard]

## Features

- **Score Search**: Look up detailed student scores by registration number
- **CSV Data Import**: Automatic import of student scores from CSV files
- **Score Level Classification**: Categorization of scores into four levels:
  - Excellent (>=8)
  - Good (6-7.99)
  - Average (4-5.99)
  - Below Average (<4)
- **Statistical Reports**: Visual representation of score distribution across subjects
- **Top Students Ranking**: List of top 10 students in Group A (Math, Physics, Chemistry)
- **Responsive Design**: Mobile-friendly interface that works on all devices
- **Modern UI**: Clean, intuitive interface with visual indicators for score levels

## Technology Stack

- **Backend**: .NET Core MVC
- **Database**: SQLite (for efficient data storage without external dependencies)
- **Data Processing**: Direct CSV processing (optimized for large datasets)
- **Frontend**: HTML, CSS, JavaScript
- **Charts**: Chart.js for data visualization
- **Containerization**: Docker support

## Technical Architecture

### Architecture Pattern

- **MVC (Model-View-Controller)**: The application follows the .NET Core MVC architecture pattern, clearly separating:
  - **Models**: Data representation (StudentScore, GroupATopStudent)
  - **Views**: UI templates and presentation logic
  - **Controllers**: Request handling and application flow

### Design Patterns

- **Repository Pattern**: Through the ScoreService class, which encapsulates all data operations
- **Dependency Injection**: Services are registered in Program.cs and injected into controllers
- **ViewModel Pattern**: Specialized models for specific view needs (StatisticsViewModel, ScoreSearchViewModel)
- **Factory Method**: For creating chart datasets and configuring components

### Database Implementation

- **SQLite**: Lightweight, file-based relational database chosen for portability
- **Direct ADO.NET**: Instead of Entity Framework, for optimal performance with large datasets
- **Connection Management**: Proper connection creation, usage, and disposal with using statements
- **Parameterized Queries**: All SQL queries use parameters to prevent SQL injection
- **Transaction Management**: Database transactions for data consistency during bulk operations

### Programming Techniques

- **Asynchronous Programming**: Extensive use of async/await for non-blocking operations
- **LINQ**: For data manipulation and transformation where appropriate
- **Null-Conditional & Null-Coalescing Operators**: For clean handling of potential null values
- **Error Handling**: Try-catch blocks with proper logging
- **Nullable Reference Types**: For better type safety
- **Extension Methods**: For enhancing functionality of existing types
- **CSV Processing**: Optimized CSV parsing using CsvHelper

### Frontend Technologies

- **Responsive Design**: Using CSS3 media queries and flexible layouts
- **CSS Custom Properties (Variables)**: For consistent theming and easy maintenance
- **Flexbox Layout**: For flexible, responsive UI components
- **Mobile-First Approach**: Base styles for mobile with progressive enhancement for larger screens
- **Chart.js**: For interactive data visualization
- **DOM Manipulation**: Using vanilla JavaScript and jQuery
- **Event Delegation**: For efficient event handling
- **Font Awesome**: For scalable vector icons
- **CSS Animations**: For interactive elements and loading indicators

### Performance Optimizations

- **Lazy Loading**: Data is only loaded when needed
- **Data Checking**: Verification before expensive operations
- **Bulk Operations**: Using transactions for efficient batch processing
- **Query Optimization**: SQL queries are designed for efficiency
- **Resource Caching**: Static resources are properly cached
- **Asynchronous Operations**: Database and file operations don't block the UI thread

### Security Considerations

- **Input Validation**: All user inputs are validated
- **Parameterized Queries**: Preventing SQL injection attacks
- **Error Handling**: Proper error messages without exposing sensitive information
- **HTTPS Support**: For secure communication

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer
- Web browser (Chrome, Firefox, Edge, etc.)
- Git (optional, for cloning the repository)

## Getting Started

### Installation

1. Clone the repository (or download and extract the ZIP file):
   ```bash
   git clone https://github.com/yourusername/g-scores.git
   cd g-scores
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the application:
   ```bash
   dotnet build
   ```

### Running the Application

1. Run the application:
   ```bash
   dotnet run
   ```

2. Open a web browser and navigate to:
   ```
   https://localhost:5001
   ```
   or
   ```
   http://localhost:5000
   ```

### First-time Setup

1. On first launch, you'll be prompted to load data from the CSV file
2. Click the "Load Data from CSV" button and wait for the import to complete
3. Once the data is loaded, you can start searching for student scores

## Using Docker

Ensure the diem_thi_thpt_2024.csv file is in the root directory of the project (next to Dockerfile)

Build the Docker image:
    docker build -t g-scores .
Run the container with a dynamic port:
    docker run -d -p 8080:10000 --name g-scores-app g-scores
Note: The application listens on the port specified by the PORT environment variable (default 10000). To use a different port (e.g., 80), set it with -e:
    docker run -d -p 8080:80 -e PORT=80 --name g-scores-app g-scores
Access the application at:
    http://localhost:8080

## Usage Guide

### Finding a Student's Score

1. Navigate to the Dashboard page
2. Enter the student's registration number in the search field
3. Click the "Search" button to view the detailed scores

### Viewing Statistical Reports

1. Click on the "Reports" option in the sidebar menu
2. The score distribution chart displays statistics across all subjects
3. Below the chart, you can find the top 10 students in Group A subjects

## Data Format

The application expects CSV data in the following format:
```
sbd,toan,ngu_van,ngoai_ngu,vat_li,hoa_hoc,sinh_hoc,lich_su,dia_li,gdcd,ma_ngoai_ngu
01000001,8.4,6.75,8.0,6.0,5.25,5.0,,,,N1
01000002,8.6,8.5,7.2,,,,7.25,6.0,8.0,N1
...
```

Where:
- `sbd`: Registration number
- `toan`: Math score
- `ngu_van`: Literature score
- `ngoai_ngu`: Foreign language score
- And so on...

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Font Awesome for icons
- Chart.js for charts and data visualization 