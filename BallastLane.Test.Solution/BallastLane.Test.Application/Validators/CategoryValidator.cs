using BallastLane.Test.Application.DTOs;

namespace BallastLane.Test.Application.Validators
{
    public class CategoryValidator
    {
        public void Validate(CategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Category name is required.", nameof(dto.Name));
        }
    }
}
