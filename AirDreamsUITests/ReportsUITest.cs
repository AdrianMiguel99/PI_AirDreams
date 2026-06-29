using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UIAutomationTests;

public class ReportsSeleniumTests
{

    private const string AdminEmailPrueba = "admin3@air.com";
    private const string AdminPasswordPrueba = "1234";
    private IWebDriver _driver = null!;
    private WebDriverWait _wait = null!;

    [SetUp]
    public void Setup()
    {
        
        var options = new ChromeOptions();
        options.AddArgument("--disable-search-engine-choice-screen");

        _driver = new ChromeDriver(options);
        _driver.Manage().Window.Maximize();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [Test]
    public void DesdeInicioSesion_DeberiaEntrarAReportesYFiltrarPorAerolinea()
    {
        
        _driver.Navigate().GoToUrl("http://localhost:5173/");

        IWebElement botonLogin = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(text(), 'Iniciar Sesión')]"))
        );

        Assert.That(botonLogin.Displayed, Is.True);
        botonLogin.Click();

        IWebElement tituloLogin = _wait.Until(driver =>
            driver.FindElement(By.XPath("//*[contains(text(), 'Iniciar Sesión')]"))
        );

        Assert.That(tituloLogin.Displayed, Is.True);

        IWebElement correoInput = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='email']"))
        );

        IWebElement passwordInput = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("input[type='password']"))
        );

        correoInput.Clear();
        correoInput.SendKeys(AdminEmailPrueba);

        passwordInput.Clear();
        passwordInput.SendKeys(AdminPasswordPrueba);

        IWebElement botonIngresar = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(text(), 'Ingresar')]"))
        );

        botonIngresar.Click();

        IWebElement panelAdmin = _wait.Until(driver =>
            driver.FindElement(By.XPath("//*[contains(text(), 'Bienvenido a Air Dreams')]"))
        );

        Assert.That(panelAdmin.Displayed, Is.True);

        IWebElement opcionReportes = _wait.Until(driver =>
            driver.FindElement(By.CssSelector("[data-testid='reports-card']"))
        );

        Assert.That(opcionReportes.Displayed, Is.True);
        ClickElement(opcionReportes);

        _wait.Until(driver => driver.Url.Contains("/admin/reports"));

        IWebElement titulo = _wait.Until(driver =>
            driver.FindElement(By.XPath("//*[contains(text(), 'Reporte de ingresos')]"))
        );

        Assert.That(titulo.Displayed, Is.True);

        IWebElement filtroAerolinea = _wait.Until(driver =>
            driver.FindElement(By.XPath("//label[contains(., 'Aerolínea')]//select"))
        );

        var selectAerolinea = new SelectElement(filtroAerolinea);

        Assert.That(selectAerolinea.Options.Count, Is.GreaterThanOrEqualTo(1));

        if (selectAerolinea.Options.Any(option => option.Text == "AirDreams"))
        {
            selectAerolinea.SelectByText("AirDreams");
        }

        IWebElement botonFiltrar = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(text(), 'Filtrar')]"))
        );

        botonFiltrar.Click();

        IWebElement resultado = _wait.Until(driver =>
            driver.FindElement(By.XPath(
                "//*[contains(text(), 'Ingresos totales') or contains(text(), 'No hay datos de ingresos')]"
            ))
        );

        Assert.That(resultado.Displayed, Is.True);
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
                    bubbles: true,
                    cancelable: true,
                    view: window
                }));
            """, element);
        }
    }
}
