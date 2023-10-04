using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Why.TagHelpers;

/// <summary>
/// Generate HTML based on post formatting syntax.
/// </summary>
[HtmlTargetElement("rich-text", Attributes = "text")]
public class RichTextTagHelper : TagHelper
{
    // Required to be settable by Razor pages.
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public required string Text { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.Clear();
        output.Attributes.Add("class", "lead");

        var cursor = new Cursor(Text, output);
        while (!cursor.IsDone)
        {
            if (cursor.TryConsume("###"))
            {
                var content = cursor.ConsumeUntil("###");
                if (content.IsEmpty)
                {
                    output.Content.AppendHtml("###");
                    continue;
                }

                output.Content.AppendHtml("""<h2 class="display-6"><marquee>""");
                output.Content.Append(content.ToString());
                output.Content.AppendHtml("</marquee></h2>");

                cursor.Advance(3);
                cursor.TryConsumeNewLine();
            }
            else if (cursor.TryConsume("!["))
            {
                var url = cursor.ConsumeUntil("]").ToString();
                if (url.Length == 0 || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    output.Content.AppendHtml("![");
                    output.Content.Append(url);
                    continue;
                }

                output.Content.AppendHtml($"""<img alt="Attached image" src="{url}"/>""");

                cursor.Advance(1);
                cursor.TryConsumeNewLine();
            }
            else if (cursor.TryConsumeNewLine())
            {
                output.Content.AppendHtml("<br/>");
            }
            else
            {
                output.Content.Append(cursor.CurrentChar.ToString());
                cursor.Advance(1);
            }
        }
    }

    private ref struct Cursor
    {
        public ReadOnlySpan<char> Text { get; init; }
        public int CurrentIndex { get; set; }

        public bool IsDone => CurrentIndex == Text.Length;
        public char CurrentChar => Text[CurrentIndex];

        public Cursor(ReadOnlySpan<char> text, TagHelperOutput output)
        {
            Text = text;
        }

        /// <summary>
        /// Increment <see cref="CurrentIndex"/> by <paramref name="amount"/>.
        /// </summary>
        public void Advance(int amount)
        {
            CurrentIndex += amount;
        }

        /// <summary>
        /// If the text contains <paramref name="substring"/> starting at
        /// <see cref="CurrentIndex"/>, skip over it and return true,
        /// otherwise, return false.
        /// </summary>
        public bool TryConsume(ReadOnlySpan<char> substring)
        {
            if (CurrentIndex > Text.Length - substring.Length ||
                !Text.Slice(CurrentIndex, substring.Length)
                    .SequenceEqual(substring))
            {
                return false;
            }

            Advance(substring.Length);
            return true;

        }

        /// <summary>
        /// If there is a new line ('\n', '\r', '\r\n') at <see cref="CurrentIndex"/>,
        /// skip over it.
        /// </summary>
        /// <returns>True if there was a new line, false otherwise.</returns>
        public bool TryConsumeNewLine()
        {
            if (IsDone) return false;

            if (TryConsume("\r\n"))
            {
                return true;
            }

            if (CurrentChar is not ('\n' or '\r'))
            {
                return false;
            }

            Advance(1);
            return true;

        }

        /// <summary>
        /// Search for <paramref name="closing"/> starting at <see cref="CurrentIndex"/>.
        /// If found, advance up to it and return the skipped over part.
        /// Otherwise, return an empty span.
        /// </summary>
        public ReadOnlySpan<char> ConsumeUntil(ReadOnlySpan<char> closing)
        {
            var consumedLength = Text[CurrentIndex..].IndexOf(closing, StringComparison.Ordinal);
            if (consumedLength < 0)
            {
                return ReadOnlySpan<char>.Empty;
            }

            var result = Text.Slice(CurrentIndex, consumedLength);
            Advance(consumedLength);
            return result;
        }
    }
}
