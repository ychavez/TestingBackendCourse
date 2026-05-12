using Course.PlaywrightTests.Infrastructure;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace Course.PlaywrightTests;

[Collection(FrontendCollection.Name)]
public sealed class FrontendFlowTests : PageTest
{
    private readonly WebAppFixture _app;

    public FrontendFlowTests(WebAppFixture app)
    {
        _app = app;
    }

    [Fact]
    public async Task HomePage_ShouldLoadProductsAndShowConnectedStatus()
    {
        await OpenHomeAsync();

        await Expect(Page.GetByTestId("products-table")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Mechanical Keyboard")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Ergonomic Mouse")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Developer Laptop")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task CreateOrder_WhenMouseQuantityIsTwo_ShouldShowPaidOrder()
    {
        await OpenHomeAsync();

        await Page.GetByTestId("quantity-ergonomic-mouse").FillAsync("2");
        await Expect(Page.GetByTestId("create-order")).ToBeEnabledAsync();
        await Page.GetByTestId("create-order").ClickAsync();

        await Expect(Page.GetByTestId("message-success")).ToContainTextAsync("Pedido creado");
        await Expect(Page.GetByTestId("order-status")).ToContainTextAsync("Paid");
        await Expect(Page.GetByTestId("order-total")).ToContainTextAsync("90.00");
        await Expect(Page.GetByTestId("payment-status")).ToContainTextAsync("Approved");
    }

    [Fact]
    public async Task CreateOrder_WhenLaptopQuantityExceedsStock_ShouldShowStockError()
    {
        await OpenHomeAsync();

        await Page.GetByTestId("quantity-developer-laptop").FillAsync("99");
        await Expect(Page.GetByTestId("create-order")).ToBeEnabledAsync();
        await Page.GetByTestId("create-order").ClickAsync();

        await Expect(Page.GetByTestId("message-error")).ToContainTextAsync("Stock insuficiente");
    }

    [Fact]
    public async Task CreateRejectedOrder_WhenLaptopQuantityIsOne_ShouldAllowCancellation()
    {
        await OpenHomeAsync();

        await Page.GetByTestId("quantity-developer-laptop").FillAsync("1");
        await Expect(Page.GetByTestId("create-order")).ToBeEnabledAsync();
        await Page.GetByTestId("create-order").ClickAsync();

        await Expect(Page.GetByTestId("message-success")).ToContainTextAsync("Pedido creado");
        await Expect(Page.GetByTestId("order-status")).ToContainTextAsync("PaymentRejected");
        await Expect(Page.GetByTestId("payment-status")).ToContainTextAsync("Rejected");

        await Expect(Page.GetByTestId("cancel-order")).ToBeEnabledAsync();
        await Page.GetByTestId("cancel-order").ClickAsync();

        await Expect(Page.GetByTestId("message-success")).ToContainTextAsync("Pedido cancelado");
        await Expect(Page.GetByTestId("order-status")).ToContainTextAsync("Cancelled");
    }

    private async Task OpenHomeAsync()
    {
        Page.SetDefaultTimeout(15_000);

        await Page.GotoAsync(_app.WebBaseUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });

        await Expect(Page.GetByTestId("connection-status")).ToContainTextAsync("Conectado", new() { Timeout = 30_000 });
        await Expect(Page.GetByTestId("interactive-status")).ToContainTextAsync("Interactive", new() { Timeout = 30_000 });
    }
}
