using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallastLane.Test.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ProductValidator _validator;

        public ProductService(
            IProductRepository repository,
            ProductValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        //public async Task CreateAsync(ProductDTO dto)
        //{
        //    _validator.Validate(dto);

        //    await _repository.CreateAsync(dto);
        //}
    }
}
