using DataAccessLayer;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ProductBL
    {
        ProductDAL data_access = new ProductDAL();
        public Product FindProductByID(int id)
        {
            return data_access.FindProductByID(id);
        }

        public List<Product> GetAllProducts()
        {
            return data_access.GetAllProducts();
        }

        public List<Product> GetProductsByPrice(string orderProduct, int? category, string? text, int? upperprice, int? lowerprice)
        {
            return data_access.GetProductsByPrice(orderProduct,category,text,upperprice,lowerprice);
        }

        public List<Category> GetCategories()
        {
            return data_access.ListCategories();
        }
    }
}
