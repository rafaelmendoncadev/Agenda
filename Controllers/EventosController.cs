using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agenda.Data;
using Agenda.Models;

namespace Agenda.Controllers;

[Authorize]
public class EventosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EventosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var eventos = await _context.Eventos
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DataInicio)
            .ToListAsync();

        return View(eventos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (evento == null)
        {
            return NotFound();
        }

        return View(evento);
    }

    public IActionResult Create()
    {
        var evento = new Evento
        {
            DataInicio = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1),
            DataFim = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 2)
        };
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Descricao,DataInicio,DataFim,DiaInteiro")] Evento evento)
    {
        if (ModelState.IsValid)
        {
            evento.UserId = _userManager.GetUserId(User)!;
            evento.DataCriacao = DateTime.Now;

            if (evento.DiaInteiro)
            {
                evento.DataInicio = evento.DataInicio.Date;
                evento.DataFim = evento.DataFim.Date.AddDays(1).AddSeconds(-1);
            }

            if (evento.DataFim <= evento.DataInicio)
            {
                ModelState.AddModelError("DataFim", "A data de fim deve ser posterior à data de início.");
                return View(evento);
            }

            _context.Add(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(evento);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,DataInicio,DataFim,DiaInteiro,UserId,DataCriacao")] Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (evento.UserId != userId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (evento.DiaInteiro)
                {
                    evento.DataInicio = evento.DataInicio.Date;
                    evento.DataFim = evento.DataFim.Date.AddDays(1).AddSeconds(-1);
                }

                if (evento.DataFim <= evento.DataInicio)
                {
                    ModelState.AddModelError("DataFim", "A data de fim deve ser posterior à data de início.");
                    return View(evento);
                }

                _context.Update(evento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(evento.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(evento);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (evento == null)
        {
            return NotFound();
        }

        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (evento != null)
        {
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool EventoExists(int id)
    {
        var userId = _userManager.GetUserId(User);
        return _context.Eventos.Any(e => e.Id == id && e.UserId == userId);
    }
}