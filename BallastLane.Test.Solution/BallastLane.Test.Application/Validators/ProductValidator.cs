using BallastLane.Test.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallastLane.Test.Application.Validators
{
    public class ProductValidator
    {
        public void Validate(ProductDTO dto)
        {
            //if (string.IsNullOrWhiteSpace(dto.Name))
            //{
            //    throw new Exception("Product name is required.");
            //}

            //if (dto.Price <= 0)
            //{
            //    throw new Exception("Price must be greater than zero.");
            //}
        }
    }
}
