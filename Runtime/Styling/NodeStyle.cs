using Facebook.Yoga;
using ReactUnity.Styling.Types;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ReactUnity.Styling
{
    public class NodeStyle
    {
        Dictionary<string, object> StyleMap;
        public List<Dictionary<string, object>> CssStyles;
        public List<LayoutValue> CssLayouts;
        Dictionary<string, object> DefaultStyle;
        public bool HasInheritedChanges { get; private set; } = false;

        public NodeStyle Parent;
        public StateStyles StateStyles;

        #region Set/Get

        public float opacity
        {
            set => SetStyleValue(StyleProperties.opacity, value);
            get => GetStyleValue<float>(StyleProperties.opacity);
        }
        public int zOrder
        {
            set => SetStyleValue(StyleProperties.zOrder, value);
            get => GetStyleValue<int>(StyleProperties.zOrder);
        }
        public bool hidden
        {
            set => SetStyleValue(StyleProperties.hidden, value);
            get => GetStyleValue<bool>(StyleProperties.hidden);
        }
        public string cursor
        {
            set => SetStyleValue(StyleProperties.cursor, value);
            get => GetStyleValue<string>(StyleProperties.cursor);
        }
        public InteractionType interaction
        {
            set => SetStyleValue(StyleProperties.interaction, value);
            get => GetStyleValue<InteractionType>(StyleProperties.interaction);
        }
        public Color backgroundColor
        {
            set => SetStyleValue(StyleProperties.backgroundColor, value);
            get => GetStyleValue<Color>(StyleProperties.backgroundColor);
        }
        public object backgroundImage
        {
            set => SetStyleValue(StyleProperties.backgroundImage, value);
            get => GetStyleValue(StyleProperties.backgroundImage);
        }
        public int borderRadius
        {
            set => SetStyleValue(StyleProperties.borderRadius, value);
            get => GetStyleValue<int>(StyleProperties.borderRadius);
        }
        public Color borderColor
        {
            set => SetStyleValue(StyleProperties.borderColor, value);
            get => GetStyleValue<Color>(StyleProperties.borderColor);
        }
        public ShadowDefinition boxShadow
        {
            set => SetStyleValue(StyleProperties.boxShadow, value);
            get => GetStyleValue<ShadowDefinition>(StyleProperties.boxShadow);
        }
        public Vector2 translate
        {
            set => SetStyleValue(StyleProperties.translate, value);
            get => GetStyleValue<Vector2>(StyleProperties.translate);
        }
        public bool translateRelative
        {
            set => SetStyleValue(StyleProperties.translateRelative, value);
            get => GetStyleValue<bool>(StyleProperties.translateRelative);
        }
        public Vector2 scale
        {
            set => SetStyleValue(StyleProperties.scale, value);
            get => GetStyleValue<Vector2>(StyleProperties.scale);
        }
        public Vector2 pivot
        {
            set => SetStyleValue(StyleProperties.pivot, value);
            get => GetStyleValue<Vector2>(StyleProperties.pivot);
        }
        public float rotate
        {
            set => SetStyleValue(StyleProperties.rotate, value);
            get => GetStyleValue<float>(StyleProperties.rotate);
        }
        public TMP_FontAsset font
        {
            set => SetStyleValue(StyleProperties.font, value);
            get => GetStyleValue<TMP_FontAsset>(StyleProperties.font);
        }
        public Color fontColor
        {
            set => SetStyleValue(StyleProperties.fontColor, value);
            get => GetStyleValue<Color>(StyleProperties.fontColor);
        }
        public FontWeight fontWeight
        {
            set => SetStyleValue(StyleProperties.fontWeight, value);
            get => GetStyleValue<FontWeight>(StyleProperties.fontWeight);
        }
        public FontStyles fontStyle
        {
            set => SetStyleValue(StyleProperties.fontStyle, value);
            get => GetStyleValue<FontStyles>(StyleProperties.fontStyle);
        }
        public YogaValue fontSize
        {
            set => SetStyleValue(StyleProperties.fontSize, value);
            get => GetStyleValue<YogaValue>(StyleProperties.fontSize);
        }
        public TextAlignmentOptions textAlign
        {
            set => SetStyleValue(StyleProperties.textAlign, value);
            get => GetStyleValue<TextAlignmentOptions>(StyleProperties.textAlign);
        }
        public TextOverflowModes textOverflow
        {
            set => SetStyleValue(StyleProperties.textOverflow, value);
            get => GetStyleValue<TextOverflowModes>(StyleProperties.textOverflow);
        }
        public bool textWrap
        {
            set => SetStyleValue(StyleProperties.textWrap, value);
            get => GetStyleValue<bool>(StyleProperties.textWrap);
        }
        public string content
        {
            set => SetStyleValue(StyleProperties.content, value);
            get => GetStyleValue<string>(StyleProperties.content);
        }

        #endregion

        #region Resolved values

        public float fontSizeActual
        {
            get
            {
                if (HasValue("fontSize"))
                {
                    var fs = fontSize;
                    var unit = fs.Unit;

                    if (unit == YogaUnit.Undefined || unit == YogaUnit.Auto) return Parent?.fontSizeActual ?? 0;
                    if (unit == YogaUnit.Point) return fs.Value;
                    return (Parent?.fontSizeActual ?? 0) * fs.Value / 100;
                }

                return Parent?.fontSizeActual ?? 0;
            }
        }

        #endregion

        public NodeStyle()
        {
            StyleMap = new Dictionary<string, object>();
        }

        public NodeStyle(NodeStyle defaultStyle, StateStyles stateStyles) : this()
        {
            DefaultStyle = defaultStyle.StyleMap;
            StateStyles = stateStyles;
        }

        public void CopyStyle(NodeStyle copyFrom)
        {
            StyleMap = new Dictionary<string, object>(copyFrom.StyleMap);
        }

        public object GetStyleValue(IStyleProperty prop, bool fromChild = false)
        {
            if (fromChild) HasInheritedChanges = true;

            object value;
            var name = prop.name;

            if (
                !StyleMap.TryGetValue(name, out value) &&
                (CssStyles == null || !CssStyles.Any(x => x.TryGetValue(name, out value))) &&
                (DefaultStyle == null || !DefaultStyle.TryGetValue(name, out value)))
            {
                if (prop.inherited)
                {
                    return Parent?.GetStyleValue(prop, true) ?? prop?.defaultValue;
                }

                return prop?.defaultValue;
            }

            return GetStyleValueSpecial(value, prop);
        }

        private object GetStyleValueSpecial(object value, IStyleProperty prop)
        {
            if (Equals(value, SpecialNames.CantParse)) return null;
            else if (Equals(value, SpecialNames.Initial) || Equals(value, SpecialNames.Unset)) return prop?.defaultValue;
            else if (Equals(value, SpecialNames.Inherit)) return Parent?.GetStyleValue(prop) ?? prop?.defaultValue;
            return value;
        }

        public T GetStyleValue<T>(IStyleProperty prop)
        {
            var value = GetStyleValue(prop);
            return value == null ? default : (T) value;
        }


        public void SetStyleValue(IStyleProperty prop, object value)
        {
            var name = prop.name;
            object currentValue;

            if (!StyleMap.TryGetValue(name, out currentValue))
            {
                if (value == null) return;
            }

            var changed = currentValue != value;
            if (value == null)
            {
                StyleMap.Remove(name);
            }
            else
            {
                StyleMap[name] = value;
            }

            if (changed)
            {
                if (StyleProperties.GetStyleProperty(name).inherited) HasInheritedChanges = true;
            }
        }

        public void MarkChangesSeen()
        {
            HasInheritedChanges = false;
        }

        public bool HasValue(string name)
        {
            return StyleMap.ContainsKey(name) ||
                (CssStyles != null && CssStyles.Any(x => x.ContainsKey(name))) ||
                (DefaultStyle != null && DefaultStyle.ContainsKey(name));
        }
    }
}
