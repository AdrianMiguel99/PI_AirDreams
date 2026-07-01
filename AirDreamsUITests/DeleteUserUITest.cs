using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UIAutomationTests;

public class DeleteUserUITest
{
    private const string AdminEmailPrueba = "admin3@air.com";
    private const string AdminPasswordPrueba = "1234";
    private const string FrontendUrl = "http://localhost:5173";

    private IWebDriver _driver = null!;
    private WebDriverWait _wait = null!;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();

        options.BinaryLocation = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";

        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-search-engine-choice-screen");

        _driver = new ChromeDriver(options);

        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
    }

    [Test]
    public void Admin_ShouldShowErrorPopup_WhenTryingToDeleteItself()
    {
        _driver.Navigate().GoToUrl($"{FrontendUrl}/login");

        var correoInput = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='email']"))
        );

        var passwordInput = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='password']"))
        );

        correoInput.Clear();
        correoInput.SendKeys(AdminEmailPrueba);

        passwordInput.Clear();
        passwordInput.SendKeys(AdminPasswordPrueba);

        var botonIngresar = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(text(), 'Ingresar')]"))
        );

        ClickElement(botonIngresar);

        _wait.Until(driver =>
            driver.Url.Contains("/admin")
        );

        _driver.Navigate().GoToUrl($"{FrontendUrl}/admin/usuarios");

        var tituloUsuarios = _wait.Until(driver =>
            driver.FindElement(By.XPath("//h2[contains(text(), 'Usuarios')]"))
        );

        Assert.That(tituloUsuarios.Displayed, Is.True);

        var deleteOwnUserButton = _wait.Until(driver =>
        {
            var rows = driver.FindElements(By.CssSelector("tbody tr"));

            foreach (var row in rows)
            {
                if (row.Text.Contains(AdminEmailPrueba))
                {
                    var buttons = row.FindElements(By.CssSelector(".btn-delete"));

                    if (buttons.Count > 0 && buttons[0].Displayed && buttons[0].Enabled)
                        return buttons[0];
                }
            }

            return null;
        });

        Assert.That(deleteOwnUserButton.Displayed, Is.True);

        ClickElement(deleteOwnUserButton);

        var alert = _wait.Until(driver =>
        {
            try
            {
                return driver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException)
            {
                return null;
            }
        });

        alert!.Accept();

        var errorPopup = _wait.Until(driver =>
        {
            var bodyText = driver.FindElement(By.TagName("body")).Text;

            if (
                bodyText.Contains("Error") &&
                (
                    bodyText.Contains("Error al eliminar usuario") ||
                    bodyText.Contains("no puede eliminar") ||
                    bodyText.Contains("No puede eliminar") ||
                    bodyText.Contains("eliminarse a sí mismo") ||
                    bodyText.Contains("eliminarse a si mismo")
                )
            )
            {
                return driver.FindElement(By.TagName("body"));
            }

            return null;
        });

        Assert.That(errorPopup.Displayed, Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        if (_driver == null)
            return;

        _driver.Quit();
        _driver.Dispose();
    }

    private void ClickElement(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript(
            "arguments[0].scrollIntoView({ block: 'center', inline: 'center' });",
            element
        );

        wait.Until( => element.Displayed && element.Enabled);

        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("""
                arguments[0].dispatchEvent(new MouseEvent('click', {
                    bubbles: true,
                    cancelable: true,
                    view: window
                }));
            """, element);
        }
    }
}