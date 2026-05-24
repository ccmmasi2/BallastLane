using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class ProductValidator
    {
        public void Validate(ProductDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.", nameof(dto.Name));

            if (dto.CategoryId <= 0)
                throw new ArgumentException("A valid category is required.", nameof(dto.CategoryId));

            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(dto.Price));
        }
    }
}
