﻿using Microsoft.AspNetCore.Mvc;

public class MemberTopMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        // 執行檢視元件的邏輯
        return View();
    }
}
