using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class ProductValidator
    {
        public void Validate(ProductDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Product name is required.");

            if (dto.CategoryId <= 0)
                throw new ValidationException("A valid category is required.");

            if (dto.Price <= 0)
                throw new ValidationException("Price must be greater than zero.");
        }
    }
}
