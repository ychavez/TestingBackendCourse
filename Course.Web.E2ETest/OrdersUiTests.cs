using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Course.Web.E2ETest;

/// <summary>
/// Pruebas E2E que abren un navegador real (Chromium) contra la app Blazor en
/// http://localhost:5206. Asegurate de tener corriendo:
///   1) Course.Api  (http://localhost:5151)
///   2) Course.Web  (http://localhost:5206)
///
/// Para ver el navegador mientras corren las pruebas, ejecutalas con el archivo
/// .runsettings incluido (Headless=false, SlowMo=500).
/// </summary>
[TestFixture]
public class OrdersUiTests : PageTest
{
    private const string BaseUrl = "http://localhost:5206";

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
        IgnoreHTTPSErrors = true
    };

    [Test]
    public async Task Home_ShouldShowProductsCatalog()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Pedidos eCommerce" }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByTestId("products-table")).ToBeVisibleAsync(new()
        {
            Timeout = 15000
        });

        await Expect(Page.GetByTestId("connection-status")).ToHaveTextAsync("Conectado", new()
        {
            Timeout = 15000
        });
    }

    [Test]
    public async Task CreateOrder_WhenSelectingProduct_ShouldShowApprovedPayment()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForCatalogReadyAsync();

        // Seleccionamos un producto barato (Mouse $45) para no superar el limite
        // de aprobacion del gateway (1000) y garantizar pago aprobado.
        await SetQuantityByTestIdAsync("quantity-ergonomic-mouse", "2");
        await ClickCreateOrderAsync();

        await EnsureSuccessAsync("No se pudo crear el pedido.");

        await Expect(Page.GetByTestId("payment-status")).ToHaveTextAsync("Approved", new()
        {
            Timeout = 15000
        });

        await Expect(Page.GetByTestId("order-id")).Not.ToHaveTextAsync("-");
    }

    [Test]
    public async Task CancelOrder_AfterCreate_ShouldUpdateStatusToCancelled()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForCatalogReadyAsync();

        // Usamos un producto caro (Developer Laptop $1200) para que el FakePaymentGateway
        // lo rechace (>$1000). Un pedido en estado PaymentRejected SI se puede cancelar;
        // uno en estado Paid no.
        await SetQuantityByTestIdAsync("quantity-developer-laptop", "1");
        await ClickCreateOrderAsync();

        await EnsureSuccessAsync("No se pudo crear el pedido antes de cancelar.");

        // Confirmamos que efectivamente quedo rechazado para que sea cancelable
        await Expect(Page.GetByTestId("payment-status")).ToHaveTextAsync("Rejected", new() { Timeout = 15000 });

        var cancelButton = Page.GetByTestId("cancel-order");
        await Expect(cancelButton).ToBeEnabledAsync(new() { Timeout = 15000 });
        await cancelButton.ClickAsync();

        await Expect(Page.GetByTestId("order-status"))
            .ToHaveTextAsync("Cancelled", new() { Timeout = 15000 });
    }

    // ----------------- Helpers -----------------

    private async Task WaitForCatalogReadyAsync()
    {
        await Expect(Page.GetByTestId("products-table")).ToBeVisibleAsync(new() { Timeout = 15000 });
        await Expect(Page.GetByTestId("connection-status")).ToHaveTextAsync("Conectado", new() { Timeout = 15000 });
        // Esperar a que Blazor Server termine la conexion interactiva (SignalR)
        await Expect(Page.GetByTestId("interactive-status")).ToHaveTextAsync("Interactive", new() { Timeout = 15000 });
    }

    private async Task SetQuantityByTestIdAsync(string testId, string value)
    {
        var input = Page.GetByTestId(testId);
        await Expect(input).ToBeVisibleAsync(new() { Timeout = 15000 });
        await input.ClickAsync(new() { ClickCount = 3 }); // seleccionar todo
        // PressSequentially escribe tecla por tecla y dispara 'oninput' que Blazor escucha
        await input.PressSequentiallyAsync(value, new() { Delay = 50 });
        await input.PressAsync("Tab");
    }

    private async Task SetFirstProductQuantityAsync(string value)
    {
        var firstQuantity = Page.Locator("input.quantity-input").First;
        await firstQuantity.ClickAsync(new() { ClickCount = 3 });
        await firstQuantity.PressSequentiallyAsync(value, new() { Delay = 50 });
        await firstQuantity.PressAsync("Tab");
    }

    private async Task ClickCreateOrderAsync()
    {
        var createButton = Page.GetByTestId("create-order");
        await Expect(createButton).ToBeEnabledAsync(new() { Timeout = 15000 });

        // Asegura que Blazor ya recalculo el total (> $0.00) antes de enviar el pedido,
        // si no la API responde 'El monto del pago debe ser mayor que cero'.
        var totalLabel = Page.Locator(".order-bar strong");
        await Expect(totalLabel).Not.ToHaveTextAsync("$0.00", new() { Timeout = 15000 });

        await createButton.ClickAsync();
    }

    /// <summary>
    /// Espera a que aparezca un mensaje (exito o error) y falla con el texto del error
    /// si la API devolvio fallo, para que el motivo real sea visible en el reporte.
    /// </summary>
    private async Task EnsureSuccessAsync(string contextMessage)
    {
        var success = Page.GetByTestId("message-success");
        var error = Page.GetByTestId("message-error");

        // Espera hasta 15s a que cualquiera de los dos sea visible
        await Page.WaitForFunctionAsync(
            @"() => document.querySelector('[data-testid=""message-success""]') 
                   || document.querySelector('[data-testid=""message-error""]')",
            new PageWaitForFunctionOptions { Timeout = 15000 });

        if (await error.IsVisibleAsync())
        {
            var text = await error.InnerTextAsync();
            await TakeFailureScreenshotAsync("create-order-error");
            Assert.Fail($"{contextMessage} Mensaje de la app: '{text}'");
        }

        await Expect(success).ToBeVisibleAsync(new() { Timeout = 5000 });
    }

    private async Task TakeFailureScreenshotAsync(string name)
    {
        var path = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{name}-{DateTime.Now:HHmmss}.png");
        await Page.ScreenshotAsync(new() { Path = path, FullPage = true });
        TestContext.WriteLine($"Screenshot guardado en: {path}");
    }
}
