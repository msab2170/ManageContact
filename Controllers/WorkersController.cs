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
using System.Configuration;
using AddressManager.Models.Pages;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Routing;
using System.Data;

namespace AddressManager.Controllers
{
    public class WorkersController : Controller
    {
        private readonly AddressManagerContext _context;
        private readonly ILogger<WorkersController> _logger;
        private readonly IConfiguration Configuration;

        public WorkersController(AddressManagerContext context, ILogger<WorkersController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            Configuration = configuration;
        }

        // GET: Workers
        public async Task<IActionResult> Index(string ? columnName, string currentFilter, string searchString, int? pageIndex)
        {
            var pageSize = Configuration.GetValue("PageSize", 20);
            ViewBag.pagePerView = Configuration.GetValue("pagePerView", 20);

            // 검색조건 콤보박스
            var excludedColumns = new List<string> { "Id", "IsDelete" };
            var columns = _context.Model.FindEntityType(typeof(Worker)).GetProperties()
                .Where(p => !excludedColumns.Contains(p.GetColumnName()))
                .Select(p => p.GetColumnName()) // 컬럼명 가져오기
                .ToList();

            if (columnName != null)
            {
                ViewBag.Columns = new SelectList(columns, columnName);
            }
            else
            {
                ViewBag.Columns = new SelectList(columns);
            }

            // 검색어가 있으면
            if (!searchString.IsNullOrEmpty())
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            @ViewData["CurrentColumnFilter"] = columnName;
            ViewData["CurrentFilter"] = searchString;

            var workers = _context.Worker.Include(w => w.Company)
                                                    .Where(w => w.IsDelete == "N");
            // 검색조건과 검색어를 리스트에 반영
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "CompanyId":
                        workers = workers.Where(s => s.Company.Name.Contains(searchString));
                        break;
                    case "Name":
                        workers = workers.Where(s => s.Name.Contains(searchString));
                        break;
                    case "Email":
                        workers = workers.Where(s => s.Email.Contains(searchString));
                        break;
                    case "Phone":
                        workers = workers.Where(s => s.Phone.Contains(searchString));
                        break;
                }
                _logger.LogInformation($"In Deleted Companies Page - search condition = {columnName}, search word = {searchString}");
            }

            return _context.Worker != null ?
                          View(await Pagination<Worker>
                          .CreateAsync(workers.OrderBy(c => c.Id).AsNoTracking(), pageIndex ?? 1, pageSize))
                          : Problem("Entity set 'AddressManagerContext.Worker'  is null.");
        }

        // GET: Workers
        public async Task<IActionResult> DelList(string? columnName, string currentFilter, string searchString, int? pageIndex)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0)
            {
                return Redirect("/");
            }

            var pageSize = Configuration.GetValue("PageSize", 20);
            ViewBag.pagePerView = Configuration.GetValue("pagePerView", 20);

            // 검색조건 콤보박스
            var excludedColumns = new List<string> { "Id", "IsDelete" };
            var columns = _context.Model.FindEntityType(typeof(Worker)).GetProperties()
                .Where(p => !excludedColumns.Contains(p.GetColumnName()))
                .Select(p => p.GetColumnName()) // 컬럼명 가져오기
                .ToList();

            if (columnName != null)
            {
                ViewBag.Columns = new SelectList(columns, columnName);
            }
            else
            {
                ViewBag.Columns = new SelectList(columns);
            }

            // 검색어가 있으면
            if (!searchString.IsNullOrEmpty())
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            @ViewData["CurrentColumnFilter"] = columnName;
            ViewData["CurrentFilter"] = searchString;

            var workers = _context.Worker.Include(w => w.Company)
                                                    .Where(w => w.IsDelete == "Y");

            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "CompanyId":
                        workers = workers.Where(s => s.Company.Name.Contains(searchString));
                        break;
                    case "Name":
                        workers = workers.Where(s => s.Name.Contains(searchString));
                        break;
                    case "Email":
                        workers = workers.Where(s => s.Email.Contains(searchString));
                        break;
                    case "Phone":
                        workers = workers.Where(s => s.Phone.Contains(searchString));
                        break;
                }
                _logger.LogInformation($"In Deleted Workers Page - search condition = {columnName}, search word = {searchString}");
            }

            return _context.Worker != null ?
                          View(await Pagination<Worker>
                          .CreateAsync(workers.OrderBy(c => c.Id).AsNoTracking(), pageIndex ?? 1, pageSize))
                          : Problem("Entity set 'AddressManagerContext.Worker'  is null.");
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0)
            {
                return Redirect("/");
            }

            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Company)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (worker == null)
            {
                return NotFound();
            }
           var change = await _context.ChangeHistory.Where(h => h.WorkerId == id).ToListAsync() ;
            var tuple = (Worker: worker, ChangeHistory:change);
            return View(tuple);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0)
            {
                return Redirect("/");
            }

            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name");
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,Name,Email,Phone, Company")] Worker worker)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (userId != 0 && userLoginId != null)
            {
                var p1 = new SqlParameter("@CompanyId", worker.CompanyId);
                var p2 = new SqlParameter("@Name", worker.Name);
                var p3 = new SqlParameter("@Email", worker.Email);
                var p4 = new SqlParameter("@Phone", worker.Phone);
                var p5 = new SqlParameter("@UserId", userId);
                var p6 = new SqlParameter("@LoginId", userLoginId);
                var p7 = new SqlParameter("@ActIP", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);

                string query = @"EXEC CreateWorker @CompanyId, @Name, @Email, @Phone, @UserId, @LoginId, @ActIP";
                await _context.Database.ExecuteSqlRawAsync(query, p1, p2, p3, p4, p5, p6, p7);

                _logger.Log(LogLevel.Information,
                    $"delete company => UserId = {p5.SqlValue}, UserLoginId = {p6.SqlValue}, " +
                    $"target = Worker, CompanyId ={p1.SqlValue}, WorkerName = {p2.SqlValue} IP = {p7.SqlValue}, time = {DateTime.Now}");

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Address", worker.CompanyId);
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", worker.CompanyId);
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Email,Phone,IsDelete")] Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Address", worker.CompanyId);
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (_context.Worker == null)
            {
                return Problem("Entity set 'AddressManagerContext.Worker'  is null.");
            }
            else if (userId == 0 || userLoginId == null)
            {
                return Problem("login user information doesn't exist.");
            }

            var p1 = new SqlParameter("@id", id);
            var p2 = new SqlParameter("@DorR", "Y");
            var p3 = new SqlParameter("@UserId", userId);
            var p4 = new SqlParameter("@LoginId", userLoginId);
            var p5 = new SqlParameter("@ActIP", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            var p6 = new SqlParameter("@Act", "D");

            string query = @"EXEC DorRWorker @id, @DorR, @UserId, @LoginId, @ActIP, @Act";
            await _context.Database.ExecuteSqlRawAsync(query, p1, p2, p3, p4, p5, p6);
            _logger.Log(LogLevel.Information,
                    $"delete Worker => UserId = {p3.SqlValue}, UserLoginId = {p4.SqlValue}, " +
                    $"WorkerId ={id}, IP = {p5.SqlValue}, time = {DateTime.Now}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Workers/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            string? userLoginId = HttpContext.Session.GetString("userLoginId");

            if (_context.Worker == null)
            {
                return Problem("Entity set 'AddressManagerContext.Worker'  is null.");
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

            string query = @"EXEC DorRWorker @id, @DorR, @UserId, @LoginId, @ActIP, @Act";
            var isSuccess = await _context.Database.ExecuteSqlRawAsync(query, p1, p2, p3, p4, p5, p6);
            
            _logger.LogInformation($"isSuccess = {isSuccess} ///// {query}");
            if (isSuccess == 0)
            {
                _logger.Log(LogLevel.Information,
                        $"restore Worker => UserId = {p3.SqlValue}, UserLoginId = {p4.SqlValue}, " +
                        $"WorkerId ={id}, IP = {p5.SqlValue}, time = {DateTime.Now}");
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.Log(LogLevel.Information,
                        $"restore Worker is failed => UserId = {p3.SqlValue}, UserLoginId = {p4.SqlValue}, " +
                        $"WorkerId ={id}, IP = {p5.SqlValue}, time = {DateTime.Now}");
            }
            return RedirectToAction(nameof(DelList));
        }

        private bool WorkerExists(int id)
        {
          return (_context.Worker?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
