using AddressManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AddressManager.Data;

namespace AddressManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly AddressManagerContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AddressManagerContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.Log(LogLevel.Information, $"Logout() -> logout");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Goodbye()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            //  IsValid : 모든값 입력 받았는지 검증 ID, 비밀번호 필수로 적어야 한다.
            // [Required] 어노테이션을 기준으로 판단한다. 
            if (ModelState.IsValid)
            {
                // DB 오픈 커넥션 후 자동으로 닫음
                var user = _context.User.FirstOrDefault(u => u.LoginId.Equals(model.UserId) && u.Password.Equals(model.Password));

                /* 로그인 성공 */
                if (user != null)
                {
                    /* HttpContext.Session.SetInt32(key, value); */
                    /* "USER_LOGIN_KEY"라는 이름으로 Session에 담는다. */
                    HttpContext.Session.SetInt32("userId", user.Id);
                    HttpContext.Session.SetString("userLoginId", user.LoginId);
                    _logger.Log(LogLevel.Information,
                        $"login success userId = {user.Id}, userLoginId = {user.LoginId}");
                    return RedirectToAction("Index", "Home");
                }

                // 로그인 실패,  사용자 ID 자체가 회원가입 X 경우
                ModelState.AddModelError(string.Empty, "사용자 ID 혹은 비밀번호가 올바르지 않습니다.");
                _logger.LogError($"cannot match user LoginId and password");
                if (user != null)
                {
                    //  ModelState.AddModelError(string, "이미탈퇴한 회원입니다.");
                    _logger.Log(LogLevel.Information,
                         $"login fail user id = {user.Id}, LoginId = {user.LoginId}");
                }
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}