using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using AutoMapper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccesLayer.Repository.IRepository;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SrvcImplementations
{
    public class ProductService:IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public Answer<bool> AddProduct(NewProductDTO itemDTO, string filePath)
        {
            Product product = _mapper.Map<Product>(itemDTO);
            product.PictureUrl = filePath;
            try
            {
                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();

                return new Answer<bool>(true, Feedback.OK, "Item added successfully");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while adding an item:" + ex.Message);
            }
        }

        public Answer<bool> DeleteProduct(int id)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            return new Answer<bool>(true, Feedback.OK, "Item deleted");
        }

        public Answer<IEnumerable<ProductDTO>> GetAll(string? includeProperties = null)
        {
            var list = _unitOfWork.Product.GetAll(includeProperties: includeProperties);
            var retList = new List<ProductDTO>();
            foreach (var product in list)
            {
                ProductDTO retItem = _mapper.Map<ProductDTO>(product);
                byte[] imageBytes = System.IO.File.ReadAllBytes(product.PictureUrl);
                retItem.PictureUrl = Convert.ToBase64String(imageBytes);

                retList.Add(retItem);

            }

            return new Answer<IEnumerable<ProductDTO>>(retList, Feedback.OK, "All items");
        }

        public Answer<IEnumerable<ProductDTO>> GetByUser(int UserId, string? includeProperties = null)
        {
            var list = _unitOfWork.Product.GetAll(u => u.UserId == UserId, includeProperties: includeProperties);
            var retList = new List<ProductDTO>();
            foreach (var product in list)
                retList.Add(_mapper.Map<ProductDTO>(product));

            return new Answer<IEnumerable<ProductDTO>>(retList, Feedback.OK, "All items from User");
        }

        public Answer<ProductDTO> GetProduct(int id, string? includeProperties = null)
        {
            Product i = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, includeProperties: "Images,FollowedItems");

            if (i == null)
                return new Answer<ProductDTO>(null, Feedback.NotFound, "Item not found");
            else
            {
                ProductDTO retItem = _mapper.Map<ProductDTO>(i);

                return new Answer<ProductDTO>(retItem, Feedback.OK, "Item found");
            }
        }

        public Answer<bool> UpdateProduct(ProductDTO itemDTO)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == itemDTO.Id);
            product = _mapper.Map<Product>(itemDTO);

            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();

            return new Answer<bool>(true, Feedback.OK, "Item changed");
        }
    }
}
