using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductManager.Models
{
    public class ProductsModel
    {
        /// <summary>
        /// Guid used to identify this object in cache.
        /// </summary>
        public string Guid { get; }

        /// <summary>
        /// Products dictionary structure. Product number is the dictionary key.
        /// </summary>
        private readonly Dictionary<string, ProductModel> _products;

        /// <summary>
        /// Constructor to initialize dictionary and Guid.
        /// </summary>
        public ProductsModel()
        {
            _products = new Dictionary<string, ProductModel>();
            Guid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Adds produc to dictionary, in case product already exists, replaces existing one with new one.
        /// </summary>
        /// <param name="product">The ProductModel object to add</param>
        public void AddProduct(ProductModel product)
        {
            if (_products.ContainsKey(product.Number))
            {
                _products[product.Number] = product;
            }
            else
            {
                _products.Add(product.Number, product);
            }
        }

        /// <summary>
        /// Checks if dictionary has product inside.
        /// </summary>
        /// <param name="productNumber">The product number to check</param>
        /// <returns></returns>
        public bool HasProduct(string productNumber)
        {
            return _products.ContainsKey(productNumber);
        }

        /// <summary>
        /// Gets the complete product list.
        /// </summary>
        /// <returns>The ProductModel list with all products for user.</returns>
        public List<ProductModel> GetProductList()
        {
            return _products.Values.ToList();
        }

        /// <summary>
        /// Gets a product using product number.
        /// </summary>
        /// <param name="productNumber">Product number to get object</param>
        /// <returns>ProductModel object associated with productNumber.</returns>
        public ProductModel GetProduct(string productNumber)
        {
            return _products[productNumber];
        }

        /// <summary>
        /// Removes a product from dictionary.
        /// </summary>
        /// <param name="productNumber">The product number to remove</param>
        /// <returns>Boolean indicating if product was removed.</returns>
        public bool RemoveProduct(string productNumber)
        {
            return _products.Remove(productNumber);
        }
    }
}