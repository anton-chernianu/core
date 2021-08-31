using System;
using ReactUnity.Helpers;
using UnityEngine.UIElements;

namespace ReactUnity.UIToolkit
{
    public class ButtonComponent<T> : UIToolkitComponent<T> where T : Button, new()
    {
        public ButtonComponent(UIToolkitContext context, string tag) : base(context, tag) { }

        public override Action AddEventListener(string eventName, Callback callback)
        {
            switch (eventName)
            {
                case "onButtonClick":
                    Action listener = () => callback.Call(this);
                    Element.clicked += listener;
                    return () => Element.clicked -= listener;
                default:
                    return base.AddEventListener(eventName, callback);
            }
        }
    }
}