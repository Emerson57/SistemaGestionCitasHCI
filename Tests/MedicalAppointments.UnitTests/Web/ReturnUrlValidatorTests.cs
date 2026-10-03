using MedicalAppointments.Web.Auth;

namespace MedicalAppointments.UnitTests.Web;

public sealed class ReturnUrlValidatorTests
{
    [Theory]
    [InlineData("/paciente")]
    [InlineData("/medico/agenda")]
    public void IsSafeLocalReturnUrl_accepts_local_paths(string url)
    {
        Assert.True(ReturnUrlValidator.IsSafeLocalReturnUrl(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("//evil.example")]
    [InlineData("https://evil.example")]
    [InlineData("/\\evil")]
    public void IsSafeLocalReturnUrl_rejects_unsafe(string? url)
    {
        Assert.False(ReturnUrlValidator.IsSafeLocalReturnUrl(url));
    }

    [Fact]
    public void ParseFromQuery_reads_returnUrl_and_ReturnUrl()
    {
        Assert.Equal("/paciente", ReturnUrlValidator.ParseFromQuery("?returnUrl=%2Fpaciente"));
        Assert.Equal("/admin", ReturnUrlValidator.ParseFromQuery("?ReturnUrl=%2Fadmin"));
    }

    [Fact]
    public void NormalizeLocalPath_adds_leading_slash_for_relative_paths()
    {
        Assert.Equal("/paciente", ReturnUrlValidator.NormalizeLocalPath("paciente"));
        Assert.Equal("/paciente", ReturnUrlValidator.NormalizeLocalPath("/paciente"));
    }
}
