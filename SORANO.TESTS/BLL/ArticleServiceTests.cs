using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.TESTS.BLL
{
    [TestClass]
    public class ArticleServiceTests
    {
        private static Mock<IUnitOfWork> _unitOfWorkMock;
        private static Mock<IStockRepository<Article>> _repositoryMock;
        private static IEnumerable<Article> _articles;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _articles = new List<Article>
            {
                new Article {ID = 1, Name = "Article #1", Barcode = "AAA", IsDeleted = false, DeliveryItems = new List<DeliveryItem>{new DeliveryItem()}},
                new Article {ID = 2, Name = "Article #2", Barcode = "BBB", IsDeleted = true},
                new Article {ID = 3, Name = "Article #3", Barcode = "CCC", IsDeleted = false}
            };

            
            _repositoryMock = new Mock<IStockRepository<Article>>();
            _repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_articles);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Article, bool>>>()))
                .ReturnsAsync((Expression<Func<Article, bool>> predicate) => _articles.SingleOrDefault(predicate.Compile()));
            _repositoryMock.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Article, bool>>>()))
                .ReturnsAsync((Expression<Func<Article, bool>> predicate) => _articles.Where(predicate.Compile()));
            _repositoryMock.Setup(r => r.Add(It.IsAny<Article>()))
                .Returns((Article article) => article);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Article>()))
                .Returns(It.IsAny<Article>());

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.Get<Article>())
                .Returns(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_WithDeleted()
        {            
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.GetAllAsync(true);

            Assert.IsNotNull(result);            
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.IsNotNull(result.Result);

            var list = result.Result.ToList();
            
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].ID, 1);
            Assert.AreEqual(list[0].Name, "Article #1");
            Assert.AreEqual(list[1].ID, 2);
            Assert.AreEqual(list[1].Name, "Article #2");
            Assert.AreEqual(list[2].ID, 3);
            Assert.AreEqual(list[2].Name, "Article #3");
        }

        [TestMethod]
        public async Task GetAllAsync_WithoutDeleted()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.GetAllAsync(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.IsNotNull(result.Result);

            var articles = result.Result.ToList();

            Assert.AreEqual(articles.Count, 2);
            Assert.AreEqual(articles[0].ID, 1);
            Assert.AreEqual(articles[0].Name, "Article #1");
            Assert.AreEqual(articles[1].ID, 3);
            Assert.AreEqual(articles[1].Name, "Article #3");
        }

        [TestMethod]
        public async Task GetAsync_Succes()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.GetAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.IsNotNull(result.Result);

            var article = result.Result;

            Assert.AreEqual(article.ID, 1);
            Assert.AreEqual(article.Name, "Article #1");
        }

        [TestMethod]
        public async Task GetAsync_NotFound()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.GetAsync(4);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.NotFound);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task CreateAsync_Success()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var article = new ArticleDto
            {
                ID = 4,
                Barcode = "DDD",
                Recommendations = new List<RecommendationDto>(),
                Attachments = new List<AttachmentDto>()
            };

            var result = await service.CreateAsync(article, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.AreEqual(result.Result, 4);
        }

        [TestMethod]
        public async Task CreateAsync_AlreadyExists()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var article = new ArticleDto
            {
                ID = 0,
                Barcode = "AAA"
            };

            var result = await service.CreateAsync(article, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.AlreadyExists);
            Assert.AreEqual(result.Result, 0);
        }

        [TestMethod]
        public async Task UpdateAsync_NotFound()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var article = new ArticleDto { ID = 4 };

            var result = await service.UpdateAsync(article, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.NotFound);
            Assert.AreEqual(result.Result, null);
        }

        [TestMethod]
        public async Task UpdateAsync_AlreadyExists()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var article = new ArticleDto
            {
                ID = 2,
                Barcode = "AAA"
            };

            var result = await service.UpdateAsync(article, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.AlreadyExists);
            Assert.AreEqual(result.Result, null);
        }

        [TestMethod]
        public async Task UpdateAsync_Success()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var article = new ArticleDto
            {
                ID = 1,
                Barcode = "DDD",
                Recommendations = new List<RecommendationDto>(),
                Attachments = new List<AttachmentDto>()
            };

            var result = await service.UpdateAsync(article, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.AreEqual(result.Result, null);
        }

        [TestMethod]
        public async Task DeleteAsync_NotFound()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.DeleteAsync(4, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.NotFound);
            Assert.AreEqual(result.Result, 0);
        }

        [TestMethod]
        public async Task DeleteAsync_InvalidOperation()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.DeleteAsync(1, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.InvalidOperation);
            Assert.AreEqual(result.Result, 0);
        }

        [TestMethod]
        public async Task DeleteAsync_Succes()
        {
            var service = new ArticleService(_unitOfWorkMock.Object);

            var result = await service.DeleteAsync(2, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ServiceResponseStatus.Success);
            Assert.AreEqual(result.Result, 2);
        }
    }
}
