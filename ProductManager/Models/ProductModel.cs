using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductManager.Models
{
    public class ProductModel
    {
        /// <summary>
        /// Represents the product number.
        /// </summary>
        [Required]
        public string Number{ get; set; }

        /// <summary>
        /// Represents the product SKU.
        /// </summary>
        [Required]
        public string SKU { get; set; }

        /// <summary>
        /// Represents the product title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Represent the product price.
        /// </summary>
        [Required]
        public double Price { get; set; }

        /// <summary>
        /// Indicates if model will be updated or created in storage structure.
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Constructor to set ProductModel properties.
        /// </summary>
        /// <param name="number">The product number</param>
        /// <param name="sku">The product sku</param>
        /// <param name="title">The product title</param>
        /// <param name="price">The product price</param>
        public ProductModel(string number, string sku, string title, double price)
        {
            this.Number = number;
            this.SKU = sku;
            this.Title = title;
            this.Price = price;
        }

        /// <summary>
        /// Empty constructor used in request reflection.
        /// </summary>
        public ProductModel() { }
    }
}