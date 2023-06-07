#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
namespace ZuHuanJingDemo2.Models.ViewModel
{
    public class MemberEditViewModel
    {

        public Member member { get; set; }

        public List<License> licenses { get; set; }
    }
}
#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
