﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AddressManager.Data;
using AddressManager.Models;
using Microsoft.Data.SqlClient;
using AddressManager.Models.Pages;
using Microsoft.IdentityModel.Tokens;
using AspectCore.DependencyInjection;

namespace AddressManager.Controllers
{
    
    public class CompaniesController : Controller
    {
        private readonly AddressManagerContext _context;
        private readonly ILogger<CompaniesController> _logger;
        private readonly IConfiguration Configuration;

        public CompaniesController(AddressManagerContext context, ILogger<CompaniesController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            Configuration = configuration;
        }

        // GET: Companies
        public async Task<IActionResult> Index(string columnName, string currentFilter, string searchString, int? pageIndex)
        {
            _logger.LogInformation("");
            // 페이징
            var pageSize = Configuration.GetValue("PageSize", 20);
            ViewBag.pagePerView = Configuration.GetValue("pagePerView", 20);

            // 검색조건 콤보박스
            var excludedColumns = new List<string> { "Id", "IsDelete" };
            var columns = _context.Model.FindEntityType(typeof(Company)).GetProperties()
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

            ViewData["CurrentColumnFilter"] = columnName;
            ViewData["CurrentFilter"] = searchString;

            var companies = _context.Company.Where(c => c.IsDelete == "N");

            // 검색조건과 검색어를 리스트에 반영
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "Address":
                        companies = companies.Where(s => s.Address.Contains(searchString));
                        break;
                    case "Contact":
                        companies = companies.Where(s => s.Contact.Contains(searchString));
                        break;
                    case "Name":
                        companies = companies.Where(s => s.Name.Contains(searchString));
                        break;
                }
                _logger.LogInformation($"In Company List Page - search condition = {columnName}, search word = {searchString}");
            }

            return _context.Company != null ?
                          View(await Pagination<Company>
                          .CreateAsync(companies.OrderBy(c => c.Name).AsNoTracking(), pageIndex ?? 1, pageSize))
                          : Problem("Entity set 'AddressManagerContext.Company'  is null.");
        }

        // GET: Companies
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
            var columns = _context.Model.FindEntityType(typeof(Company)).GetProperties()
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

            ViewData["CurrentColumnFilter"] = columnName;
            ViewData["CurrentFilter"] = searchString;

            var companies = _context.Company.Where(c => c.IsDelete == "Y");

            // 검색조건과 검색어를 리스트에 반영
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "Address":
                        companies = companies.Where(s => s.Address.Contains(searchString));
                        break;
                    case "Contact":
                        companies = companies.Where(s => s.Contact.Contains(searchString));
                        break;
                    case "Name":
                        companies = companies.Where(s => s.Name.Contains(searchString));
                        break;
                }
                _logger.LogInformation($"In Deleted Companies Page - search condition = {columnName}, search word = {searchString}");
            }

            return _context.Company != null ?
                          View(await Pagination<Company>
                          .CreateAsync(companies.OrderBy(c => c.Id).AsNoTracking(), pageIndex ?? 1, pageSize))
                          : Problem("Entity set 'AddressManagerContext.Company'  is null.");
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0)
            {
                return Redirect("/");
            }

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
            var change = await _context.ChangeHistory
                                    .Where(h => h.CompanyId == id)
                                    .ToListAsync();
            change.ForEach(ch => {
                ch.Act = ch.Act == "D" ? "Delete" : ch.Act == "C" ? "Create" : ch.Act == "R" ? "Restore" : ch.Act;
                });                        
            var tuple = (Company: company, ChangeHistory: change);

            return View(tuple);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if(userId == 0)
            {
                return Redirect("/");
            }
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

            if (userId == 0)
            {
                return Redirect("/");
            }

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
            return RedirectToAction(nameof(DelList));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Company?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
