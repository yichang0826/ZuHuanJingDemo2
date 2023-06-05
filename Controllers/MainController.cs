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
    public class MainController : Controller
    {
        private readonly ZuHuanJingDemo2Context _context;
        private readonly IConfiguration _configuration;

        public MainController(ZuHuanJingDemo2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Search(string? query, string sortField, string sortFun)
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
                    command.Parameters.AddWithValue("@Query", $"%{query}%");

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
                else if (sortField.Contains("Course"))
                {
                    selectQuery = "SELECT * FROM `Course` WHERE `Course_Name` LIKE @Query OR `Course_Teacher` LIKE @Query OR `Course_Id` LIKE @Query ORDER BY " + sortField + " " + sortFun;
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
                    return PartialView("~/Views/Members/_CourseList.cshtml", courses);
                }
                TempData["Text"] = sortField + "  Member Equals: " + sortField.Equals("Member");
                return RedirectToAction("ErrorView", "Home");
            }
            catch (Exception ex)
            {
                TempData["Text"] = ex;
                return RedirectToAction("ErrorView", "Home");
            }
        }
    }
}
