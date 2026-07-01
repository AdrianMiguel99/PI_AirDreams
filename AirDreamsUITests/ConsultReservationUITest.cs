using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UIAutomationTests;

public class ConsultReservationUITest
{
    private const string FrontendUrl = "http://localhost:5173";

    private const string ReservationCodePrueba = "TXN-3fbdb1ce";
    private const string PassengerLastNamePrueba = "Arrieta";

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
    public void User_ShouldConsultReservationAndSeeReservationInformation()
    {
        _driver.Navigate().GoToUrl($"{FrontendUrl}/consultar-reserva");

        _wait.Until(driver =>
            driver.FindElement(By.TagName("body")).Displayed
        );

        var inputs = _wait.Until(driver =>
        {
            var visibleInputs = driver
                .FindElements(By.CssSelector("input"))
                .Where(input => input.Displayed && input.Enabled)
                .ToList();

            return visibleInputs.Count >= 2 ? visibleInputs : null;
        });

        var reservationCodeInput = inputs[0];
        var passengerLastNameInput = inputs[1];

        reservationCodeInput.Clear();
        reservationCodeInput.SendKeys(ReservationCodePrueba);

        passengerLastNameInput.Clear();
        passengerLastNameInput.SendKeys(PassengerLastNamePrueba);

        var consultButton = _wait.Until(driver =>
            driver.FindElement(By.XPath("//button[contains(., 'Consultar') or contains(., 'Buscar') or contains(., 'Ver reserva')]"))
        );

        ClickElement(consultButton);

        var bodyText = _wait.Until(driver =>
        {
            var text = driver.FindElement(By.TagName("body")).Text;

            if (
                text.Contains("No se encontró") ||
                text.Contains("no se encontró") ||
                text.Contains("No encontrada") ||
                text.Contains("no encontrada") ||
                text.Contains("Error") ||
                text.Contains("error")
            )
            {
                return text;
            }

            if (
                text.Contains("Información") ||
                text.Contains("información") ||
                text.Contains("Detalles") ||
                text.Contains("detalles") ||
                text.Contains("Salida") ||
                text.Contains("Destino") ||
                text.Contains("Origen") ||
                text.Contains("Directo") ||
                text.Contains("escala") ||
                text.Contains("Cancelar") ||
                text.Contains("Agregar maletas")
            )
            {
                return text;
            }

            return null;
        });

        Console.WriteLine("URL actual:");
        Console.WriteLine(_driver.Url);

        Console.WriteLine("Texto visible en pantalla:");
        Console.WriteLine(bodyText);

        Assert.That(
            bodyText.Contains("No se encontró") ||
            bodyText.Contains("no se encontró") ||
            bodyText.Contains("No encontrada") ||
            bodyText.Contains("no encontrada") ||
            bodyText.Contains("Error") ||
            bodyText.Contains("error"),
            Is.False,
            "La consulta de reserva devolvió un mensaje de error. Revise que el código de reserva y apellido existan en la base."
        );

        Assert.That(
            bodyText.Contains("Información") ||
            bodyText.Contains("información") ||
            bodyText.Contains("Detalles") ||
            bodyText.Contains("detalles") ||
            bodyText.Contains("Salida") ||
            bodyText.Contains("Destino") ||
            bodyText.Contains("Origen") ||
            bodyText.Contains("Directo") ||
            bodyText.Contains("escala") ||
            bodyText.Contains("Cancelar") ||
            bodyText.Contains("Agregar maletas"),
            Is.True
        );
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