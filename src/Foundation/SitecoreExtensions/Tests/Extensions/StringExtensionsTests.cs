/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.SitecoreExtensions.Extensions;
using Xunit;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Tests.Extensions
{
    public class StringExtensionsTests
  {
    [Theory]
    [InlineData("TestString", "Test String")]
    [InlineData("Test String", "Test String")]
    public void Humanize_ShouldReturnValueSplittedWithWhitespaces(string input, string expected)
    {
      input.Humanize().Should().Be(expected);
    }

    [Theory]
    [InlineData("  ", "none")]
    [InlineData("", "none")]
    [InlineData("somePath", "url('somePath')")]
    public void ToCssUrlValue_ShouldReturnValueSplittedWithWhitespaces(string input, string expected)
    {
      input.ToCssLinkValue().Should().Be(expected);
    }
  }
}