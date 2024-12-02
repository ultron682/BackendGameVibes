namespace BackendGameVibes.Tests.Controllers;

using Xunit;
using Moq;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class ForumControllerTests {
    private readonly Mock<IForumPostService> _mockPostService;
    private readonly Mock<IForumThreadService> _mockThreadService;
    private readonly Mock<IForumRoleService> _mockRoleService;
    private readonly ForumController _controller;

    public ForumControllerTests() {
        _mockPostService = new Mock<IForumPostService>();
        _mockThreadService = new Mock<IForumThreadService>();
        _mockRoleService = new Mock<IForumRoleService>();

        _controller = new ForumController(_mockPostService.Object, _mockThreadService.Object, _mockRoleService.Object);
    }


}
