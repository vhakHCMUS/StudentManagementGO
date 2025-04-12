using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentManagementGO.Models;
using StudentManagementGO.Services;

namespace StudentManagementGO.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ScoreService _scoreService;

    public HomeController(ILogger<HomeController> logger, ScoreService scoreService)
    {
        _logger = logger;
        _scoreService = scoreService;
    }

    public async Task<IActionResult> Index()
    {
        // Initialize the database and check if data is loaded
        ViewBag.IsDataLoaded = await _scoreService.IsDataLoaded();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string registrationNumber)
    {
        ViewBag.IsDataLoaded = await _scoreService.IsDataLoaded();
        ViewBag.RegistrationNumber = registrationNumber;

        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            ViewBag.ErrorMessage = "Registration number is required.";
            return View();
        }

        var studentScore = await _scoreService.GetStudentScoreByRegistrationNumber(registrationNumber);
        
        if (studentScore == null)
        {
            ViewBag.ErrorMessage = "No score found for the provided registration number.";
            return View();
        }

        return View(studentScore);
    }

    [HttpPost]
    public async Task<IActionResult> LoadData()
    {
        try
        {
            await _scoreService.LoadDataFromCsv();
            TempData["SuccessMessage"] = "Data loaded successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data");
            TempData["ErrorMessage"] = "Error loading data: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
