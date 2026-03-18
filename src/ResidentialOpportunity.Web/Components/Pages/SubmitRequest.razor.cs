using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Web.Configuration;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class SubmitRequest
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private IOptions<BrandingOptions> BrandingOptionsAccessor { get; set; } = default!;
    [Inject] private ILogger<SubmitRequest> Logger { get; set; } = default!;

    private BrandingOptions _branding = new();
    private CreateServiceRequestCommand _command = new()
    {
        IssueCategory = IssueCategory.Other,
        UrgencyLevel = UrgencyLevel.Standard
    };

    private bool _isSubmitting;
    private string? _errorMessage;
    private string? _uploadMessage;
    private ServiceRequestDto? _submittedRequest;

    protected override void OnInitialized()
    {
        _branding = BrandingOptionsAccessor.Value;
    }

    private async Task HandleSubmit()
    {
        _isSubmitting = true;
        _errorMessage = null;

        try
        {
            _submittedRequest = await RequestService.CreateAsync(_command);
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

    private void ResetForm()
    {
        _submittedRequest = null;
        _errorMessage = null;
        _uploadMessage = null;
        _command = new CreateServiceRequestCommand
        {
            IssueCategory = IssueCategory.Other,
            UrgencyLevel = UrgencyLevel.Standard
        };
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
