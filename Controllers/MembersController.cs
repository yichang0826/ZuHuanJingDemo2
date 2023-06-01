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

namespace ZuHuanJingDemo2.Controllers
{
    public class MembersController : Controller
    {
        private readonly ZuHuanJingDemo2Context _context;
        private readonly IConfiguration _configuration;

        public MembersController(ZuHuanJingDemo2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Members
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            List<Member> users = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string selectQuery = "SELECT * FROM `member`";
                using MySqlCommand command = new(selectQuery, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                    {
                        int id = reader.GetInt32("Member_Id");
                        string name = reader.GetString("Member_Name");
                        string account = reader.GetString("Member_Account");
                        string password = reader.GetString("Member_Password");
                        string email = reader.GetString("Member_Email");
                        int isbaned = reader.GetInt32("Is_Baned");
                        DateTime createdate = reader.GetDateTime("Member_CreateDate");
                        Member user = new()
                        {
                            Member_Id = id,
                            Member_Name = name,
                            Member_Account = account,
                            Member_Password = password,
                            Member_Email = email,
                            Is_Baned = isbaned,
                            Member_CreateDate = createdate
                        };
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return View();
            }

            return View(users);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Member_Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Member_Name,Member_Account,Member_Password,Member_Email,Is_Baned")] Member member)
        {
            DateTime datenow = DateTime.Now;
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                try
                {
                    using MySqlConnection connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();
                    string insertQuery = "INSERT INTO `Member` (Member_Name, Member_Account, Member_Password, Member_Email, Is_Baned, Member_CreateDate) " +
                                         "VALUES ( @Member_Name, @Member_Account, @Member_Password, @Member_Email, @Is_Baned, @Member_CreateDate)";
                    using MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@Member_Name", member.Member_Name);
                    command.Parameters.AddWithValue("@Member_Account", member.Member_Account);
                    command.Parameters.AddWithValue("@Member_Password", member.Member_Password);
                    command.Parameters.AddWithValue("@Member_Email", member.Member_Email);
                    command.Parameters.AddWithValue("@Is_Baned", member.Is_Baned);
                    command.Parameters.AddWithValue("@Member_CreateDate", datenow);
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    ViewBag.Text = $"出現錯誤：{ex.Message}";
                    return View(member);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return View(member);
            }
        }


        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Member_Id,Member_Name,Member_Account,Member_Password,Member_Email,Is_Baned,Member_CreateDate")] Member member)
        {
            if (id != member.Member_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Member_Id))
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
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Member_Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Member == null)
            {
                return Problem("Entity set 'ZuHuanJingDemo2Context.Member'  is null.");
            }
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return (_context.Member?.Any(e => e.Member_Id == id)).GetValueOrDefault();
        }
    }
}
