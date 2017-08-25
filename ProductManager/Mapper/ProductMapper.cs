using ProductManager.Models;
using ProductManager.Models.Entity;

namespace ProductManager.Mapper
{
    /// <summary>
    /// Maps ProductModel objects and PRODUCT entities.
    /// </summary>
    public class ProductMapper
    {
        /// <summary>
        /// Maps a PRODUCT entity to a ProductModel object.
        /// </summary>
        /// <param name="productEntity">The PRODUCT object to map</param>
        /// <returns>The mapped ProductModel object.</returns>
        public static ProductModel MapProductEntityToProductModel(PRODUCT productEntity)
        {
            return new ProductModel(
                productEntity.NUMBER.Trim(), productEntity.SKU.Trim(), productEntity.TITLE.Trim(), productEntity.PRICE);
        }

        /// <summary>
        /// Maps a ProductModel object to a PRODUCT entity.
        /// </summary>
        /// <param name="productModel">The ProductModel object to map</param>
        /// <returns>The mapped PRODUCT entity.</returns>
        public static PRODUCT MapProductModelToProductEntity(ProductModel productModel)
        {
            var product = new PRODUCT
            {
                NUMBER = productModel.Number.Trim(),
                SKU = productModel.SKU.Trim(),
                TITLE = productModel.Title.Trim(),
                PRICE = productModel.Price
            };

            return product;
        }
    }
}
