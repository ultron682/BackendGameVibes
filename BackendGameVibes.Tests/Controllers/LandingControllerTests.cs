using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Controllers {
    public class LandingControllerTests {
        private readonly LandingController _controller;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<IGameService> _gameServiceMock;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IForumThreadService> _forumThreadServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        public LandingControllerTests() {
            _cacheMock = new Mock<IMemoryCache>();
            _gameServiceMock = new Mock<IGameService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _forumThreadServiceMock = new Mock<IForumThreadService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new LandingController(
                _cacheMock.Object,
                _gameServiceMock.Object,
                _reviewServiceMock.Object,
                _forumThreadServiceMock.Object,
                _mapperMock.Object
            );
        }


    }
}
