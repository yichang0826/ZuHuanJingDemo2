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
using System.Data;

namespace ZuHuanJingDemo2.Controllers
{
    public class MainController : Controller
    {
        private readonly ZuHuanJingDemo2Context _context;
        private readonly IConfiguration _configuration;

        public MainController(ZuHuanJingDemo2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Course()
        {

            return View();
        }

        public IActionResult CourseDetails(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string selectQuery = "SELECT c.*, cm.CourseMember_CreateDate " +
                    "FROM `Course` c " +
                    $"LEFT JOIN `CourseMember` cm ON cm.CourseId = c.Course_Id AND cm.MemberId = {id} " +
                    "WHERE Course_Id = @CourseId";

                using MySqlCommand command = new MySqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@CourseId", id);

                try
                {
                    using MySqlDataReader reader = command.ExecuteReader();
                    Course? course = null;
                    while (reader.Read())
                    {
                        DateTime? coursememberCreateDate = reader.IsDBNull(reader.GetOrdinal("CourseMember_CreateDate")) ? null : reader.GetDateTime("CourseMember_CreateDate");
                        TempData["Status"] = coursememberCreateDate;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(int applyCourseId, int applyUserId)
        {
            int maxmid = 0;
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                try
                {
                    string insertQuery = "INSERT INTO `CourseMember` (CourseId, MemberId, CourseMember_CreateDate) VALUES (@CourseId, @MemberId, @CreateDate)";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", applyCourseId);
                        command.Parameters.AddWithValue("@MemberId", applyUserId);
                        command.Parameters.AddWithValue("@CreateDate", DateTime.Now);

                        int rowsAffected = command.ExecuteNonQuery();
                        // 檢查插入操作的影響行數，確保成功插入記錄
                        if (rowsAffected > 0)
                        {
                            
                            // 插入成功的處理
                        }
                        else
                        {
                            // 插入失敗的處理
                        }
                    }

                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
                return RedirectToAction("CourseDetails", new {id = applyCourseId });
                //return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }

        public IActionResult Search(string searchQuery, string sortField, string sortFun, string? sortDate, string? searching)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                List<Member> members = new();
                List<Course> courses = new();

                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                string selectQuery = "";

                //if (typeof(Member).Name.Equals(sortField))
                if (sortField.Contains("Member"))
                {
                    selectQuery = $"SELECT * FROM `Member` WHERE `Member_Name` LIKE @Query OR `Member_Account` LIKE @Query ORDER BY {sortField} {sortFun}";
                    //selectQuery = "SELECT * FROM `Member` WHERE `Member_Name` LIKE @Query OR `Member_Account` LIKE @Query ORDER BY " + sortField + " " +sortFun;

                    using MySqlCommand command = new MySqlCommand(selectQuery, connection);
                    command.Parameters.AddWithValue("@Query", $"%{searchQuery}%");

                    using MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int memberId = reader.GetInt32("Member_Id");
                        string memberName = reader.GetString("Member_Name");
                        string memberAccount = reader.GetString("Member_Account");
                        string memberEmail = reader.GetString("Member_Email");
                        int isBaned = reader.GetInt32("Is_Baned");
                        DateTime createDate = reader.GetDateTime("Member_CreateDate");

                        Member member = new Member()
                        {
                            Member_Id = memberId,
                            Member_Name = memberName,
                            Member_Account = memberAccount,
                            Member_Email = memberEmail,
                            Is_Baned = isBaned,
                            Member_CreateDate = createDate
                        };
                        members.Add(member);
                    }
                    return PartialView("~/Views/Members/_MemberList.cshtml", members);
                }
                else if (sortDate != null && sortField.Contains("Course"))
                {
                    selectQuery = "SELECT * FROM `Course` WHERE (`Course_Name` LIKE @searchQuery OR `Course_Teacher` LIKE @searchQuery OR `Course_Id` LIKE @searchQuery) ";
                    switch (sortDate)
                    {
                        case "all": break;
                        case "before": selectQuery += " AND `Course_StartDate` > CURDATE() "; break;
                        case "started": selectQuery += " AND `Course_StartDate` <= CURDATE() AND `Course_EndDate` >= CURDATE() "; break;
                        case "ended": selectQuery += " AND `Course_EndDate` < CURDATE() "; break;
                    }
                    selectQuery += "ORDER BY " + sortField + " " + sortFun + ";";

                    //TempData["Text"] = "  sortField: " + sortField + "  || SortFun: " + sortFun + "  || SortDate: " + sortDate + "  || selectQuery:  " + selectQuery;
                    //return RedirectToAction("ErrorView", "Home");

                    using MySqlCommand command = new(selectQuery, connection);
                    command.Parameters.AddWithValue("@searchQuery", $"%{searchQuery}%");

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
                    if (searching == null)
                    {
                        return PartialView("~/Views/Courses/_CourseList.cshtml", courses);
                    }
                    else if (searching == "Main")
                    {
                        return PartialView("~/Views/Main/_CourseList.cshtml", courses);
                    }
                }
                TempData["Text"] = "  sortField: " + sortField + "  || SortFun: " + sortFun + "  || SortDate: " + sortDate + "  || selectQuery:  " + selectQuery;
                return RedirectToAction("ErrorView", "Home");
            }
            catch (Exception ex)
            {
                TempData["Text"] = ex.Message;
                return RedirectToAction("ErrorView", "Home");
            }
        }
    }
}
