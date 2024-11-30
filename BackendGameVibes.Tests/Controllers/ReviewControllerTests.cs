using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Controllers {
    internal class ReviewControllerTests {
        private readonly ReviewController _controller;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        public ReviewControllerTests() {
            _reviewServiceMock = new Mock<IReviewService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ReviewController(_reviewServiceMock.Object, _mapperMock.Object);
        }

    }
}
