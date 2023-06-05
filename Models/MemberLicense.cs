using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ZuHuanJingDemo2.Models
{
    public class MemberLicense
    {
        [Key]public int MemberLicense_Id { get; set; }
        [Display(Name = "會員Id")][Required,NotNull]public int MemberId { get; set; } // 外鍵
        [Display(Name = "證照Id")][Required, NotNull] public int LicenseId { get; set; } // 外鍵
        [Display(Name = "創建日期")] public DateTime CreatedDate { get; set; } = new DateTime();
        public Member Member { get; set; } = new Member(); // 外鍵關聯
        public License License { get; set; } = new License();// 外鍵關聯
    }
}
