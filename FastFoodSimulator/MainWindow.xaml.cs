using FastFoodSimulatorLibrary;
using FastFoodSimulatorLibrary.Restaurant_;
using System;

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FastFoodSimulator;


public partial class MainWindow : Window
{
    CancellationTokenSource cancellationTokenSource;
    Restaurant restaurant;
    public event EventHandler<Message> StringReturned;

    public MainWindow()
    {
        InitializeComponent();
        btStop.IsEnabled = false;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        int number;
        int customerInterval = int.TryParse(tbCustomerInterval.Text, out number) ? number : 1000;
        tbCustomerInterval.Text = customerInterval.ToString();

        int orderInterval = int.TryParse(tbOrderInterval.Text, out number) ? number : 1000;
        tbOrderInterval.Text = orderInterval.ToString();

        int orderIntervaltoKitchen = int.TryParse(tbOrderIntervaltoKitchen.Text, out number) ? number : 1000;
        tbOrderInterval.Text = orderInterval.ToString();

        int ChefCount = int.TryParse(tbChefs.Text, out number) ? number : 5;
        tbChefs.Text = ChefCount.ToString();

        int TradeCount = int.TryParse(tbTraders.Text, out number) ? number : 5;
        tbTraders.Text = TradeCount.ToString();

        restaurant = new Restaurant(customerInterval, orderInterval, orderIntervaltoKitchen, ChefCount, TradeCount);
        await StartEvent();
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        restaurant.Start(cancellationToken);
        btStop.IsEnabled = true;
        btStart.IsEnabled = false;
    }
    private async Task StartEvent()
    {
        restaurant.StringReturned += (sender, result) =>
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() =>
            {
                var textBoxToUpdate = GetTextBoxToUpdate(result.Name);
                if (textBoxToUpdate == tbTicket || textBoxToUpdate == tbChef || textBoxToUpdate == tbTrader || textBoxToUpdate == tbTakeOrder)
                    textBoxToUpdate.Text = result.Msg;
                else
                    textBoxToUpdate.Text += $"{result.Msg}\n";
                textBoxToUpdate.ScrollToEnd();
            });
        };
    }

    private TextBox GetTextBoxToUpdate(string name)
    {
        switch (name)
        {
            case "Cook":
                return tbCook;
            case "Kitchen":
                return tbKitchen;
            case "Chefs":
                return tbChef;
            case "Traders":
                return tbTrader;
            case "Order":
                return tbTicket;
            case "TakeOrder":
                return tbTakeOrder;
            case "TraderDo":
                return tbTraderDo;
            default:
                return tbServer;
        }
    }
    private async void btStop_ClickAsync(object sender, RoutedEventArgs e)
    {
        btStart.IsEnabled = true;
        btStop.IsEnabled = false;
        if (restaurant != null)
        {
            cancellationTokenSource.Cancel();
            await restaurant.StopAsync();
        }
    }

}
