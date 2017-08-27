using System.Linq;
using ProductManager.Mapper;
using ProductManager.Models;
using ProductManager.Models.Entity;

namespace ProductManager.Service
{
    /// <summary>
    /// Service class used to access entity framework (database) objects.
    /// </summary>
    public class ProductService
    {
        /// <summary>
        /// Creates/Updates a new product in database.
        /// </summary>
        /// <param name="userName">The current logged user name</param>
        /// <param name="product">The ProductModel object to create</param>
        /// <param name="isUpdate">Flag indicating if request comes from Update page</param>
        /// <returns>Boolean indicating if record was created/updated</returns>
        public bool CreateProduct(string userName, ProductModel product, bool isUpdate = false)
        {
            var entity = ProductMapper.MapProductModelToProductEntity(product);

            var entities = new ProductManagerEntities1();
            var existingProduct = entities.PRODUCTs.Find(product.Number);
            var userEntity = entities.APPUSERs.Where(u => u.USERNAME == userName).ToList();
            if (existingProduct == null && userEntity?.Count == 1)
            {
                entities.PRODUCTs.Add(entity);

                entities.USERPRODUCTs.Add(new USERPRODUCT() {PRODUCT = entity, APPUSER = userEntity[0]});

                return entities.SaveChanges() > 0;
            }
            else if (existingProduct != null && isUpdate)
            {
                existingProduct.TITLE = product.Title;
                existingProduct.SKU = product.SKU;
                existingProduct.PRICE = product.Price;

                return entities.SaveChanges() > 0;
            }

            return false;
        }

        /// <summary>
        /// Gets product list associated to logged user.
        /// </summary>
        /// <param name="userName">The current logged user name</param>
        /// <returns>The ProductsModel object containing product list.</returns>
        public ProductsModel GetProductsModel(string userName)
        {
            var entities = new ProductManagerEntities1();
            var userEntity = entities.APPUSERs.Where(u => u.USERNAME == userName).ToList();
            if (userEntity?.Count == 1)
            {
                var products = entities.USERPRODUCTs.Where(u => u.APPUSER.USERNAME == userName).ToList();
                var productsModel = new ProductsModel();
                foreach (var prod in products)
                {
                    productsModel.AddProduct(ProductMapper.MapProductEntityToProductModel(prod.PRODUCT));
                }

                return productsModel;
            }

            return new ProductsModel();
       }

        /// <summary>
        /// Gets a specific ProductModel using productNumber.
        /// </summary>
        /// <param name="productNumber">The product number to get object</param>
        /// <returns>Existing product model or new empty product model.</returns>
        public ProductModel GetProductModel(string productNumber)
        {
            var entities = new ProductManagerEntities1();
            var productEntity = entities.PRODUCTs.Where(u => u.NUMBER == productNumber).ToList();
            if (productEntity?.Count == 1)
            {
                return ProductMapper.MapProductEntityToProductModel(productEntity[0]);
            }

            return new ProductModel();
        }

        /// <summary>
        /// Removes product from database.
        /// </summary>
        /// <param name="userName">The current logged user name</param>
        /// <param name="productNumber">The product number to remove</param>
        /// <returns>Boolean indicating if product was removed</returns>
        public bool RemoveProduct(string userName, string productNumber)
        {
            var entities = new ProductManagerEntities1();
            var productEntity = entities.PRODUCTs.Find(productNumber);
            var userEntity = entities.APPUSERs.Where(u => u.USERNAME == userName).ToList();
            if (productEntity != null && userEntity.Count == 1)
            {
                var userProductEntity =
                    entities.USERPRODUCTs.Where(
                        x => x.PRODUCTNUMBER == productEntity.NUMBER).ToList();

                entities.PRODUCTs.Remove(productEntity);
                if (userProductEntity.Count == 1)
                    entities.USERPRODUCTs.Remove(userProductEntity[0]);

                return entities.SaveChanges() > 0;
            }

            return false;
        }
    }
}