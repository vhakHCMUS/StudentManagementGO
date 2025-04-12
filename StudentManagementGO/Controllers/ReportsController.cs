using Microsoft.AspNetCore.Mvc;
using StudentManagementGO.Models;
using StudentManagementGO.Services;

namespace StudentManagementGO.Controllers;

public class ReportsController : Controller
{
    private readonly ILogger<ReportsController> _logger;
    private readonly ScoreService _scoreService;

    public ReportsController(ILogger<ReportsController> logger, ScoreService scoreService)
    {
        _logger = logger;
        _scoreService = scoreService;
    }

    public async Task<IActionResult> Index()
    {
        // Check if data is loaded
        if (!await _scoreService.IsDataLoaded())
        {
            TempData["ErrorMessage"] = "No data loaded. Please load data first.";
            return RedirectToAction("Index", "Home");
        }

        var viewModel = new StatisticsViewModel
        {
            SubjectStatistics = await _scoreService.GetSubjectStatistics(),
            TopGroupAStudents = await _scoreService.GetTopGroupAStudents()
        };

        return View(viewModel);
    }
} 