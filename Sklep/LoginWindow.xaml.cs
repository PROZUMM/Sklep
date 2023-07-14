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
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        void Submit_Clicked(object sender, RoutedEventArgs e)
        {
            if(!username.Text.Equals(String.Empty) && !password.Equals(String.Empty))
            {
                using(var context = new SklepDbContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.Username.Equals(username.Text));
                    if (user != null)
                    {
                        if(user.Password.Equals(password.Password.ToString()))
                        {
                            //todo
                        }
                        else
                        {
                            error.Text = "Nieprawidłowe dane logowania!";
                            if(error.Visibility == Visibility.Hidden)
                            {
                                error.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        error.Text = "Nieprawidłowe dane logowania!";
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
                if (error.Visibility == Visibility.Hidden)
                {
                    error.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
