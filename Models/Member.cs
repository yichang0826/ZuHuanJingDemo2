using System.ComponentModel.DataAnnotations;

namespace ZuHuanJingDemo2.Models
{
    public class Member
    {
        [Key] public int Member_Id { get; set; }
        [Display(Name = "名稱")][StringLength(255)][Required] public string? Member_Name { get; set; }
        [Display(Name = "帳號")][StringLength(255)][Required] public string? Member_Account { get; set; }
        [Display(Name = "密碼")][StringLength(255)][DataType("password")][Required] public string? Member_Password { get; set; }
        [Display(Name = "郵箱")][StringLength(255)][Required] public string? Member_Email { get; set; }
        [Display(Name = "封鎖")] public int Is_Baned { get; set; } = 0;
        [Display(Name = "日期")] public DateTime Member_CreateDate { get; set; } = DateTime.Now;
    }
}
