using System;
using System.Web.Mvc;
using ProductManager.Business.Cache;
using ProductManager.Models;
using ProductManager.Service;

namespace ProductManager.Controllers
{
    /// <summary>
    /// Manages all actions related to product creation, update, delete and list.
    /// </summary>
    public class ProductController : Controller
    {
        /// <summary>
        /// Constant used to save product list object in Session with this key.
        /// </summary>
        public static string PRODUCT_LIST_GUID_KEY = "PRODUCT_LIST_GUID_KEY";

        /// <summary>
        /// Constant used to save logged user name in Session with this key.
        /// </summary>
        public static string LOGGED_USER_NAME_KEY = "LOGGED_USER_NAME_KEY";

        /// <summary>
        /// Constant used to save use persistent storage flag in Session with this key.
        /// </summary>
        public static string USE_PERSISTENT_STORAGE_KEY = "USE_PERSISTENT_STORAGE_KEY";

        /// <summary>
        /// List all products associated with logged user.
        /// </summary>
        /// <returns>Action result instance.</returns>
        [Authorize]
        public ActionResult List()
        {
            InitData();

            var guid = Convert.ToString(Session[PRODUCT_LIST_GUID_KEY]);
            ProductsModel model;
            var useDb = Convert.ToBoolean(Session[USE_PERSISTENT_STORAGE_KEY]);
            if (useDb)
            {
                var service = new ProductService();
                model = service.GetProductsModel(Convert.ToString(Session[LOGGED_USER_NAME_KEY]));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(guid))
                    model = CacheManager.RetrieveObjectById(guid) as ProductsModel;
                else
                    model = new ProductsModel();
            }
           

            return View(model);
        }

        /// <summary>
        /// Returns Create Product view page.
        /// </summary>
        /// <returns>Action result instance.</returns>
        [Authorize]
        public ActionResult Create()
        {
            InitData();
            return View();
        }

        /// <summary>
        /// Creates object saving in cache or database according to selected persistent mode.
        /// </summary>
        /// <param name="model">ProductModel object ot save</param>
        /// <returns>Action result instance.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var guid = Convert.ToString(Session[PRODUCT_LIST_GUID_KEY]);
                var saveInDb = Convert.ToBoolean(Session[USE_PERSISTENT_STORAGE_KEY]);
                if (saveInDb)
                {
                    var service = new ProductService();
                    var wasCreated =
                        service.CreateProduct(Convert.ToString(Session[LOGGED_USER_NAME_KEY]), model);

                    if (wasCreated) return RedirectToAction("List");

                    ModelState.AddModelError("", "Product number already exists in database.");

                    model.IsUpdate = false;
                    return View(model);
                }
                else
                {
                    var products = CacheManager.RetrieveObjectById(guid) as ProductsModel;
                    products?.AddProduct(model);
                    CacheManager.CacheObject(products, guid);
                }

                return RedirectToAction("List");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Loads create view but loading selected product information to edit it.
        /// </summary>
        /// <param name="productNumber">The selected product number</param>
        /// <returns>Action result instance.</returns>
        [Authorize]
        public ActionResult Edit(string productNumber)
        {
            InitData();

            var guid = Convert.ToString(Session[PRODUCT_LIST_GUID_KEY]);
            var saveInDb = Convert.ToBoolean(Session[USE_PERSISTENT_STORAGE_KEY]);
            ProductModel model = null;
            if (saveInDb)
            {
                var service = new ProductService();
                model = service.GetProductModel(productNumber);
            }
            else
            {
                var products = CacheManager.RetrieveObjectById(guid) as ProductsModel;

                if (products == null || !products.HasProduct(productNumber)) return RedirectToAction("List");

                model = products.GetProduct(productNumber);
            }

            model.IsUpdate = true;
            return View("Create", model);
        }

        /// <summary>
        /// <returns>Updates product information in database or cache according to persistent mode.</returns>
        /// </summary>
        /// <param name="model">The ProductModel to update</param>
        /// <returns>Action result instance.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Update(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var guid = Convert.ToString(Session[PRODUCT_LIST_GUID_KEY]);
                var saveInDb = Convert.ToBoolean(Session[USE_PERSISTENT_STORAGE_KEY]);
                if (saveInDb)
                {
                    var service = new ProductService();
                    var wasUpdated =
                        service.CreateProduct(
                            Convert.ToString(Session[LOGGED_USER_NAME_KEY]), model, true);

                    if (wasUpdated) return RedirectToAction("List");

                    ModelState.AddModelError("", "There was an error updating record.");
                    return View("Create", model);
                }
                else
                {
                    var products = CacheManager.RetrieveObjectById(guid) as ProductsModel;

                    if (products == null || !products.HasProduct(model.Number)) return RedirectToAction("List");

                    var existingProduct = products.GetProduct(model.Number);
                    existingProduct.Title = model.Title;
                    existingProduct.SKU = model.SKU;
                    existingProduct.Price = model.Price;

                    products.AddProduct(existingProduct);

                    CacheManager.CacheObject(products, guid);
                }

                return RedirectToAction("List");
            }

            return View("Create");
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="productNumber">The product number to delete</param>
        /// <returns>Action result instance.</returns>
        [Authorize]
        public ActionResult Delete(string productNumber)
        {
            InitData();

            var guid = Convert.ToString(Session[PRODUCT_LIST_GUID_KEY]);
            var saveInDb = Convert.ToBoolean(Session[USE_PERSISTENT_STORAGE_KEY]);
            if (saveInDb)
            {
                var service = new ProductService();
                var wasDeleted = service.RemoveProduct(Convert.ToString(Session[LOGGED_USER_NAME_KEY]), productNumber);

                if (wasDeleted) return RedirectToAction("List");

                ModelState.AddModelError("", "There was an error deleting record.");
                return RedirectToAction("List");
            }
            else
            {
                var products = CacheManager.RetrieveObjectById(guid) as ProductsModel;

                if (products == null || !products.HasProduct(productNumber)) return RedirectToAction("List");

                var wasDeleted = products.RemoveProduct(productNumber);
                if (wasDeleted)
                {
                    CacheManager.CacheObject(products, guid);
                    return RedirectToAction("List");
                }

                ModelState.AddModelError("", "There was an error deleting record.");
                return RedirectToAction("List");
            }
        }

        /// <summary>
        /// Inits common data first time (After Login with empty Session and Cache).
        /// </summary>
        private void InitData()
        {
            if (CacheManager.ExistCachedObject(Session[PRODUCT_LIST_GUID_KEY] as string)) return;

            var products = new ProductsModel();
            CacheManager.CacheObject(products, products.Guid);
            Session.Add(PRODUCT_LIST_GUID_KEY, products.Guid);
            Session.Add(LOGGED_USER_NAME_KEY, TempData[LOGGED_USER_NAME_KEY]);
            Session.Add(USE_PERSISTENT_STORAGE_KEY, TempData[USE_PERSISTENT_STORAGE_KEY]);
        }
    }
}