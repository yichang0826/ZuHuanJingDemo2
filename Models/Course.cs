using System.ComponentModel.DataAnnotations;

namespace ZuHuanJingDemo2.Models
{
    public class Course
    {
        [Display(Name = "課程編號")][Key]public int Course_Id { get; set; }
        [Display(Name = "課程名稱")][StringLength(255)][Required]public string? Course_Name { get; set; }
        [Display(Name = "課程教師")][StringLength(255)][Required]public string? Course_Teacher { get; set; }
        [Display(Name = "課程介紹")][Required]public string? Course_Introduction { get; set; }
        [Display(Name = "最大人數")][Required]public int? Course_MaxCount { get; set; }
        [Display(Name = "目前人數")][Required]public int? Course_SumCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "開始時間")][Required]public DateTime Course_StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "結束時間")][Required]public DateTime Course_EndDate { get; set; }
        [Display(Name = "創建時間")][Required] public DateTime Course_CreateDate { get; set; }
        [Display(Name = "啟用")]public int Course_IsActive { get; set; }
    }
}