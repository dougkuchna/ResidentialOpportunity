using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Infrastructure.Data;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class SubmitRequest
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private AppDbContext DbContext { get; set; } = default!;

    private CreateServiceRequestCommand _command = new()
    {
        IssueCategory = IssueCategory.Other,
        UrgencyLevel = UrgencyLevel.Standard
    };

    private bool _isSubmitting;
    private string? _errorMessage;
    private string? _uploadMessage;
    private Guid? _customerId;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                var customer = await DbContext.Customers
                    .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

                if (customer is not null)
                {
                    _customerId = customer.Id;
                    _command.Name = customer.ContactInfo.Name;
                    _command.Email = customer.ContactInfo.Email;
                    _command.Phone = customer.ContactInfo.Phone;
                }
            }
        }
        catch { /* anonymous user, no prefill */ }
    }

    private async Task HandleSubmit()
    {
        _isSubmitting = true;
        _errorMessage = null;

        try
        {
            var result = await RequestService.CreateAsync(_command, _customerId);
            Navigation.NavigateTo($"/request-confirmation/{result.Id}");
        }
        catch (FluentValidation.ValidationException ex)
        {
            _errorMessage = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            _errorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        _uploadMessage = null;
        _errorMessage = null;

        try
        {
            var file = e.File;
            if (file.Size > 1024 * 100) // 100KB max
            {
                _errorMessage = "File too large. Maximum 100KB.";
                return;
            }

            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 100);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            CreateServiceRequestCommand? parsed = null;

            if (file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                parsed = JsonSerializer.Deserialize<CreateServiceRequestCommand>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else if (file.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                var serializer = new XmlSerializer(typeof(CreateServiceRequestCommand));
                using var stringReader = new StringReader(content);
                parsed = serializer.Deserialize(stringReader) as CreateServiceRequestCommand;
            }

            if (parsed is not null)
            {
                _command = parsed;
                _uploadMessage = $"Fields pre-filled from {file.Name}";
            }
            else
            {
                _errorMessage = "Could not parse file. Ensure it matches the expected format.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error reading file: {ex.Message}";
        }
    }
}
