﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace ZuHuanJingDemo2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
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
                        try
                        {
                            course = new Course()
                            {
                                Course_Id = reader.GetInt32("Course_Id"),
                                Course_Name = reader.GetString("Course_Name"),
                                Course_Teacher = reader.GetString("Course_Teacher"),
                                Course_Introduction = reader.GetString("Course_Introduction"),
                                Course_MaxCount = reader.GetInt32("Course_MaxCount"),
                                Course_SumCount = reader.GetInt32("Course_SumCount"),
                                Course_StartDate = reader.GetDateTime("Course_StartDate"),
                                Course_EndDate = reader.GetDateTime("Course_EndDate"),
                                Course_CreateDate = reader.GetDateTime("Course_CreateDate"),
                                Course_IsActive = reader.GetInt32("Course_IsActive")
                            };
                        }
                        catch { }
                    }
                    if (course == null)
                    {
                        return NotFound();
                    }
                    else { return View(course); }
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
            var model = new Course();
            return View(model);
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
        public IActionResult Edit(int? id)
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

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Course_Id,Course_Name,Course_Teacher,Course_Introduction,Course_MaxCount,Course_SumCount,Course_StartDate,Course_EndDate,Course_CreateDate,Course_IsActive")] Course course)
        {
            if (id != course.Course_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                    using MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    string updateQuery = "UPDATE `Course` SET `Course_Name` = @CourseName, `Course_Teacher` = @CourseTeacher, " +
                                         "`Course_Introduction` = @CourseIntroduction, `Course_MaxCount` = @CourseMaxCount, " +
                                         "`Course_SumCount` = @CourseSumCount, `Course_StartDate` = @CourseStartDate, " +
                                         "`Course_EndDate` = @CourseEndDate, `Course_CreateDate` = @CourseCreateDate, " +
                                         "`Course_IsActive` = @CourseIsActive WHERE `Course_Id` = @CourseId";

                    using MySqlCommand command = new MySqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@CourseId", course.Course_Id);
                    command.Parameters.AddWithValue("@CourseName", course.Course_Name);
                    command.Parameters.AddWithValue("@CourseTeacher", course.Course_Teacher);
                    command.Parameters.AddWithValue("@CourseIntroduction", course.Course_Introduction);
                    command.Parameters.AddWithValue("@CourseMaxCount", course.Course_MaxCount);
                    command.Parameters.AddWithValue("@CourseSumCount", course.Course_SumCount);
                    command.Parameters.AddWithValue("@CourseStartDate", course.Course_StartDate);
                    command.Parameters.AddWithValue("@CourseEndDate", course.Course_EndDate);
                    command.Parameters.AddWithValue("@CourseCreateDate", course.Course_CreateDate);
                    command.Parameters.AddWithValue("@CourseIsActive", course.Course_IsActive);
                    command.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
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


        public IActionResult GetCourseMember(int courseId)
        {
            List<Member> members = new List<Member>();
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            //string selectQuery = "SELECT m.Member_Id, m.Member_Name " +
            //        "FROM `member` m " +
            //        $"LEFT JOIN `CourseMember` cm `MemberId` ON cm.CourseId = @CourseId" +
            //        "WHERE m.Member_Id = cm.MemberId";
            string selectQuery = "SELECT m.Member_Id, m.Member_Name " +
            "FROM `member` m " +
            "LEFT JOIN `CourseMember` cm ON cm.MemberId = m.Member_Id AND cm.CourseId = @CourseId";
            using MySqlCommand command = new MySqlCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@CourseId", courseId);
            try
            {
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Member member = new Member()
                    {
                        Member_Id = reader.GetInt32("Member_Id"),
                        Member_Name = reader.GetString("Member_Name"),
                    };
                    members.Add(member);
                }
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
            return PartialView("~/Views/Courses/_CourseMemberList.cshtml", members);
        }

        private bool CourseExists(int id)
        {
            return (_context.Course?.Any(e => e.Course_Id == id)).GetValueOrDefault();
        }
    }
}
