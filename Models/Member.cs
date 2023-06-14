using System.ComponentModel.DataAnnotations;

namespace ZuHuanJingDemo2.Models
{
    public class Member
    {
        [Display(Name = "編號")]
        [Key]
        public int Member_Id { get; set; }

        [Display(Name = "名稱")]
        [StringLength(255)]
        [Required(ErrorMessage = "名稱為必填項目")]
        public string? Member_Name { get; set; }

        [Required(ErrorMessage = "帳號為必填項目")]
        [RegularExpression(@"[A-Za-z0-9_]+", ErrorMessage = "帳號只能包含字母、數字和底線")]
        [Display(Name = "帳號")]
        [StringLength(255)]
        public string? Member_Account { get; set; }

        [Required(ErrorMessage = "密碼是必填項目")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "密碼只能包含字母和數字")]
        [Display(Name = "密碼")]
        [StringLength(255)]
        [DataType("password")]
        public string? Member_Password { get; set; }

        [Display(Name = "郵箱")]
        [StringLength(255)]
        [Required(ErrorMessage = "郵箱為必填項目")]
        public string? Member_Email { get; set; }

        [Display(Name = "腳色")]
        public string? Member_Role { get; set; } = "Guest";

        [Display(Name = "封鎖")]
        public int Member_IsBaned { get; set; } = 0;

        [Display(Name = "創建日期")]
        public DateTime Member_CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "證照")]
        public List<License> Member_Licenses { get; set; } = new List<License>();// 多對多關聯
    }
}
