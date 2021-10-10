using System;
using System.Collections.Generic;
using ReactUnity.Helpers;
using ReactUnity.Scheduling;
using ReactUnity.StyleEngine;
using ReactUnity.Styling;
using ReactUnity.UGUI.StateHandlers;
using UnityEngine;

namespace ReactUnity.UGUI
{
    public class UGUIContext : ReactContext
    {
        static public Dictionary<string, Func<string, string, UGUIContext, IReactComponent>> ComponentCreators { get; }
            = new Dictionary<string, Func<string, string, UGUIContext, IReactComponent>>
            {
                { "text", (tag, text, context) => new TextComponent(text, context, tag) },
                { "anchor", (tag, text, context) => new AnchorComponent(context) },
                { "view", (tag, text, context) => new ContainerComponent(context, tag) },
                { "button", (tag, text, context) => new ButtonComponent(context) },
                { "toggle", (tag, text, context) => new ToggleComponent(context) },
                { "label", (tag, text, context) => new LabelComponent(context, tag) },
                { "input", (tag, text, context) => new InputComponent(text, context) },
                { "scroll", (tag, text, context) => new ScrollComponent(context) },
                { "image", (tag, text, context) => new ImageComponent(context) },
                { "rawimage", (tag, text, context) => new RawImageComponent(context) },
                { "svg", (tag, text, context) => new SvgComponent(context) },
                { "render", (tag, text, context) => new RenderComponent(context) },
                { "object", (tag, text, context) => new ObjectComponent(context) },
                { "video", (tag, text, context) => new VideoComponent(context) },
                { "prefab", (tag, text, context) => new PrefabComponent(context) },
                { "portal", (tag, text, context) => new PortalComponent(context) },
                { "icon", (tag, text, context) => new IconComponent(text, context, tag) },
                { "style", (tag, text, context) => new StyleComponent(context, tag, text) },
            };


        public override Dictionary<string, Type> StateHandlers { get; }
            = new Dictionary<string, Type>()
            {
                { "active", typeof(ActiveStateHandler) },
                { "focus", typeof(FocusStateHandler) },
                { "focus-within", typeof(FocusWithinStateHandler) },
                { "focus-visible", typeof(FocusVisibleStateHandler) },
                { "hover", typeof(HoverStateHandler) },
            };

        public IconSet DefaultIconSet { get; }
        public Dictionary<string, IconSet> IconSets { get; } = new Dictionary<string, IconSet>() { };

        public override CursorSet CursorSet { get; }

        public static Func<string, string, UGUIContext, UGUIComponent> defaultCreator =
            (tag, text, context) => new ContainerComponent(context, tag);

        public static Func<string, UGUIContext, ITextComponent> textCreator =
            (text, context) => new TextComponent(text, context, "_text") { IsPseudoElement = true };

        public UGUIContext(
            RectTransform hostElement, GlobalRecord globals, ScriptSource script,
            IDispatcher dispatcher, ITimer timer, IMediaProvider mediaProvider,
            Action onRestart, List<IconSet> iconSets, IconSet defaultIconSet, CursorSet cursorSet
        ) : base(globals, script, dispatcher, timer, mediaProvider, onRestart, true)
        {
            Host = new HostComponent(hostElement, this);
            InsertStyle(ResourcesHelper.UseragentStylesheet?.text, -1);
            Host.ResolveStyle(true);

            if (iconSets != null)
            {
                if (iconSets.Count > 0) IconSets["default"] = iconSets[0];
                foreach (var ic in iconSets) IconSets[ic.Name] = ic;
            }

            CursorSet = cursorSet;
            DefaultIconSet = defaultIconSet;
            if (DefaultIconSet == null)
            {
                if (IconSets.TryGetValue("default", out var def)) DefaultIconSet = def;
            }
        }

        public override IReactComponent CreateComponent(string tag, string text)
        {
            IReactComponent res = null;
            if (ComponentCreators.TryGetValue(tag, out var creator))
                res = creator(tag, text, this);
            else res = defaultCreator(tag, text, this);
            return res;
        }

        public override ITextComponent CreateText(string text)
        {
            return textCreator(text, this);
        }

        public override IReactComponent CreatePseudoComponent(string tag)
        {
            var tc = new TextComponent("", this, tag);
            tc.IsPseudoElement = true;
            tc.GameObject.name = tag;
            return tc;
        }

        public override void PlayAudio(AudioClip clip)
        {
            var source = (Host as HostComponent).GetOrAddComponent<AudioSource>();
            source.PlayOneShot(clip);
        }
    }
}
