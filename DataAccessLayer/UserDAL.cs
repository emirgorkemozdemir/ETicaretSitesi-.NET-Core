using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer;

namespace DataAccessLayer
{
    public class UserDAL
    {
        public void Add(User kullanıcı)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                context.Users.Add(kullanıcı);
                context.SaveChanges();
            }
        }

        public bool Login(string username, string password)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                var selected_user = context.Users.Where(m => m.UserName == username && m.Password == password).FirstOrDefault();
                if (selected_user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int FindUserIDByMail(string mail)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                return context.Users.Where(user => user.Mail == mail).FirstOrDefault().UserId;
            }
        }

        public int FindUserIDByUserName(string username)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                return context.Users.Where(user => user.UserName == username).FirstOrDefault().UserId;
            }
        }

        public User MyProfile(int id)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                return context.Users.Find(id);
            }
        }
    }
}




