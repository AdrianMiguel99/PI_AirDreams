using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System;
using System.Threading;

namespace Test1.UITests
{
    public class AirportCreationUITest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.BinaryLocation = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe"; // Set the path to the Brave browser executable
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        [Test]
        public void CreateAirport_AsAdmin_ShouldSucceed()
        {
            _driver.Navigate().GoToUrl("http://localhost:5173/login");

            var emailInput = _wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")));
            emailInput.Clear();
            emailInput.SendKeys("admin3@air.com");   

            var passwordInput = _driver.FindElement(By.CssSelector("input[type='password']"));
            passwordInput.Clear();
            passwordInput.SendKeys("1234");

            var loginButton = _driver.FindElement(By.XPath("//button[contains(text(),'Ingresar')]"));
            loginButton.Click();

            _wait.Until(d => d.Url.Contains("/admin"));

            _driver.Navigate().GoToUrl("http://localhost:5173/admin/airports/register");

            _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'País')]/following-sibling::select")).Displayed);

            var codigoInput = _driver.FindElement(By.XPath("//label[contains(text(),'Código')]/following-sibling::input"));
            codigoInput.Clear();
            codigoInput.SendKeys("TST");

            var nombreInput = _driver.FindElement(By.XPath("//label[contains(text(),'Nombre del Aeropuerto')]/following-sibling::input"));
            nombreInput.Clear();
            nombreInput.SendKeys("Aeropuerto de Prueba Selenium");

            var paisSelect = new SelectElement(_driver.FindElement(By.XPath("//label[contains(text(),'País')]/following-sibling::select")));
            paisSelect.SelectByText("Costa Rica");

            _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Ciudad')]/following-sibling::select")).Enabled);
            var ciudadSelect = new SelectElement(_driver.FindElement(By.XPath("//label[contains(text(),'Ciudad')]/following-sibling::select")));
            ciudadSelect.SelectByText("San José");

            var zonaInput = _driver.FindElement(By.XPath("//label[contains(text(),'Zona Horaria')]/following-sibling::input"));
            zonaInput.Clear();
            zonaInput.SendKeys("-06:00");

            var registrarBtn = _driver.FindElement(By.XPath("//button[contains(text(),'Registrar Aeropuerto')]"));
            registrarBtn.Click();

            _wait.Until(d => d.FindElement(By.CssSelector(".popup-card .success")).Displayed);
            var mensaje = _driver.FindElement(By.CssSelector(".popup-card p")).Text;

            Assert.That(mensaje, Does.Contain("creado correctamente"));
        }
    }
}