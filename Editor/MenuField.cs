using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    public class MenuField : BaseField<string> {
        private GenericMenu _menu;
        private List<string> _menuItems;
        private VisualElement _visualInputContainer;
        private Label _popupTextElement;
        private VisualElement _arrowElement;

        public override string value {
            get { return base.value; }
            set {
                base.value = value;
                string[] pathParts = value.Split('/');
                _popupTextElement.text = pathParts[pathParts.Length - 1];
            }
        }

        public new class UxmlFactory : UxmlFactory<MenuField> { }

        public MenuField() : this("Menu", new List<string>()) { }

        public MenuField(string label, List<string> menuItemsWithPaths) : base(label, CreateVisualInput()) {
            _menuItems = menuItemsWithPaths;
            this.AddToClassList("unity-base-popup-field");
            this.AddToClassList("unity-popup-field");
            OnEnable();
        }

        public void OnEnable() {
            Debug.Log("MenuField OnEnable called");
            _visualInputContainer = this.Q<VisualElement>(className: "unity-popup-field__input");
            _popupTextElement = _visualInputContainer.Q<Label>(className: "unity-base-popup-field__text");
            _arrowElement = _visualInputContainer.Q<VisualElement>(className: "unity-base-popup-field__arrow");

            _visualInputContainer.RegisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
            _visualInputContainer.RegisterCallback<PointerDownEvent>(OnClick, TrickleDown.TrickleDown);
        }

        private static VisualElement CreateVisualInput() {
            var visualInputContainer = new VisualElement();
            visualInputContainer.AddToClassList("unity-base-field__input");
            visualInputContainer.AddToClassList("unity-base-popup-field__input");
            visualInputContainer.AddToClassList("unity-popup-field__input");
            visualInputContainer.name = "visualInputContainer";
            var popupTextElement = new Label();
            popupTextElement.AddToClassList("unity-text-element");
            popupTextElement.AddToClassList("unity-base-popup-field__text");
            popupTextElement.RemoveFromClassList("unity-label");
            visualInputContainer.Add(popupTextElement);
            popupTextElement.pickingMode = PickingMode.Ignore;
            popupTextElement.name = "popupTextElement";
            var arrowElement = new VisualElement();
            arrowElement.AddToClassList("unity-base-popup-field__arrow");
            visualInputContainer.Add(arrowElement);
            arrowElement.pickingMode = PickingMode.Ignore;
            arrowElement.name = "arrowElement";
            return visualInputContainer;
        }

        private void OnClick<T>(T evt){
            ShowMenu();
        }

        private void ShowMenu() {
            _menu = new GenericMenu();
            Debug.Log("Menu");
            foreach (string item in _menuItems) {
                _menu.AddItem(new GUIContent(item), false, OnMenuItemSelected, item);
            }

            Vector2 menuPosition = new Vector2(_visualInputContainer.layout.x, _visualInputContainer.layout.yMax);
            _menu.DropDown(new Rect(menuPosition, Vector2.zero));
        }

        private void OnMenuItemSelected(object userData) {
            string selectedValue = (string)userData;
            value = selectedValue;
        }

        private object GetVisualInput() {
            FieldInfo field = typeof(BaseField<string>).GetField("visualInput", BindingFlags.NonPublic | BindingFlags.Instance);
            return field.GetValue(this);
        }
    }
}