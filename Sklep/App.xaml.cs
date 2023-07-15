using Sklep.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sklep
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            using(var context = new SklepDbContext())
            {
                var cart = new Cart();
                var user = new User()
                {
                    Username = "admin",
                    Password = "admin",
                    isModerator = true,
                    Cart = cart
                };
                if(context.Users.FirstOrDefault(x=>x.Username.Equals(user.Username)) == null)
                {
                    context.Carts.Add(cart);
                    context.Users.Add(user);

                    context.SaveChanges();
                }
                var cart1 = new Cart();
                var user1 = new User()
                {
                    Username = "user",
                    Password = "user",
                    isModerator = false,
                    Cart = cart1
                };
                if (context.Users.FirstOrDefault(x => x.Username.Equals(user1.Username)) == null)
                {
                    context.Carts.Add(cart1);
                    context.Users.Add(user1);

                    context.SaveChanges();
                }

                var item1 = new Item()
                {
                    Name = "Jabłko",
                    Description = "Czerwone jabłko",
                    Price = 2.54
                };
                var item2 = new Item()
                {
                    Name = "Gruszka",
                    Description = "Zielona gruszka",
                    Price = 3.57
                };
                var item3 = new Item()
                {
                    Name = "Pietruszka",
                    Description = "Super pietruszka",
                    Price = 0.91
                };
                var item4 = new Item()
                {
                    Name = "Czereśnie",
                    Description = "Zwykłe czereśnie",
                    Price = 17.10
                };
                var item5 = new Item()
                {
                    Name = "Jagody",
                    Description = "Pyszne jagody",
                    Price = 7.19
                };
                var item6 = new Item()
                {
                    Name = "Wiśnie",
                    Description = "Wiśnie",
                    Price = 8.21
                };
                if (!context.Items.Any())
                {
                    context.Items.Add(item1);
                    context.Items.Add(item2);
                    context.Items.Add(item3);
                    context.Items.Add(item4);
                    context.Items.Add(item5);
                    context.Items.Add(item6);
                    context.SaveChanges();
                }

            }
        }
    }
}
