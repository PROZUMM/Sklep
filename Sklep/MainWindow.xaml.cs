using Microsoft.EntityFrameworkCore;
using Sklep.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
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
        private readonly int _id;
        private bool isModerator;
        private readonly SklepDbContext _context;
        private List<Item> cartList = new List<Item>();
        public MainWindow(int id, SklepDbContext context)
        {
            InitializeComponent();
            _id = id;
            _context = context;

            var user = context.Users.Include(x => x.Orders).ThenInclude(x => x.OrderedItems).FirstOrDefault(x => x.Id.Equals(_id));
            isModerator = user.isModerator;
            if (isModerator)
            {
                addProduct.Visibility = Visibility.Visible;
                users.Visibility = Visibility.Visible;
            }
            else
            {
                addProduct.Visibility = Visibility.Hidden;
                users.Visibility = Visibility.Hidden;
            }
            GetProducts();
            foreach (var item in user.Orders)
            {
                //Debug.WriteLine(item.OItems.Count);
            }
        }
        void GetProducts()
        {
            productsHolder.Children.Clear();
            var list = _context.Items.AsNoTracking().ToList();

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

                var cartButton = new Button() { Content = "Dodaj do koszyka", Background = Brushes.Green, Tag = item.Id };
                cartButton.Click += new RoutedEventHandler(addToCart_Clicked);
                grid.Children.Add(cartButton);
                Grid.SetRowSpan(cartButton, 3);
                Grid.SetColumn(cartButton, 1);
                if (isModerator)
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
        void GetCart()
        {
            cartHolder.Children.Clear();
            double fullPrice = 0;
            var submitButton = new Button() { Content = "Zrealizuj zamówienie", Background = Brushes.Green, Height = 50, Margin = new Thickness(5) };
            submitButton.Click += new RoutedEventHandler(submitCart_Clicked);

            var user = _context.Users.FirstOrDefault(x => x.Id.Equals(_id));
            var cart = _context.Carts.Include(x => x.Items).FirstOrDefault(x => x.Id.Equals(user.CartId));

            if (cart.Items != null)
            {
                var list = cart.Items;
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

                    for (int i = 0; i < list.Count; i++)
                    {
                        fullPrice += list[i].Price * list[i].Count;
                        submitButton.Content = $"Zrealizuj zamówienie [{fullPrice}] zł";
                        var stack = new StackPanel();
                        var grid = new Grid();
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());


                        var name = new TextBlock { Text = list[i].Name, Padding = new Thickness(5), FontSize = 25 };
                        var description = new TextBlock { Text = list[i].Description, Padding = new Thickness(5) };
                        var price = new TextBlock { Text = $"{list[i].Price.ToString()} zł/szt", Padding = new Thickness(5) };
                        var count = new TextBlock { Text = $"Ilość w koszyku: {list[i].Count}", Padding = new Thickness(5) };
                        grid.Children.Add(name);
                        grid.Children.Add(description);
                        grid.Children.Add(price);
                        grid.Children.Add(count);
                        stack.Background = Brushes.LightGray;
                        stack.Margin = new Thickness(10);
                        stack.Children.Add(grid);
                        Grid.SetRow(name, 0);
                        Grid.SetColumn(name, 0);
                        Grid.SetRow(description, 1);
                        Grid.SetColumn(description, 0);
                        Grid.SetRow(price, 2);
                        Grid.SetColumn(price, 0);
                        Grid.SetRow(count, 3);
                        Grid.SetColumn(count, 0);

                        var deleteButton = new Button() { Content = "Usuń", Background = Brushes.Red, Tag = list[i].Id };
                        deleteButton.Click += new RoutedEventHandler(deleteProductFromCart_Clicked);
                        grid.Children.Add(deleteButton);
                        Grid.SetRowSpan(deleteButton, 4);
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
        void GetOrders()
        {
            orderHolder.Children.Clear();

            var user = _context.Users.Include(x => x.Orders).ThenInclude(x => x.OrderedItems).FirstOrDefault(x => x.Id.Equals(_id));

            if (user.Orders.Count < 1)
            {
                var stack = new StackPanel();
                var textBlock = new TextBlock() { Text = "Nie masz żadnych zamówień", FontSize = 25 };
                stack.Children.Add(textBlock);
                orderHolder.Children.Add(stack);
            }
            else
            {
                foreach(var order in user.Orders)
                {
                    var stack = new StackPanel();
                    stack.Background = Brushes.LightGray;
                    stack.Margin = new Thickness(10);
                    var price = new TextBlock { Text = $"Wartość: {order.fullPrice} zł", Padding = new Thickness(5, 0, 5, 5), FontWeight = FontWeights.Bold };
                    var line = new StackPanel() { Orientation= Orientation.Horizontal, Height = 1, Background=Brushes.Black, Width=stack.Width };
                    var date = new TextBlock { Text = order.Date.ToString("dd/MM/yyyy HH:mm"), Padding = new Thickness(5), FontWeight = FontWeights.Bold };
                    stack.Children.Add(date);
                    
                    stack.Children.Add(price);
                    stack.Children.Add(line);
                                       
                    foreach (var item in order.OrderedItems)
                    {
                        var s = new StackPanel() { Orientation = Orientation.Horizontal};


                        var name = new TextBlock { Text = $"{item.Name}", Padding = new Thickness(5), FontWeight = FontWeights.Bold};
                        var description = new TextBlock { Text = item.Description, Padding = new Thickness(5) };
                        
                        var count = new TextBlock { Text = $"Ilość: {item.Count}", Padding = new Thickness(5) };
                       
                        s.Children.Add(name);
                        s.Children.Add(description);
                        s.Children.Add(count);
                        
                        stack.Children.Add(s);
                    }
                    orderHolder.Children.Add(stack);
                }
            }            
        }
        void GetUsers()
        {
            usersHolder.Children.Clear();
            var u = _context.Users.FirstOrDefault(x => x.Id.Equals(_id));
            var users = _context.Users.ToList();

            if (users.Count < 1)
            {
                var stack = new StackPanel();
                var textBlock = new TextBlock() { Text = "Nie ma tu żadnych użytkowników", FontSize = 25 };
                stack.Children.Add(textBlock);
                usersHolder.Children.Add(stack);
            }
            else
            {
                foreach (var user in users)
                {
                    if (user.Equals(u)) continue;

                    var stack = new StackPanel();
                    var grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());

                    var name = new TextBlock { Text = user.Username, Padding = new Thickness(5), FontSize = 25 };
                    var role = new TextBlock { Padding = new Thickness(5) };
                    if (user.isModerator)
                    {
                        role.Text = "Moderator";
                        role.Foreground = Brushes.Red;
                        role.FontWeight= FontWeights.Bold;
                    }
                    else role.Text = "User";

                    grid.Children.Add(name);
                    grid.Children.Add(role);

                    stack.Background = Brushes.LightGray;
                    stack.Margin = new Thickness(10);
                    stack.Children.Add(grid);
                    Grid.SetRow(name, 0);
                    Grid.SetColumn(name, 0);
                    Grid.SetRow(role, 0);
                    Grid.SetColumn(role, 1);

                    var cartButton = new Button() { Content = "Zmień role", Background = Brushes.Gray, Tag = user.Id };
                    cartButton.Click += new RoutedEventHandler(changeRole_Clicked);
                    grid.Children.Add(cartButton);
                    Grid.SetRow(cartButton, 0);
                    Grid.SetColumn(cartButton, 2);

                    usersHolder.Children.Add(stack);
                }
            }
        }

        private void changeRole_Clicked(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Tag.ToString());
            var user = _context.Users.FirstOrDefault(x => x.Id.Equals(id));
            if(user.isModerator)
            {
                user.isModerator = false;
            }
            else user.isModerator= true;
            _context.Users.Update(user);
            _context.SaveChanges();
            GetUsers();
        }

        private void submitCart_Clicked(object sender, RoutedEventArgs e)
        {
            double _fullPrice = 0;
            var user = _context.Users.Include(x=>x.Cart).FirstOrDefault(x => x.Id.Equals(_id));
            var cart = _context.Carts.Include(x=>x.Items).FirstOrDefault(x => x.Id.Equals(user.CartId));
            foreach(var item in cart.Items)
            {
                _fullPrice += item.Price * item.Count;
            }
            var order = new Order()
            {
                Date = DateTime.Now,
                fullPrice=_fullPrice,
                User=user
            };
            order.OrderedItems = cart.Items.ToList();
            _context.Orders.Add(order);
           
            _context.SaveChanges();

            cart.Items.Clear();
            _context.SaveChanges();

            cartHolder.Children.Clear();
            var stack = new StackPanel();
            var textBlock = new TextBlock() { Text = "Zamówienie zrealizowane", FontSize = 25, Foreground=Brushes.Green };
            stack.Children.Add(textBlock);
            cartHolder.Children.Add(stack);
        }

        private void deleteProductFromCart_Clicked(object sender, RoutedEventArgs e)
        {
            var user = _context.Users.Include(x=>x.Cart).ThenInclude(x=>x.Items).FirstOrDefault(x => x.Id.Equals(_id));
            var id = int.Parse((sender as Button).Tag.ToString());
            var item = user.Cart.Items.FirstOrDefault(x => x.Id.Equals(id));
            
            if(item.Count > 1)
            {
                item.Count--;
            }
            else
            {
                user.Cart.Items.Remove(item);
            }
            
            _context.Users.Update(user);
            _context.SaveChanges();
            GetCart();
        }

        private void deleteProductFromList_Clicked(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Tag.ToString());
            var item = _context.Items.FirstOrDefault(x => x.Id.Equals(id));
            var users = _context.Users.Include(x => x.Cart).ThenInclude(x => x.Items).ToList();
            
            //var list = cart.Items.Where(x => x.Id.Equals(id));
            foreach(var user in users)
            {
                foreach(var i in user.Cart.Items.ToList())
                {
                    if(i.Equals(item))
                    {
                        user.Cart.Items.Remove(item);
                        _context.Users.Update(user);
                    }                  
                }
            }
          
            _context.Items.Remove(item);
            _context.SaveChanges();
            GetProducts();
        }

        private void addToCart_Clicked(object sender, RoutedEventArgs e)
        {

            var id = int.Parse((sender as Button).Tag.ToString());
            var item = _context.Items.FirstOrDefault(x => x.Id.Equals(id));
            var user = _context.Users.Include(x => x.Cart).ThenInclude(x => x.Items).FirstOrDefault(x => x.Id.Equals(_id));
            var cart = _context.Carts.Include(x => x.Items).FirstOrDefault(x => x.Id.Equals(user.CartId));
            
            if(cart.Items.IndexOf(item) < 0)
            {
                cart.Items.Add(item);
                item.Count = 1;
            }
            else
            {
                foreach (Item ci in cart.Items)
                {
                    if (ci.Equals(item))
                    {
                        ci.Count++;
                    }
                }
            }
            
            

            _context.Carts.Update(cart);
            _context.SaveChanges();


            (sender as Button).Content = $"Dodaj do koszyka \nPomyślnie dodano produkt \nIlość w koszyku: [{item.Count}]";
        }

        private void items_Clicked(object sender, RoutedEventArgs e)
        {
            GetProducts();
            productsPanel.Visibility = Visibility.Visible;
            addproductPanel.Visibility = Visibility.Hidden;
            cartPanel.Visibility = Visibility.Hidden;
            ordersPanel.Visibility = Visibility.Hidden;
            usersPanel.Visibility = Visibility.Hidden;
        }
        private void cart_Clicked(object sender, RoutedEventArgs e)
        {
            GetCart();
            cartPanel.Visibility = Visibility.Visible;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
            ordersPanel.Visibility = Visibility.Hidden;
            usersPanel.Visibility = Visibility.Hidden;
        }
        private void orders_Clicked(object sender, RoutedEventArgs e)
        {
            GetOrders();
            ordersPanel.Visibility = Visibility.Visible;
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
            usersPanel.Visibility = Visibility.Hidden;

        }
        private void addProduct_Clicked(object sender, RoutedEventArgs e)
        {
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Visible;
            ordersPanel.Visibility = Visibility.Hidden;
            usersPanel.Visibility = Visibility.Hidden;
        }
        private void users_Clicked(object sender, RoutedEventArgs e)
        {
            GetUsers();
            usersPanel.Visibility = Visibility.Visible;
            cartPanel.Visibility = Visibility.Hidden;
            productsPanel.Visibility = Visibility.Hidden;
            addproductPanel.Visibility = Visibility.Hidden;
            ordersPanel.Visibility = Visibility.Hidden;
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
                    _context.Items.Add(item);
                    _context.SaveChanges();
                    error.Text = "Pomyślnie dodano przedmiot";
                    error.Foreground = Brushes.Green;
                    if (error.Visibility == Visibility.Hidden)
                    {
                        error.Visibility = Visibility.Visible;
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
