using CoeurApi.App.Modules.Users.DTOs;
using CoeurApi.App.Modules.Users.Models;
using CoeurApi.App.Modules.Users.Services;
using CoeurApi.App.Shared.Exceptions;
using CoeurApi.App.Shared.Interfaces;
using Moq;

namespace CoeurApi.Tests.Modules.Users;

public class UsersServiceTests
{
    private readonly Mock<IUsersRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    private UsersService CreateService() => new(_repository.Object, _unitOfWork.Object, _currentUser.Object);

    [Fact]
    public async Task CreateAsync_ComEmailJaExistente_DeveLancarConflict()
    {
        _repository.Setup(r => r.ExistsByEmailAsync("existente@teste.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();
        var dto = new CreateUserDto("Fulano", "existente@teste.com", "senha12345");

        var ex = await Assert.ThrowsAsync<AppException>(() => service.CreateAsync(dto));

        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarUsuarioESalvar()
    {
        _repository.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();
        var dto = new CreateUserDto("Fulano", "novo@teste.com", "senha12345");

        var result = await service.CreateAsync(dto);

        Assert.Equal("novo@teste.com", result.Email);
        _repository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UsuarioTentandoAcessarOutroPerfil_DeveLancarForbidden()
    {
        _currentUser.Setup(c => c.Id).Returns(Guid.NewGuid());
        _currentUser.Setup(c => c.IsAdmin).Returns(false);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<AppException>(() => service.GetByIdAsync(Guid.NewGuid()));

        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_Admin_DevePermitirAcessarQualquerUsuario()
    {
        var targetUser = User.Create("Fulano", "fulano@teste.com", "hash");
        _currentUser.Setup(c => c.Id).Returns(Guid.NewGuid());
        _currentUser.Setup(c => c.IsAdmin).Returns(true);
        _repository.Setup(r => r.GetByIdAsync(targetUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);

        var service = CreateService();
        var result = await service.GetByIdAsync(targetUser.Id);

        Assert.Equal(targetUser.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UsuarioNaoEncontrado_DeveLancarNotFound()
    {
        var id = Guid.NewGuid();
        _currentUser.Setup(c => c.Id).Returns(id);
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        var ex = await Assert.ThrowsAsync<AppException>(() => service.GetByIdAsync(id));

        Assert.Equal(404, ex.StatusCode);
    }
}
