using MedicalAppointments.Web.Services.Api;

namespace MedicalAppointments.UnitTests.Web;

public sealed class ApiErrorTranslatorRegistrationTests
{
    [Fact]
    public void Duplicate_email_conflict_uses_registration_message()
    {
        var ex = new ApiException(
            "A user with this email already exists.",
            409,
            new ApiProblemDetails { Detail = "A user with this email already exists." });

        Assert.Equal(
            ApiErrorTranslator.DuplicateEmailRegistrationMessage(),
            ApiErrorTranslator.ToRegistrationUserMessage(ex));
    }

    [Fact]
    public void Registration_sign_in_failure_message_is_distinct_from_registration_failed()
    {
        Assert.NotEqual(
            ApiErrorTranslator.RegistrationFailedMessage(),
            ApiErrorTranslator.RegistrationAccountCreatedLoginRequiredMessage());
    }
}
