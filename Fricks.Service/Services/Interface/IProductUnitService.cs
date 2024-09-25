using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.ProductUnitModels;

namespace Fricks.Service.Services.Interface
{
    public interface IProductUnitService
    {
        public Task<ProductUnitModel> AddProductUnit(ProductUnitProcessModel ProductUnit);
        public Task<ProductUnitModel> UpdateProductUnit(int id, ProductUnitProcessModel ProductUnit);
        public Task<ProductUnitModel> DeleteProductUnit(int id);
        public Task<ProductUnitModel> GetProductUnitById(int id);
        public Task<List<ProductUnitModel>> GetAllProductUnit();
    }
}
