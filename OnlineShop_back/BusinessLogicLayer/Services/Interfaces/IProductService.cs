using Addition.DTO;
using Addition.Mutual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IProductService
    {
        Answer<ProductDTO> GetProduct(int id, string? includeProperties = null);
        Answer<bool> AddProduct(NewProductDTO itemDTO, string filePath);
        Answer<bool> UpdateProduct(ProductDTO itemDTO);
        Answer<IEnumerable<ProductDTO>> GetAll(string? includeProperties = null);
        Answer<IEnumerable<ProductDTO>> GetByUser(int UserId, string? includeProperties = null);
        Answer<bool> DeleteProduct(int id);
    }
}
