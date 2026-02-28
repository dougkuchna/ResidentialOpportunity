using FluentValidation;
using Moq;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Application.Validators;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Application.Tests.Services;

public class ServiceRequestServiceTests
{
    private readonly Mock<IServiceRequestRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly IValidator<CreateServiceRequestCommand> _validator = new CreateServiceRequestValidator();
    private readonly ServiceRequestService _service;

    public ServiceRequestServiceTests()
    {
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _service = new ServiceRequestService(_repoMock.Object, _uowMock.Object, _validator);
    }

    private static CreateServiceRequestCommand ValidCommand => new()
    {
        Name = "John Doe",
        Email = "john@example.com",
        Phone = "555-123-4567",
        Street = "123 Main St",
        City = "Springfield",
        State = "IL",
        ZipCode = "62704",
        IssueDescription = "AC not cooling properly",
        IssueCategory = IssueCategory.Cooling,
        UrgencyLevel = UrgencyLevel.Standard
    };

    [Fact]
    public async Task CreateAsync_WithValidCommand_ReturnsDto()
    {
        var result = await _service.CreateAsync(ValidCommand);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("AC not cooling properly", result.IssueDescription);
        Assert.Equal(RequestStatus.Submitted, result.Status);
    }

    [Fact]
    public async Task CreateAsync_WithValidCommand_CallsRepositoryAndUoW()
    {
        await _service.CreateAsync(ValidCommand);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCommand_ThrowsValidationException()
    {
        var cmd = ValidCommand;
        cmd.Name = ""; // Required field missing

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(cmd));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCommand_DoesNotCallRepository()
    {
        var cmd = ValidCommand;
        cmd.Email = ""; // Required field missing

        try { await _service.CreateAsync(cmd); } catch { }

        _repoMock.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithCustomerId_PassesCustomerIdThrough()
    {
        var customerId = Guid.NewGuid();
        ServiceRequest? captured = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ServiceRequest, CancellationToken>((sr, _) => captured = sr);

        await _service.CreateAsync(ValidCommand, customerId);

        Assert.NotNull(captured);
        Assert.Equal(customerId, captured!.CustomerId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var entity = ServiceRequest.Create(
            new ContactInfo("Test", "test@test.com", "555-0000"),
            new Address("1 St", "City", "IL", "60601"),
            "Test issue", IssueCategory.Other, UrgencyLevel.Standard);

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ServiceRequest?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsMatchingRequests()
    {
        var entities = new List<ServiceRequest>
        {
            ServiceRequest.Create(
                new ContactInfo("Test", "test@test.com", "555-0000"),
                new Address("1 St", "City", "IL", "60601"),
                "Issue 1", IssueCategory.Cooling, UrgencyLevel.Standard),
            ServiceRequest.Create(
                new ContactInfo("Test", "test@test.com", "555-0000"),
                new Address("2 St", "City", "IL", "60601"),
                "Issue 2", IssueCategory.Heating, UrgencyLevel.Urgent)
        };

        _repoMock.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var results = await _service.GetByEmailAsync("test@test.com");

        Assert.Equal(2, results.Count);
    }
}
