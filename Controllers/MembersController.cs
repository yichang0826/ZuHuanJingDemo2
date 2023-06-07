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
using ZuHuanJingDemo2.Models.ViewModel;
using System.Data;
using System.ComponentModel;

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

        #region ======================================================================== Index
        public IActionResult Index()
        {
            return View();
        }

        //[Route("[controller]")]
        //[HttpGet("GetMembers")]
        //public IActionResult GetMembers()
        //{
        //    string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
        //    List<Member> users = new();
        //    try
        //    {
        //        using MySqlConnection connection = new(connectionString);
        //        connection.Open();
        //        string selectQuery = "SELECT * FROM `member`";
        //        using MySqlCommand command = new(selectQuery, connection);
        //        using MySqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
        //            {
        //                int id = reader.GetInt32("Member_Id");
        //                string name = reader.GetString("Member_Name");
        //                string account = reader.GetString("Member_Account");
        //                string password = reader.GetString("Member_Password");
        //                string email = reader.GetString("Member_Email");
        //                int isbaned = reader.GetInt32("Is_Baned");
        //                DateTime createdate = reader.GetDateTime("Member_CreateDate");
        //                Member user = new()
        //                {
        //                    Member_Id = id,
        //                    Member_Name = name,
        //                    Member_Account = account,
        //                    Member_Password = password,
        //                    Member_Email = email,
        //                    Is_Baned = isbaned,
        //                    Member_CreateDate = createdate
        //                };
        //                users.Add(user);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = $"出現錯誤：{ex.Message}" });
        //    }
        //    return Ok(users);
        //}
        #endregion

        #region ======================================================================== Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string selectQuery = "SELECT m.*, ml.CreatedDate, l.* " +
                             "FROM `Member` m " +
                             "LEFT JOIN `MemberLicense` ml ON m.Member_Id = ml.MemberId " +
                             "LEFT JOIN `License` l ON ml.LicenseId = l.License_Id " +
                             "WHERE m.Member_Id = @MemberId";

                using MySqlCommand command = new(selectQuery, connection);
                command.Parameters.AddWithValue("@MemberId", id);

                try
                {
                    using MySqlDataReader reader = command.ExecuteReader();
                    Member? member = null;
                    while (reader.Read())
                    {
                        if (member == null)
                        {
                            int memberId = reader.GetInt32("Member_Id");
                            string memberName = reader.GetString("Member_Name");
                            string memberAccount = reader.GetString("Member_Account");
                            string memberPassword = reader.GetString("Member_Password");
                            string memberEmail = reader.GetString("Member_Email");
                            int isBaned = reader.GetInt32("Is_Baned");
                            DateTime createDate = reader.GetDateTime("Member_CreateDate");

                            member = new Member()
                            {
                                Member_Id = memberId,
                                Member_Name = memberName,
                                Member_Account = memberAccount,
                                Member_Password = memberPassword,
                                Member_Email = memberEmail,
                                Is_Baned = isBaned,
                                Member_CreateDate = createDate,
                                Member_Licenses = new()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("License_Id")))
                        {
                            int licenseId = reader.GetInt32("License_Id");
                            string licenseName = reader.GetString("License_Name");
                            string licenseIntroduction = reader.GetString("License_Introduction");
                            DateTime mlicenseCreateDate = reader.GetDateTime("CreatedDate");

                            Models.License license = new()
                            {
                                License_Id = licenseId,
                                License_Name = licenseName,
                                License_Introduction = licenseIntroduction,
                                License_CreateDate = mlicenseCreateDate
                            };

                            member.Member_Licenses.Add(license);
                        }
                    }
                    if (member == null)
                    {
                        return NotFound();
                    }

                    return View(member);
                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤1：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }
        #endregion

        #region ======================================================================== Create
        public IActionResult Create()
        {
            List<Models.License> licenseslist = new();
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            try
            {
                #region
                //using MySqlConnection connection = new(connectionString);
                //connection.Open();
                //string selectQuery = "SELECT * FROM `license` WHERE `License_IsActive` = 1";
                //using MySqlCommand command = new(selectQuery, connection);
                //using MySqlDataReader reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                //    {
                //        int id = reader.GetInt32("License_Id");
                //        string name = reader.GetString("License_Name");
                //        string introduction = reader.GetString("License_Introduction");
                //        DateTime createdate = reader.GetDateTime("License_CreateDate");
                //        int isactive = reader.GetInt32("License_IsActive");

                //        License license = new License()
                //        {
                //            License_Id = id,
                //            License_Name = name,
                //            License_Introduction = introduction,
                //            License_IsActive = isactive == 1,
                //            License_CreateDate = createdate
                //        };
                //        licenseslist.Add(license);
                //    }
                //}
                #endregion
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string selectQuery = "SELECT `License_Id`,`License_Name` FROM `license` WHERE `License_IsActive` = 1";
                using MySqlCommand command = new(selectQuery, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                    {
                        int id = reader.GetInt32("License_Id");
                        string name = reader.GetString("License_Name");

                        Models.License license = new()
                        {
                            License_Id = id,
                            License_Name = name
                        };
                        licenseslist.Add(license);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
            ViewData["licenseslist"] = licenseslist;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Member_Name,Member_Account,Member_Password,Member_Email,Is_Baned")] Member member, int[] selectedLicenses)
        {
            int maxid = 0;
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                string selectQuery = "SELECT MAX(`Member_Id`) AS MaxId FROM `member`";
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
                try
                {
                    string insertQuery = "INSERT INTO `Member` (Member_Id, Member_Name, Member_Account, Member_Password, Member_Email, Is_Baned, Member_CreateDate) " +
                                         "VALUES (@Member_Id,  @Member_Name, @Member_Account, @Member_Password, @Member_Email, @Is_Baned, @Member_CreateDate)";
                    using MySqlCommand command = new(insertQuery, connection);
                    command.Parameters.AddWithValue("@Member_Id", maxid);
                    command.Parameters.AddWithValue("@Member_Name", member.Member_Name);
                    command.Parameters.AddWithValue("@Member_Account", member.Member_Account);
                    command.Parameters.AddWithValue("@Member_Password", member.Member_Password);
                    command.Parameters.AddWithValue("@Member_Email", member.Member_Email);
                    command.Parameters.AddWithValue("@Is_Baned", member.Is_Baned);
                    command.Parameters.AddWithValue("@Member_CreateDate", DateTime.Now);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }

                if (selectedLicenses != null)
                {
                    try
                    {
                        foreach (var licenseId in selectedLicenses)
                        {
                            Console.WriteLine(licenseId);
                            string insertQuery = "INSERT INTO `memberlicense` (MemberId, LicenseId, CreatedDate) " +
                                             "VALUES ( @MemberId, @LicenseId, @CreatedDate)";
                            using MySqlCommand command = new(insertQuery, connection);
                            command.Parameters.AddWithValue("@MemberId", maxid);
                            command.Parameters.AddWithValue("@LicenseId", licenseId);
                            command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Text"] = $"出現錯誤：{ex.Message}";
                        TempData["Detail"] = maxid;
                        return View("~/Views/Home/ErrorView.cshtml");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }
        #endregion

        #region ======================================================================== Edit
        public IActionResult Edit(int? Id)
        {
            MemberEditViewModel model = new MemberEditViewModel();
            if (Id == null)
            {
                return NotFound();
            }

            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                #region reading the license list
                string selectQuery = "SELECT * FROM `license`";
                using MySqlCommand selectcommand = new MySqlCommand(selectQuery, connection);
                using MySqlDataReader reader = selectcommand.ExecuteReader();
                model.licenses = new();
                while (reader.Read())
                {
                    int id = reader.GetInt32("License_Id");
                    string name = reader.GetString("License_Name");
                    string introduction = reader.GetString("License_Introduction");
                    DateTime createdate = reader.GetDateTime("License_CreateDate");
                    int isactive = reader.GetInt32("License_IsActive");
                    Models.License license = new()
                    {
                        License_Id = id,
                        License_Name = name,
                        License_Introduction = introduction,
                        License_IsActive = isactive == 1,
                        License_CreateDate = createdate
                    };
                    model.licenses.Add(license);
                }
                reader.Close();
                #endregion
                #region reading the member file
                selectQuery = "SELECT m.*, ml.CreatedDate, l.* " +
                             "FROM `Member` m " +
                             "LEFT JOIN `MemberLicense` ml ON m.Member_Id = ml.MemberId " +
                             "LEFT JOIN `License` l ON ml.LicenseId = l.License_Id " +
                             "WHERE m.Member_Id = @MemberId";

                using MySqlCommand command = new(selectQuery, connection);
                command.Parameters.AddWithValue("@MemberId", Id);

                try
                {
                    using MySqlDataReader reader1 = command.ExecuteReader();
                    Member? member = null;
                    while (reader1.Read())
                    {
                        if (member == null)
                        {
                            int memberId = reader1.GetInt32("Member_Id");
                            string memberName = reader1.GetString("Member_Name");
                            string memberAccount = reader1.GetString("Member_Account");
                            string memberPassword = reader1.GetString("Member_Password");
                            string memberEmail = reader1.GetString("Member_Email");
                            int isBaned = reader1.GetInt32("Is_Baned");
                            DateTime createDate = reader1.GetDateTime("Member_CreateDate");

                            member = new Member()
                            {
                                Member_Id = memberId,
                                Member_Name = memberName,
                                Member_Account = memberAccount,
                                Member_Password = memberPassword,
                                Member_Email = memberEmail,
                                Is_Baned = isBaned,
                                Member_CreateDate = createDate,
                                Member_Licenses = new()
                            };
                        }
                        if (!reader1.IsDBNull(reader1.GetOrdinal("License_Id")))
                        {
                            int licenseId = reader1.GetInt32("License_Id");
                            string licenseName = reader1.GetString("License_Name");
                            string licenseIntroduction = reader1.GetString("License_Introduction");
                            DateTime mlicenseCreateDate = reader1.GetDateTime("CreatedDate");
                            Models.License license = new()
                            {
                                License_Id = licenseId,
                                License_Name = licenseName,
                                License_Introduction = licenseIntroduction,
                                License_CreateDate = mlicenseCreateDate
                            };
                            member.Member_Licenses.Add(license);
                        }
                        model.member = member;
                    }
                    if (member == null)
                    {
                        return NotFound();
                    }
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Member_Id,Member_Name,Member_Account,Member_Email,Is_Baned,Member_CreateDate")] Member member, int[] selectedLicenses)
        {
            if (id != member.Member_Id)
            {
                TempData["Text"] = $"出現錯誤：{id} != {member.Member_Id}";
                return View("~/Views/Home/ErrorView.cshtml");
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                List<int> existingLicenses = new();
                try
                {
                    using MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();

                    string selectLicensesQuery = "SELECT `LicenseId` FROM `memberlicense` WHERE `MemberId` = @MemberId";
                    using MySqlCommand selectLicensesCommand = new MySqlCommand(selectLicensesQuery, connection);
                    selectLicensesCommand.Parameters.AddWithValue("@MemberId", id);
                    using MySqlDataReader licensesReader = selectLicensesCommand.ExecuteReader();
                    while (licensesReader.Read())
                    {
                        int licenseId = licensesReader.GetInt32("LicenseId");
                        existingLicenses.Add(licenseId);
                    }
                    licensesReader.Close();

                    List<int> licensesToDelete = existingLicenses.Except(selectedLicenses).ToList();
                    List<int> licensesToAdd = selectedLicenses.Except(existingLicenses).ToList();

                    // 删除已取消选择的许可证
                    foreach (int licenseId in licensesToDelete)
                    {
                        string deleteLicenseQuery = "DELETE FROM `memberlicense` WHERE (`MemberId` = @MemberId AND `LicenseId` = @LicenseId)";
                        using MySqlCommand deleteLicenseCommand = new MySqlCommand(deleteLicenseQuery, connection);
                        deleteLicenseCommand.Parameters.AddWithValue("@MemberId", id);
                        deleteLicenseCommand.Parameters.AddWithValue("@LicenseId", licenseId);
                        deleteLicenseCommand.ExecuteNonQuery();
                    }

                    // 添加新选择的许可证
                    foreach (int licenseId in licensesToAdd)
                    {
                        string insertLicenseQuery = "INSERT INTO `memberlicense` (MemberId, LicenseId, CreatedDate) VALUES (@MemberId, @LicenseId, @CreatedDate)";
                        using MySqlCommand insertLicenseCommand = new MySqlCommand(insertLicenseQuery, connection);
                        insertLicenseCommand.Parameters.AddWithValue("@MemberId", id);
                        insertLicenseCommand.Parameters.AddWithValue("@LicenseId", licenseId);
                        insertLicenseCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        insertLicenseCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    TempData["Text"] = id + " " + ex;
                    //TempData["Text"] = $"出現錯誤：{ex.Message}";
                    return View("~/Views/Home/ErrorView.cshtml");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
        }
        #endregion

        #region ======================================================================== Delete
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null) { return NotFound(); }

            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // 删除 MemberLicense 数据
                string deleteMemberLicenseQuery = "DELETE FROM `MemberLicense` WHERE `MemberId` = @MemberId";
                using MySqlCommand deleteMemberLicenseCommand = new(deleteMemberLicenseQuery, connection);
                deleteMemberLicenseCommand.Parameters.AddWithValue("@MemberId", id);
                await deleteMemberLicenseCommand.ExecuteNonQueryAsync();

                // 删除 Member 数据
                string deleteMemberQuery = "DELETE FROM `Member` WHERE `Member_Id` = @MemberId";
                using MySqlCommand deleteMemberCommand = new(deleteMemberQuery, connection);
                deleteMemberCommand.Parameters.AddWithValue("@MemberId", id);
                await deleteMemberCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                TempData["Text"] = $"出現錯誤：{ex.Message}";
                return View("~/Views/Home/ErrorView.cshtml");
            }
            return RedirectToAction("Index");
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Member == null)
        //    {
        //        return Problem("Entity set 'ZuHuanJingDemo2Context.Member'  is null.");
        //    }
        //    var member = await _context.Member.FindAsync(id);
        //    if (member != null)
        //    {
        //        _context.Member.Remove(member);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion

        public IActionResult Search(string? query, string sortField, string sortFun)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
                List<Member> members = new();

                using MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                string selectQuery = "SELECT * FROM `Member` WHERE `Member_Name` LIKE @Query OR `Member_Account` LIKE @Query OR `Member_Id` LIKE @Query ORDER BY " + sortField + " " + sortFun;

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
                return PartialView("_MemberList", members);
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return View();
            }
        }

        private bool MemberExists(int id)
        {
            return (_context.Member?.Any(e => e.Member_Id == id)).GetValueOrDefault();
        }
    }
}
