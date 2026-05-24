using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallastLane.Test.Tests.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnCategories()
        {
            //// Arrange
            //var repositoryMock = new Mock<ICategoryRepository>();

            //repositoryMock
            //    .Setup(x => x.GetAllAsync())
            //    .ReturnsAsync(new List<Category>
            //    {
            //new Category { Id = 1, Name = "Technology" }
            //    });

            //var service = new CategoryService(repositoryMock.Object);

            //// Act
            //var result = await service.GetAllAsync();

            //// Assert
            //Assert.Single(result);
        }
    }
}
