using Sklep.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        void Submit_Clicked(object sender, RoutedEventArgs e)
        {
            if(username.Text.Length > 3 && password.Password.ToString().Length > 3)
            {
                if(password.Password.ToString().Equals(secPassword.Password.ToString()))
                {
                    using(var context = new SklepDbContext())
                    {
                        if(context.Users.FirstOrDefault(x=>x.Username.Equals(username.Text))==null)
                        {
                            var cart = new Cart();
                            var user = new User()
                            {
                                Username = username.Text,
                                Password = secPassword.Password.ToString(),
                                isModerator = false,
                                Cart = cart
                            };
                            context.Carts.Add(cart);
                            context.Users.Add(user);

                            context.SaveChanges();
                            error.Text = "Konto utworzone pomyślnie. Możesz przejść do ekranu logowania!";
                            error.Foreground = Brushes.Green;
                            
                            if (error.Visibility == Visibility.Hidden)
                            {
                                error.Visibility = Visibility.Visible;
                            }                   
                        }
                        else
                        {
                            error.Text = "Taki użytkownik już istnieje!";
                            error.Foreground = Brushes.Red;
                            if (error.Visibility == Visibility.Hidden)
                            {
                                error.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                else
                {
                    error.Text = "Wprowadź poprawne dane!";
                    error.Foreground = Brushes.Red;
                    if (error.Visibility == Visibility.Hidden)
                    {
                        error.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                error.Text = "Wprowadź poprawne dane!";
                error.Foreground = Brushes.Red;
                if (error.Visibility == Visibility.Hidden)
                {
                    error.Visibility = Visibility.Visible;
                }
            }

        }
        void Back_Clicked(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.Show();
            this.Close();
        }
    }
}
