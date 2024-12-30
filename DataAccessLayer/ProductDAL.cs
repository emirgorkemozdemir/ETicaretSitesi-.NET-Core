using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductDAL
    {
        public Product FindProductByID(int id)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                return context.Products.Find(id);
            }
        }

        public List<Product> GetProductsByPrice(string orderProduct, int? category, string? text, int? upperprice, int? lowerprice)
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {

                var query = context.Products.AsQueryable();

                if (category != null)
                {
                    query = query.Where(p => p.Categoryid == category);
                }

                if (text != null)
                {
                    query = query.Where(p => p.Title.Contains(text));
                }

                if (upperprice != null)
                {
                    query = query.Where(p => p.Price <= upperprice);
                }

                if (lowerprice != null)
                {
                    query = query.Where(p => p.Price >= lowerprice);
                }


                if (orderProduct == "asc")
                {
                    return query.OrderBy(p => p.Price).Take(50).ToList();
                }
                else if (orderProduct == "desc")
                {
                    return query.OrderByDescending(p => p.Price).Take(50).ToList();
                }
                else
                {
                    return query.Take(50).ToList();
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            using (EticaretDb2Context context = new EticaretDb2Context())
            {
                return context.Products.Take(50).ToList();
            }
        }
    }
}
