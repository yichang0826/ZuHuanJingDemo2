using System;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZuHuanJingDemo2.Data;
using ZuHuanJingDemo2.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ZuHuanJingDemo2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class LicensesController : Controller
    {
        private readonly ZuHuanJingDemo2Context _context;
        private readonly IConfiguration _configuration;
        public LicensesController(ZuHuanJingDemo2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region ======================================================================== Index
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            List<License> licenses = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string selectQuery = "SELECT * FROM `license` ";
                using MySqlCommand command = new(selectQuery, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                    {
                        int id = reader.GetInt32("License_Id");
                        string name = reader.GetString("License_Name");
                        string introduction = reader.GetString("License_Introduction");
                        DateTime createdate = reader.GetDateTime("License_CreateDate");
                        int isactive = reader.GetInt32("License_IsActive");

                        License license = new License()
                        {
                            License_Id = id,
                            License_Name = name,
                            License_Introduction = introduction,
                            License_IsActive = isactive == 1,
                            License_CreateDate = createdate
                        };
                        licenses.Add(license);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
            return View(licenses);
        }
        #endregion

        // GET: Licenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.License == null)
            {
                return NotFound();
            }

            var license = await _context.License
                .FirstOrDefaultAsync(m => m.License_Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // GET: Licenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("License_Id,License_Name,License_Introduction,License_CreateDate")] License license)
        {
            if (ModelState.IsValid)
            {
                _context.Add(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(license);
        }

        // GET: Licenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.License == null)
            {
                return NotFound();
            }

            var license = await _context.License.FindAsync(id);
            if (license == null)
            {
                return NotFound();
            }
            return View(license);
        }

        // POST: Licenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("License_Id,License_Name,License_Introduction,License_CreateDate")] License license)
        {
            if (id != license.License_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(license);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseExists(license.License_Id))
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
            return View(license);
        }

        // GET: Licenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.License == null)
            {
                return NotFound();
            }

            var license = await _context.License
                .FirstOrDefaultAsync(m => m.License_Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.License == null)
            {
                return Problem("Entity set 'ZuHuanJingDemo2Context.License'  is null.");
            }
            var license = await _context.License.FindAsync(id);
            if (license != null)
            {
                _context.License.Remove(license);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseExists(int id)
        {
          return (_context.License?.Any(e => e.License_Id == id)).GetValueOrDefault();
        }
    }
}
