using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Threading.Tasks;

namespace MyApp.E2ETests
{
    [TestClass]
    public class LoginTests : PageTest
    {
        [TestMethod]
        public async Task UserCanLoginSuccessfully()
        {
            //  رفتن به صفحه لاگین
            await Page.GotoAsync("http://localhost:5218/login");

            // پر کردن فیلدهای فرم
            await Page.FillAsync("input[name='userName']", "testuser");
            await Page.FillAsync("input[name='password']", "testpassword");

            // کلیک روی دکمه ورود
            await Page.ClickAsync("button[type='submit']");

            // اینجا باید صفحه‌ای که بعد از ورود باید نمایش داده بشه رو بررسی کنید
            var pageTitle = await Page.TitleAsync();
            Assert.AreEqual("صفحه اصلی", pageTitle);  // مثلا چک کنید که عنوان صفحه به صفحه اصلی تغییر کرده باشد
        }
    }
}
