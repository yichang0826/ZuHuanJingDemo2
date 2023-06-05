using System.ComponentModel.DataAnnotations;

namespace ZuHuanJingDemo2.Models
{
    public class License
    {
        [Key]public int License_Id { get; set; }
        [Display(Name = "證照名稱")][StringLength(255)][Required]public string? License_Name { get; set; }
        [Display(Name = "證照介紹")][StringLength(255)][Required] public string? License_Introduction { get; set; }
        public DateTime License_CreateDate { get; set; }
        public bool License_IsActive { get; set; } = true;
        public List<Member> License_Members { get; set; } = new List<Member>();// 多對多關聯
    }
}
