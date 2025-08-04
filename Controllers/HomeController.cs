using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Agenda.Data;
using Agenda.Models;

namespace Agenda.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var agora = DateTime.Now;

        var proximosEventos = await _context.Eventos
            .Where(e => e.UserId == userId && e.DataInicio >= agora)
            .OrderBy(e => e.DataInicio)
            .Take(5)
            .ToListAsync();

        var eventosHoje = await _context.Eventos
            .Where(e => e.UserId == userId && 
                       e.DataInicio.Date <= agora.Date && 
                       e.DataFim.Date >= agora.Date)
            .OrderBy(e => e.DataInicio)
            .ToListAsync();

        var totalEventos = await _context.Eventos
            .CountAsync(e => e.UserId == userId);

        ViewBag.ProximosEventos = proximosEventos;
        ViewBag.EventosHoje = eventosHoje;
        ViewBag.TotalEventos = totalEventos;

        return View();
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
