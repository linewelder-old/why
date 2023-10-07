using Microsoft.AspNetCore.Razor.TagHelpers;
using Why.TagHelpers;

namespace WhyTests;

public class RichTextTagHelperTests
{
    public static readonly IEnumerable<object[]> NewLines = new List<string[]>
    {
        new[] { "\n" },
        new[] { "\r" },
        new[] { "\r\n" }
    };

    [Fact]
    public void TextGetsHtmlEncoded()
    {
        AssertTransformation(
            "<h1>test</h1>",
            "&lt;h1&gt;test&lt;/h1&gt;");
    }

    [Fact]
    public void SurrogatePairsAreHandledCorrectly()
    {
        AssertTransformation(
            "\ud835\udd4f\ud835\udd50",
            "&#x1D54F;&#x1D550;");
    }

    [Theory, MemberData(nameof(NewLines))]
    public void NewLinesAreReplacedWithBr(string newLine)
    {
        AssertTransformation(
            $"{newLine}one{newLine}two{newLine}",
            "<br/>one<br/>two<br/>");
    }

    [Fact]
    public void Marquee_Works()
    {
        AssertTransformation(
            "###Marquee<3###",
            HtmlMarquee("Marquee&lt;3"));
    }

    [Fact]
    public void Marquee_UnclosedIsLeftAsIs()
    {
        AssertTransformation(
            "###Marquee",
            "###Marquee");
    }

    [Theory, MemberData(nameof(NewLines))]
    public void Marquee_NoExtraLineBreakAddedAfter(string newLine)
    {
        AssertTransformation(
            $"###Marquee###{newLine}some text",
            $"{HtmlMarquee("Marquee")}some text");
    }

    [Fact]
    public void Image_Works()
    {
        AssertTransformation(
            "![https://example.com/image.png]",
            HtmlImage("https://example.com/image.png"));
    }

    [Fact]
    public void Image_UnclosedIsLeftAsIs()
    {
        AssertTransformation(
            "![https://example.com/image.png",
            "![https://example.com/image.png");
    }

    [Theory, MemberData(nameof(NewLines))]
    public void Image_NoExtraLineBreakAddedAfter(string newLine)
    {
        AssertTransformation(
            $"###Marquee###{newLine}some text",
            $"{HtmlMarquee("Marquee")}some text");
    }

    [Fact]
    public void Image_WithIncorrectUrlIsLeftAsIs()
    {
        AssertTransformation(
            "![https://example.com/image.png\"]",
            "![https://example.com/image.png&quot;]");
    }

    private static void AssertTransformation(string markupString, string expectedContent)
    {
        var tagHelperContext = new TagHelperContext(
            tagName: "rich-text",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "div",
            attributes: new TagHelperAttributeList(),
            getChildContentAsync: (_, _) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        var tagHelper = new RichTextTagHelper
        {
            Text = markupString
        };

        tagHelper.Process(tagHelperContext, output);

        Assert.Equal("div", output.TagName);
        Assert.Equal(expectedContent, output.Content.GetContent());
    }

    private static string HtmlMarquee(string content) =>
        $"""<h2 class="display-6"><marquee>{content}</marquee></h2>""";

    private static string HtmlImage(string url) =>
        $"""<img alt="Attached image" src="{url}"/>""";
}
