using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace UIAutomationTests;

public class InviteUserUITest
{
    private const string FrontendUrl = "http://localhost:5173";

    private const string AdminEmail = "admin3@air.com";
    private const string AdminPassword = "1234";

    private const string InvitedEmail = "testUI@gmail.com";

    private IWebDriver _driver = null!;
    private WebDriverWait _wait = null!;

    [SetUp]
    public void Setup()
    {
        var options = new FirefoxOptions();

        options.AddArgument("--width=1600");
        options.AddArgument("--height=900");

        _driver = new FirefoxDriver(options);

        _driver.Manage().Window.Maximize();

        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
    }

    [Test]
    public void Admin_ShouldInviteNewAdminUser()
    {
        // Login
        _driver.Navigate().GoToUrl($"{FrontendUrl}/login");

        var emailInput = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='email']")));

        var passwordInput = _driver.FindElement(By.CssSelector("input[type='password']"));

        emailInput.Clear();
        emailInput.SendKeys(AdminEmail);

        passwordInput.Clear();
        passwordInput.SendKeys(AdminPassword);

        var loginButton = _driver.FindElement(
            By.XPath("//button[contains(text(),'Ingresar')]"));

        ClickElement(loginButton);

        _wait.Until(driver => driver.Url.Contains("/admin"));

        // Usuarios
        _driver.Navigate().GoToUrl($"{FrontendUrl}/admin/usuarios");

        _wait.Until(driver =>
            driver.FindElement(By.XPath("//h2[contains(text(),'Usuarios')]")));

        // Añadir+
        var addButton = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(text(),'Añadir+')]")));

        ClickElement(addButton);

        // Página de invitación
        _wait.Until(driver => driver.Url.Contains("/registro"));

        // Correo
        var inviteEmail = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='email']")));

        inviteEmail.Clear();
        inviteEmail.SendKeys(InvitedEmail);

        // Tipo de usuario
        var roleSelect = new SelectElement(
            _driver.FindElement(By.TagName("select")));

        roleSelect.SelectByText("Admin");

        // Enviar
        var sendButton = _driver.FindElement(
            By.XPath("//button[contains(text(),'Enviar Invitación')]"));

        ClickElement(sendButton);

        // Esperar mensaje
        var message = _wait.Until(driver =>
        {
            var text = driver.FindElement(By.TagName("body")).Text;

            if (
                text.Contains("Invitación enviada") ||
                text.Contains("exitosamente") ||
                text.Contains("ya fue invitado") ||
                text.Contains("ya existe")
            )
            {
                return text;
            }

            return null;
        });

        Console.WriteLine(message);

        Assert.That(
            message.Contains("Invitación enviada") ||
            message.Contains("exitosamente"),
            Is.True,
            "No se mostró el mensaje de invitación exitosa."
        );
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    private void ClickElement(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript(
            "arguments[0].scrollIntoView({block:'center'});",
            element);

        _wait.Until(_ => element.Displayed && element.Enabled);

        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("""
                arguments[0].dispatchEvent(new MouseEvent('click', {
                    bubbles:true,
                    cancelable:true,
                    view:window
                }));
            """, element);
        }
    }
}