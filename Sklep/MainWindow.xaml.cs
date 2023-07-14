using Microsoft.EntityFrameworkCore;
using Sklep.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sklep
{
    public partial class MainWindow : Window
    {
        private User _user;
        int counter = 0;
        public MainWindow(User user)
        {
            InitializeComponent();
            _user = user;

            if(user.isModerator)
            {
                addProduct.Visibility= Visibility.Visible;
                users.Visibility = Visibility.Visible;
            }
            else
            {
                addProduct.Visibility = Visibility.Hidden;
                users.Visibility = Visibility.Hidden;
            }
            GetProducts();
        }
        void GetProducts()
        {
            productsHolder.Children.Clear();
            counter = 0;
            using (var context = new SklepDbContext())
            {
                var list =  context.Items.ToList();
                
                foreach (var item in list)
                {

                    var stack = new StackPanel();
                    var grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    
                    var name = new TextBlock { Text = item.Name, Padding = new Thickness(5), FontSize=25 };
                    var description = new TextBlock { Text = item.Description, Padding = new Thickness(5) };
                    var price = new TextBlock { Text = $"{item.Price.ToString()} zł/szt", Padding = new Thickness(5) };
                    grid.Children.Add(name);
                    grid.Children.Add(description);
                    grid.Children.Add(price);
                    stack.Background = Brushes.LightGray;
                    stack.Margin= new Thickness(10);
                    stack.Children.Add(grid);
                    Grid.SetRow(name, 0);
                    Grid.SetColumn(name, 0);
                    Grid.SetRow(description, 1);
                    Grid.SetColumn(description, 0);
                    Grid.SetRow(price, 3);
                    Grid.SetColumn(price, 0);

                    var cartButton = new Button() { Content = "Dodaj do koszyka", Background = Brushes.Green, Tag=item.Id};
                    cartButton.Click += new RoutedEventHandler(addToCart_Clicked);
                    grid.Children.Add(cartButton);
                    Grid.SetRowSpan(cartButton, 3);
                    Grid.SetColumn(cartButton, 1);
                    if(_user.isModerator)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        var deleteButton = new Button() { Content = "Usuń", Background = Brushes.Red, Tag = item.Id };
                        deleteButton.Click += new RoutedEventHandler(deleteProductFromList_Clicked);
                        grid.Children.Add(deleteButton);
                        Grid.SetRowSpan(deleteButton, 3);
                        Grid.SetColumn(deleteButton, 2);
                    }
                    
                    productsHolder.Children.Add(stack);
                }               
            }
        }
        void GetCart()
        {
            cartHolder.Children.Clear();
            var submitButton = new Button() { Content = "Zrealizuj zamówienie", Background = Brushes.Green, Height=50, Margin= new Thickness(5) };
            submitButton.Click += new RoutedEventHandler(submitCart_Clicked);

            using (var context = new SklepDbContext())
            {
                if(_user.Cart!=null && _user.Cart.Items!=null)
                {
                    
                    var list = _user.Cart.Items.ToList();
                    if (list.Count == 0)
                    {
                        submitButton.IsEnabled = false;
                        var stack = new StackPanel();
                        var textBlock = new TextBlock() { Text = "Twój koszyk jest pusty", FontSize = 25 };
                        stack.Children.Add(textBlock);
                        cartHolder.Children.Add(submitButton);
                        cartHolder.Children.Add(stack);

                    }
                    else
                    {
                        cartHolder.Children.Add(submitButton);
                        submitButton.IsEnabled = true;

                        foreach (var item in list)
                        {
                            var stack = new StackPanel();
                            var grid = new Grid();
                            grid.RowDefinitions.Add(new RowDefinition());
                            grid.RowDefinitions.Add(new RowDefinition());
                            grid.RowDefinitions.Add(new RowDefinition());
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            grid.ColumnDefinitions.Add(new ColumnDefinition());

                            var name = new TextBlock { Text = item.Name, Padding = new Thickness(5), FontSize = 25 };
                            var description = new TextBlock { Text = item.Description, Padding = new Thickness(5) };
                            var price = new TextBlock { Text = $"{item.Price.ToString()} zł/szt", Padding = new Thickness(5) };
                            grid.Children.Add(name);
                            grid.Children.Add(description);
                            grid.Children.Add(price);
                            stack.Background = Brushes.LightGray;
                            stack.Margin = new Thickness(10);
                            stack.Children.Add(grid);
                            Grid.SetRow(name, 0);
                            Grid.SetColumn(name, 0);
                            Grid.SetRow(description, 1);
                            Grid.SetColumn(description, 0);
                            Grid.SetRow(price, 3);
                            Grid.SetColumn(price, 0);

                            var deleteButton = new Button() { Content = "Usuń", Background = Brushes.Red, Tag = item.Id };
                            deleteButton.Click += new RoutedEventHandler(deleteProductFromCart_Clicked);
                            grid.Children.Add(deleteButton);
                            Grid.SetRowSpan(deleteButton, 3);
                            Grid.SetColumn(deleteButton, 1);

                            cartHolder.Children.Add(stack);
                            
                        }
                    }
                }
                else
                {
                    submitButton.IsEnabled = false;
                    var stack = new StackPanel();
                    var textBlock = new TextBlock() { Text = "Twój koszyk jest pusty", FontSize = 25 };
                    stack.Children.Add(textBlock);
                    cartHolder.Children.Add(submitButton);
                    cartHolder.Children.Add(stack);
                }
                

                
            }
        }

        private void submitCart_Clicked(object sender, RoutedEventArgs e)
        {
            var order = new Order()
            {
                Items = _user.Cart.Items,
                Date = DateTime.Now,
            };
            using(var context = new SklepDbContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id.Equals(_user.Id));
                user.Orders.Add(order);
                user.Cart = null;
                context.SaveChanges();
            }

            cartHolder.Children.Clear();
            var stack = new StackPanel();
            var textBlock = new TextBlock() { Text = "Zamówienie zrealizowane", FontSize = 25, Foreground=Brushes.Green };
            stack.Children.Add(textBlock);
            cartHolder.Children.Add(stack);
        }

        private void deleteProductFromCart_Clicked(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Tag.ToString());
            using (var context = new SklepDbContext())
            {
                var item = _user.Cart.Items.FirstOrDefault(x => x.Id.Equals(id));
                _user.Cart.Items.Remove(item);
                context.SaveChanges();
            }
            GetCart();
        }

        private void deleteProductFromList_Clicked(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Tag.ToString());
            using(var context = new SklepDbContext())
            {
                var item = context.Items.FirstOrDefault(x => x.Id.Equals(id));
                context.Items.Remove(item);
                context.SaveChanges();
            }
            GetProducts();
        }

        private void addToCart_Clicked(object sender, RoutedEventArgs e)
        {
            
            var id = int.Parse((sender as Button).Tag.ToString());
            using (var context = new SklepDbContext())
            {
                var user = context.Users.Include(x=>x.Cart).FirstOrDefault(x => x.Id.Equals(_user.Id));

                var item = context.Items.FirstOrDefault(x => x.Id.Equals(id));
                user.Cart.Items.Add(item);
                context.SaveChanges();
                counter++;
                _user = user;
            }
            (sender as Button).Content = $"Dodaj do koszyka \n Pomyślnie dodano produkt +{counter}";
        }

        private void items_Clicked(object sender, RoutedEventArgs e)
        {
            GetProducts();
            productsPanel.Visibility = Visibility.Visible;
            addproductPanel.Visibility = Visibility.Hidden;
            cartPanel.Visibility = Visibility.Hidden;
        }
        private void cart_Clicked(object sender, RoutedEventArgs e)
        {
            GetCart();
            cartPanel.Visibility = Visibility.Visible;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
        }
        private void orders_Clicked(object sender, RoutedEventArgs e)
        {
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
        }
        private void addProduct_Clicked(object sender, RoutedEventArgs e)
        {
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Visible;
        }
        private void users_Clicked(object sender, RoutedEventArgs e)
        {
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
        }
        private void logout_Clicked(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.Show();
            this.Close();
        }

        private void Submit_addProduct_Clicked(object sender, RoutedEventArgs e)
        {
            if(name.Text.Length > 3 && description.Text.Length > 3 && price.Text.Length > 0)
            {
                if(double.TryParse(price.Text, out double priceParsed))
                {
                    var item = new Item()
                    {
                        Name = name.Text,
                        Description = description.Text,
                        Price = priceParsed
                    };
                    using(var context = new SklepDbContext())
                    {
                        context.Items.Add(item);
                        context.SaveChanges();
                        error.Text = "Pomyślnie dodano przedmiot";
                        error.Foreground = Brushes.Green;
                        if (error.Visibility == Visibility.Hidden)
                        {
                            error.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    error.Text = "Nieprawidłowa wartość w polu 'cena'!";
                    error.Foreground = Brushes.Red;
                    if (error.Visibility == Visibility.Hidden)
                    {
                        error.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                error.Text = "Wypełnij wszystkie pola!";
                error.Foreground = Brushes.Red;
                if (error.Visibility == Visibility.Hidden)
                {
                    error.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
