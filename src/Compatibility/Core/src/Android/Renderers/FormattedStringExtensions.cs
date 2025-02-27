﻿using System.Text;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Widget;
using Color = Microsoft.Maui.Graphics.Color;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	public static class FormattedStringExtensions
	{
		public static SpannableString ToAttributed(this FormattedString formattedString, Font defaultFont, Color defaultForegroundColor, TextView view)
		{
			if (formattedString == null)
				return null;

			var builder = new StringBuilder();
			for (int i = 0; i < formattedString.Spans.Count; i++)
			{
				Span span = formattedString.Spans[i];
				var text = span.Text;
				if (text == null)
					continue;

				builder.Append(text);
			}

			var spannable = new SpannableString(builder.ToString());

			var c = 0;
			for (int i = 0; i < formattedString.Spans.Count; i++)
			{
				Span span = formattedString.Spans[i];
				var text = span.Text;
				if (text == null)
					continue;

				int start = c;
				int end = start + text.Length;
				c = end;

				if (span.TextColor != null)
				{
					spannable.SetSpan(new ForegroundColorSpan(span.TextColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}
				else if (defaultForegroundColor != null)
				{
					spannable.SetSpan(new ForegroundColorSpan(defaultForegroundColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}

				if (span.BackgroundColor != null)
				{
					spannable.SetSpan(new BackgroundColorSpan(span.BackgroundColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}
				if (span.LineHeight >= 0)
				{
					spannable.SetSpan(new LineHeightSpan(view, span.LineHeight), start, end, SpanTypes.InclusiveExclusive);
				}
				if (!span.IsDefault())
				{
					var spanFont = Font.OfSize(span.FontFamily, span.FontSize).WithAttributes(span.FontAttributes);
					spannable.SetSpan(new FontSpan(spanFont, view, span.CharacterSpacing.ToEm()), start, end, SpanTypes.InclusiveInclusive);
				}
				else
					spannable.SetSpan(new FontSpan(defaultFont, view, span.CharacterSpacing.ToEm()), start, end, SpanTypes.InclusiveInclusive);
				if (span.IsSet(Span.TextDecorationsProperty))
					spannable.SetSpan(new TextDecorationSpan(span), start, end, SpanTypes.InclusiveInclusive);

			}
			return spannable;
		}

		class FontSpan : MetricAffectingSpan
		{
			public FontSpan(Font font, TextView view, float characterSpacing)
			{
				Font = font;
				TextView = view;
				CharacterSpacing = characterSpacing;
			}

			public Font Font { get; }

			public TextView TextView { get; }

			public float CharacterSpacing { get; }

			public override void UpdateDrawState(TextPaint tp)
			{
				Apply(tp);
			}

			public override void UpdateMeasureState(TextPaint p)
			{
				Apply(p);
			}

			void Apply(Paint paint)
			{
				paint.SetTypeface(Font.ToTypeface());
				float value = (float)Font.Size;

				paint.TextSize = TypedValue.ApplyDimension(
					Font.AutoScalingEnabled ? ComplexUnitType.Sp : ComplexUnitType.Dip,
					value, TextView.Resources.DisplayMetrics);

				paint.LetterSpacing = CharacterSpacing;
			}
		}

		class TextDecorationSpan : MetricAffectingSpan
		{
			public TextDecorationSpan(Span span)
			{
				Span = span;
			}

			public Span Span { get; }

			public override void UpdateDrawState(TextPaint tp)
			{
				Apply(tp);
			}

			public override void UpdateMeasureState(TextPaint p)
			{
				Apply(p);
			}

			void Apply(Paint paint)
			{
				var textDecorations = Span.TextDecorations;
				paint.UnderlineText = (textDecorations & TextDecorations.Underline) != 0;
				paint.StrikeThruText = (textDecorations & TextDecorations.Strikethrough) != 0;
			}
		}

		class LineHeightSpan : Java.Lang.Object, ILineHeightSpan
		{
			private double _lineHeight;
			private int _ascent;
			private int _descent;

			public LineHeightSpan(TextView view, double lineHeight)
			{
				_lineHeight = lineHeight;
				Paint.FontMetricsInt fm = view.Paint.GetFontMetricsInt();
				_ascent = fm.Ascent;
				_descent = fm.Descent;
			}

			public void ChooseHeight(Java.Lang.ICharSequence text, int start, int end, int spanstartv, int v, Paint.FontMetricsInt fm)
			{
				fm.Ascent = (int)(_ascent * _lineHeight);
				fm.Descent = (int)(_descent * _lineHeight);
			}
		}
	}
}