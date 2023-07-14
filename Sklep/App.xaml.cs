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
                var user = new User()
                {
                    Username = "admin",
                    Password = "admin",
                    isModerator = true,
                };
                if(context.Users.FirstOrDefault(x=>x.Username.Equals(user.Username)) == null)
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                var user1 = new User()
                {
                    Username = "user",
                    Password = "user",
                    isModerator = false,
                };
                if (context.Users.FirstOrDefault(x => x.Username.Equals(user1.Username)) == null)
                {
                    context.Users.Add(user1);
                    context.SaveChanges();
                }
            }
        }
    }
}
