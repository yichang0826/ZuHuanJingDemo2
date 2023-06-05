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
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            List<Course> courses = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string selectQuery = "SELECT * FROM `course`";
                using MySqlCommand command = new(selectQuery, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                    {
                        int courseid = reader.GetInt32("Course_Id");
                        string name = reader.GetString("Course_Name");
                        string teacher = reader.GetString("Course_Teacher");
                        string introduction = reader.GetString("Course_Introduction");
                        int maxcount = reader.GetInt32("Course_MaxCount");
                        int sumcount = reader.GetInt32("Course_SumCount");
                        DateTime starttime = reader.GetDateTime("Course_StartDate");
                        DateTime enddate = reader.GetDateTime("Course_EndDate");
                        DateTime createdate = reader.GetDateTime("Course_CreateDate");
                        int isactive = reader.GetInt32("Course_IsActive");
                        Course course = new()
                        {
                            Course_Id = courseid,
                            Course_Name = name,
                            Course_Teacher = teacher,
                            Course_Introduction = introduction,
                            Course_MaxCount = maxcount,
                            Course_SumCount = sumcount,
                            Course_StartDate = starttime,
                            Course_EndDate = enddate,
                            Course_CreateDate = createdate,
                            Course_IsActive = isactive
                        };
                        courses.Add(course);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return View();
            }

            return View(courses);
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
                    ViewBag.Text = $"出現錯誤2：{ex.Message}";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤1：{ex.Message}";
                return View();
            }
        }
        #endregion


        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Course_Name,Course_Teacher,Course_Introduction,Course_MaxCount,Course_SumCount,Course_StartDate,Course_EndDate,Course_CreateDate,Course_IsActive")] Course course)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");

                try
                {
                    using MySqlConnection connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();

                    string insertQuery = "INSERT INTO `Course` (Course_Name, Course_Teacher, Course_Introduction, Course_MaxCount, Course_SumCount, Course_StartDate, Course_EndDate, Course_CreateDate, Course_IsActive) " +
                                         "VALUES (@Course_Name, @Course_Teacher, @Course_Introduction, @Course_MaxCount, @Course_SumCount, @Course_StartDate, @Course_EndDate, @Course_CreateDate, @Course_IsActive)";

                    using MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@Course_Name", course.Course_Name);
                    command.Parameters.AddWithValue("@Course_Teacher", course.Course_Teacher);
                    command.Parameters.AddWithValue("@Course_Introduction", course.Course_Introduction);
                    command.Parameters.AddWithValue("@Course_MaxCount", course.Course_MaxCount);
                    command.Parameters.AddWithValue("@Course_SumCount", course.Course_SumCount);
                    command.Parameters.AddWithValue("@Course_StartDate", course.Course_StartDate);
                    command.Parameters.AddWithValue("@Course_EndDate", course.Course_EndDate);
                    command.Parameters.AddWithValue("@Course_CreateDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Course_IsActive", course.Course_IsActive);

                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    // Handle exception
                    return View(course);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }


        // GET: Courses/Edit/5
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

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Course_Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Course == null)
            {
                return Problem("Entity set 'ZuHuanJingDemo2Context.Course'  is null.");
            }
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string? query, string sortField, string sortFun)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                List<Course> courses = new();

                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                string selectQuery = "SELECT * FROM `Course` WHERE `Course_Name` LIKE @Query OR `Course_Teacher` LIKE @Query OR `Course_Id` LIKE @Query ORDER BY " + sortField + " " + sortFun;

                using MySqlCommand command = new(selectQuery, connection);
                command.Parameters.AddWithValue("@Query", $"%{query}%");

                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
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
                    

                    Course course = new Course()
                    {
                        Course_Id = courseId,
                        Course_Name = courseName,
                        Course_Teacher = courseTeacher,
                        Course_MaxCount = courseMaxCount,
                        Course_SumCount = courseSumCount,
                        Course_StartDate = courseStartDate,
                        Course_EndDate = courseEndDate,
                        Course_CreateDate = createDate,
                        Course_IsActive = courseIsActive
                        
                    };
                    courses.Add(course);
                }
                return PartialView("_CourseList", courses);
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return View();
            }
        }

        private bool CourseExists(int id)
        {
          return (_context.Course?.Any(e => e.Course_Id == id)).GetValueOrDefault();
        }
    }
}
