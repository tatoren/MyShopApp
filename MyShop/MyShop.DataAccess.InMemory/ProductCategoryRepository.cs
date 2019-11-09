using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<ProductCategory> productCategories;

        public ProductCategoryRepository()
        {
            productCategories = cache["productCategories"] as List<ProductCategory>;

            if (productCategories == null)
            {
                productCategories = new List<ProductCategory>();
            }
        }
        public void Commit()
        {
            cache["productCategories"] = productCategories;
        }
        public void Insert(ProductCategory productCategory)
        {
            productCategories.Add(productCategory);
        }
        public void Update(ProductCategory productCategory)
        {
        ProductCategory productToUpdate = productCategories.Find(p => p.ID == productCategory.ID);
            if (productToUpdate != null)
            {
                productToUpdate = productCategory;
            }
            else
            {
                throw new Exception("No product category found");
            }
        }
        public ProductCategory Find(string ID)
        {
            ProductCategory productCategoryToFind = productCategories.Find(p => p.ID == ID);
            if (productCategoryToFind != null)
            {
                return productCategoryToFind;
            }
            else
            {
                throw new Exception("No product category found");
            }
        }
        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }
        public void Delete(String ID)
        {
            ProductCategory productCategoryToDelete = productCategories.Find(p => p.ID == ID);
            if (productCategoryToDelete != null)
            {
                productCategories.Remove(productCategoryToDelete);
            }
            else
            {
                throw new Exception("No product category found");
            }
        }
    }
}
