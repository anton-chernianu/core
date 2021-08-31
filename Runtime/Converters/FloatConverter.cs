using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ReactUnity.Styling;
using ReactUnity.Styling.Computed;
using UnityEngine;

namespace ReactUnity.Converters
{
    public class FloatConverter : IStyleParser, IStyleConverter
    {
        static CultureInfo culture = new CultureInfo("en-US");

        Dictionary<string, float> SuffixMap;
        Dictionary<string, Func<float, object>> SuffixMapper;
        bool AllowSuffixless;

        public FloatConverter()
        {
            SuffixMap = new Dictionary<string, float>();
            SuffixMapper = new Dictionary<string, Func<float, object>>();
            AllowSuffixless = true;
        }

        public FloatConverter(
            Dictionary<string, float> suffixMap,
            Dictionary<string, Func<float, object>> suffixMapper = null,
            bool allowSuffixless = true
        )
        {
            SuffixMap = suffixMap;
            SuffixMapper = suffixMapper ?? new Dictionary<string, Func<float, object>>();
            AllowSuffixless = allowSuffixless;
        }


        public object FromString(string value)
        {
            if (value == null) return CssKeyword.Invalid;
            return Parse(value);
        }

        public object Convert(object value)
        {
            if (value is float f) return f;
            if (value is int i) return (float) i;
            if (value is double d) return (float) d;
            return FromString(value?.ToString());
        }

        private object Parse(string value)
        {
            var i = 0;

            var numberPart = new StringBuilder();
            var suffixPart = new StringBuilder();
            var numberEnded = false;

            while (i < value.Length)
            {
                var c = value[i];
                if (!numberEnded && (char.IsDigit(c) || c == '.' || c == '+' || c == '-')) numberPart.Append(c);
                else
                {
                    numberEnded = true;
                    suffixPart.Append(c);
                }

                i++;
            }

            if (numberPart.Length > 0 && float.TryParse(numberPart.ToString(), NumberStyles.Float, culture, out var res))
            {
                var suffix = suffixPart.ToString();

                var multiplier = 1f;
                if (suffix != "")
                {
                    if (SuffixMapper.TryGetValue(suffix, out var mapper)) return mapper(res);
                    if (!SuffixMap.TryGetValue(suffix, out multiplier)) return CssKeyword.Invalid;
                }
                else if (!AllowSuffixless && res != 0)
                    return CssKeyword.Invalid;


                return res * multiplier;
            }
            else return CssKeyword.Invalid;
        }
    }

    public class PercentageConverter : FloatConverter
    {
        public PercentageConverter() : base(new Dictionary<string, float>
        {
            { "%", 0.01f },
        })
        { }
    }

    public class ColorValueConverter : FloatConverter
    {
        public ColorValueConverter() : base(new Dictionary<string, float>
        {
            { "%", 255 },
        })
        { }
    }

    public class LengthConverter : FloatConverter
    {
        public LengthConverter() : base(
            new Dictionary<string, float>
            {
                { "px", 1 },
                { "pt", 96f / 72f },
                { "pc", 16 },
                { "in", 96 },
                { "cm", 38 },
                { "mm", 3.8f },
                { "Q", 38f / 40f },
                { "%", 0.01f },
            },
            new Dictionary<string, Func<float, object>>
            {
                { "rem", x => new ComputedRootRelative(x, ComputedRootRelative.RootValueType.Rem) },
                { "vw", x => new ComputedRootRelative(x / 100f, ComputedRootRelative.RootValueType.Width) },
                { "vh", x => new ComputedRootRelative(x / 100f, ComputedRootRelative.RootValueType.Height) },
                { "vmin", x => new ComputedRootRelative(x / 100f, ComputedRootRelative.RootValueType.Min) },
                { "vmax", x => new ComputedRootRelative(x / 100f, ComputedRootRelative.RootValueType.Max) },
                { "em", x => new ComputedFontSize(x) },
                //{ "lh", x => new DynamicFontSizeValue(x) },
                //{ "ex", x => new DynamicFontSizeValue(x / 2) },
                //{ "ch", x => new DynamicFontSizeValue(x / 2) }
            }
        )
        { }
    }

    public class AngleConverter : FloatConverter
    {
        public AngleConverter() : base(new Dictionary<string, float>
        {
            { "deg", 1 },
            { "rad", 180 / Mathf.PI },
            { "grad", 400 / 360f },
            { "turn", 360 },
        })
        { }
    }

    public class DurationConverter : FloatConverter
    {
        public DurationConverter() : base(new Dictionary<string, float>
        {
            { "ms", 1 },
            { "s", 1000 },
        }, null, false)
        { }
    }
}