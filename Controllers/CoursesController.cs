using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZuHuanJingDemo2.Data;
using ZuHuanJingDemo2.Models;
using System.Data;

namespace ZuHuanJingDemo2.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ZuHuanJingDemo2Context _context;
        private readonly IConfiguration _configuration;

        public CoursesController(ZuHuanJingDemo2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region ========================================================= Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region ========================================================= Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string selectQuery = "SELECT * FROM `Course` WHERE Course_Id = @CourseId";

                using MySqlCommand command = new MySqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@CourseId", id);

                try
                {
                    using MySqlDataReader reader = command.ExecuteReader();
                    Course? course = null;
                    while (reader.Read())
                    {
                        if (course == null)
                        {
                            int courseId = reader.GetInt32("Course_Id");
                            string courseName = reader.GetString("Course_Name");
                            string courseTeacher = reader.GetString("Course_Teacher");
                            string courseIntroduction = reader.GetString("Course_Introduction");
                            int courseMaxCount = reader.GetInt32("Course_MaxCount");
                            int courseSumCount = reader.GetInt32("Course_SumCount");
                            DateTime courseStartDate = reader.GetDateTime("Course_StartDate");
                            DateTime courseEndDate = reader.GetDateTime("Course_EndDate");
                            DateTime createDate = reader.GetDateTime("Course_CreateDate");
                            int courseIsActive = reader.GetInt32("Course_IsActive");

                            course = new Course()
                            {
                                Course_Id = courseId,
                                Course_Name = courseName,
                                Course_Teacher = courseTeacher,
                                Course_Introduction = courseIntroduction,
                                Course_MaxCount = courseMaxCount,
                                Course_SumCount = courseSumCount,
                                Course_StartDate = courseStartDate,
                                Course_EndDate = courseEndDate,
                                Course_CreateDate = createDate,
                                Course_IsActive = courseIsActive
                            };
                        }
                    }
                    if (course == null)
                    {
                        return NotFound();
                    }

                    return View(course);
                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }
        #endregion

        #region ========================================================= Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Course_Name, Course_Teacher, Course_Introduction, Course_MaxCount, Course_SumCount, Course_StartDate, Course_EndDate, Course_CreateDate, Course_IsActive")] Course course, string CourseIntroduction)
        {
            int maxid = 0;
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                string selectQuery = "SELECT MAX(`Course_Id`) AS MaxId FROM `course`";
                using MySqlCommand selectcommand = new MySqlCommand(selectQuery, connection);
                using MySqlDataReader reader = selectcommand.ExecuteReader();
                if (reader.Read())
                {
                    if (!reader.IsDBNull("MaxId"))
                    {
                        maxid = reader.GetInt32("MaxId");
                    }
                }
                reader.Close();
                if (maxid != 0)
                {
                    maxid++;
                }

                string insertQuery = "INSERT INTO `Course` (Course_Id, Course_Name, Course_Teacher, Course_Introduction, Course_MaxCount, Course_SumCount, Course_StartDate, Course_EndDate, Course_CreateDate, Course_IsActive) " +
                                     "VALUES (@Course_Id, @Course_Name, @Course_Teacher, @Course_Introduction, @Course_MaxCount, @Course_SumCount, @Course_StartDate, @Course_EndDate, @Course_CreateDate, @Course_IsActive)";

                using MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Course_Id", maxid);
                command.Parameters.AddWithValue("@Course_Name", course.Course_Name);
                command.Parameters.AddWithValue("@Course_Teacher", course.Course_Teacher);
                command.Parameters.AddWithValue("@Course_Introduction", course.Course_Introduction ?? "Fuck you");
                command.Parameters.AddWithValue("@Course_MaxCount", course.Course_MaxCount);
                command.Parameters.AddWithValue("@Course_SumCount", course.Course_SumCount);
                command.Parameters.AddWithValue("@Course_StartDate", course.Course_StartDate);
                command.Parameters.AddWithValue("@Course_EndDate", course.Course_EndDate);
                command.Parameters.AddWithValue("@Course_CreateDate", DateTime.Now);
                command.Parameters.AddWithValue("@Course_IsActive", course.Course_IsActive);

                command.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }
        #endregion

        #region ========================================================= Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Course_Id,Course_Name,Course_Teacher,Course_Introduction,Course_MaxCount,Course_SumCount,Course_StartDate,Course_EndDate,Course_CreateDate,Course_IsActive")] Course course)
        {
            if (id != course.Course_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Course_Id))
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
            return View(course);
        }
        #endregion

        #region ========================================================= Delete
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null) { return NotFound(); }
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // 删除 Course 数据
                string deleteMemberLicenseQuery = "DELETE FROM `Course` WHERE `Course_Id` = @CourseId";
                using MySqlCommand deleteMemberLicenseCommand = new(deleteMemberLicenseQuery, connection);
                deleteMemberLicenseCommand.Parameters.AddWithValue("@CourseId", id);
                await deleteMemberLicenseCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
            return RedirectToAction("Index");
        }



        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Course == null)
        //    {
        //        return NotFound();
        //    }

        //    var course = await _context.Course
        //        .FirstOrDefaultAsync(m => m.Course_Id == id);
        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(course);
        //}

        //// POST: Courses/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Course == null)
        //    {
        //        return Problem("Entity set 'ZuHuanJingDemo2Context.Course'  is null.");
        //    }
        //    var course = await _context.Course.FindAsync(id);
        //    if (course != null)
        //    {
        //        _context.Course.Remove(course);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion


        private bool CourseExists(int id)
        {
            return (_context.Course?.Any(e => e.Course_Id == id)).GetValueOrDefault();
        }
    }
}
