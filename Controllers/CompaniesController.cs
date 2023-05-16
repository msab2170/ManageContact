using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AddressManager.Data;
using AddressManager.Models;
using Microsoft.Data.SqlClient;

namespace AddressManager.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AddressManagerContext _context;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(AddressManagerContext context, ILogger<CompaniesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
              return _context.Company != null ? 
                          View(await _context.Company
                          .Where(c => c.IsDelete == "N")

                          .ToListAsync()) :
                          Problem("Entity set 'AddressManagerContext.Company'  is null.");
        }

        // GET: Companies
        public async Task<IActionResult> DelList()
        {
            return _context.Company != null ?
                        View(await _context.Company
                        .Where(c => c.IsDelete == "Y")

                        .ToListAsync()) :
                        Problem("Entity set 'AddressManagerContext.Company'  is null.");
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Contact")] Company company)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (ModelState.IsValid && userId != 0 && userLoginId != null)
            {
                var a = new SqlParameter("@Name", company.Name);
                var b = new SqlParameter("@Contact", company.Contact);
                var c = new SqlParameter("@Address", company.Address);

                var d = new SqlParameter("@UserId", userId);
                var e = new SqlParameter("@LoginId", userLoginId);
                var f = new SqlParameter("@CompanyId", company.Id);
                var g = new SqlParameter("@CompanyName", company.Name);
                var h = new SqlParameter("@ActIP", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
                var i = new SqlParameter("@Act", "C");
                string query = @"EXEC CreateCompany 
                                    @Name, @Contact, @Address, @UserId, 
                                    @LoginId, @CompanyId, @CompanyName, @ActIP, @Act";

                await _context.Database.ExecuteSqlRawAsync(query, a, b,c,d,e,f,g,h,i);
                _logger.Log(LogLevel.Information, 
                    $"create company => UserId = {d.SqlValue}, UserLoginId = {e.SqlValue}, " +
                    $"target = Company, CompanyName ={g.SqlValue}, IP = {h.SqlValue}, time = {DateTime.Now}");

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Contact,IsDelete")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (_context.Company == null)
            {
                return Problem("Entity set 'AddressManagerContext.Company'  is null.");
            } 
            else if (userId == 0 || userLoginId == null) {
                return Problem("login user information doesn't exist.");
            }
            var p1 = new SqlParameter("@id", id);
            var p2 = new SqlParameter("@DorR", "Y");
            var p3 = new SqlParameter("@UserId", userId);
            var p4 = new SqlParameter("@LoginId", userLoginId);
            var p5 = new SqlParameter("@ActIP", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            var p6 = new SqlParameter("@Act", "D");

            string query = @"EXEC DorRCompany @id, @DorR, @UserId, @LoginId, @ActIP, @Act";
            await _context.Database.ExecuteSqlRawAsync(query, p1, p2, p3, p4, p5, p6);
            _logger.Log(LogLevel.Information,
                    $"delete company => UserId = {p3.SqlValue}, UserLoginId = {p4.SqlValue}, " +
                    $"target = Company, CompanyId ={id}, IP = {p5.SqlValue}, time = {DateTime.Now}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Companies/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (_context.Company == null)
            {
                return Problem("Entity set 'AddressManagerContext.Company'  is null.");
            }
            else if (userId == 0 || userLoginId == null)
            {
                return Problem("login user information doesn't exist.");
            }
            var p1 = new SqlParameter("@id", id);
            var p2 = new SqlParameter("@DorR", "N");
            var p3 = new SqlParameter("@UserId", userId);
            var p4 = new SqlParameter("@LoginId", userLoginId);
            var p5 = new SqlParameter("@ActIP", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            var p6 = new SqlParameter("@Act", "R");

            string query = @"EXEC DorRCompany @id, @DorR, @UserId, @LoginId, @ActIP, @Act";
            await _context.Database.ExecuteSqlRawAsync(query, p1, p2, p3, p4, p5, p6);
            _logger.Log(LogLevel.Information,
                    $"Restore company => UserId = {p3.SqlValue}, UserLoginId = {p4.SqlValue}, " +
                    $"target = Company, CompanyId ={id}, IP = {p5.SqlValue}, time = {DateTime.Now}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Company?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
