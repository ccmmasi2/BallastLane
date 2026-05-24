using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Domain.Exceptions;

namespace BallastLane.Test.Application.Validators
{
    public class CategoryValidator
    {
        public void Validate(CategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Category name is required.");
        }
    }
}
