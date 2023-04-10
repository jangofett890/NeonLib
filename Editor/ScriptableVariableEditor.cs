using NeonLib.Events;
using NeonLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    [CustomEditor(typeof(ScriptableVariable), true)]
    class ScriptableVariableEditor : UnityEditor.Editor {

        private ScriptableVariable _variable;
        private VisualElement _RootElement;
        private VisualTreeAsset _treeAsset;
        private DropdownField _typeDropdown;
        private VisualElement _valueContainer;

        public override VisualElement CreateInspectorGUI() {
            _treeAsset = Resources.Load<VisualTreeAsset>("UI/Editor/ScriptableVariableEditor");
            _variable = (ScriptableVariable)target;
            _RootElement = new VisualElement();
            _RootElement.styleSheets.Add(Resources.Load<StyleSheet>("UI/Editor/ScriptableVariableStyles"));
            _RootElement.Add(_treeAsset.CloneTree());
            _typeDropdown = _RootElement.Q<DropdownField>("ValueType");
            _valueContainer = _RootElement.Q<VisualElement>("ValueContainer");
            List<TypeMapping> typeMappings = GetTypeMappings();
            PopulateTypeDropdown(typeMappings);
            _typeDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
                _lastSelectedDisplayName = evt.newValue;
                Type selectedType = null;
                TypeMapping matchingMapping = typeMappings.FirstOrDefault(mapping => mapping.DisplayName == evt.newValue);
                selectedType = matchingMapping.ValueType;

                if (selectedType == null) {
                    selectedType = FindType(evt.newValue);
                }
                
                if (selectedType != null) {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(selectedType) || selectedType == typeof(UnityEngine.Object)) {
                        _variable.typeFullName = selectedType.AssemblyQualifiedName;
                        _variable.unityObjectValue = null;
                        _variable.jsonValue = null;
                    }
                    else {

                        if (selectedType == typeof(string)) {
                            _variable.Value = "";
                        }
                        else if (selectedType != typeof(Enum))
                            _variable.Value = Activator.CreateInstance(selectedType);

                        _variable.ValueType = selectedType;
                    }
                    UpdateValueContainer();

                    matchingMapping.OnTypeSelected?.Invoke();
                }
            });
            UpdateValueContainer();
            return _RootElement;
        }

        private string _lastSelectedDisplayName;

        private void PopulateTypeDropdown(List<TypeMapping> typeMappings) {
            List<string> options = new List<string>();

            //var projectTypes = AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(x => x.GetTypes())
            //    .Where(x => x.IsClass && !x.IsAbstract && x.IsSerializable && !options.Contains(x.FullName) && !typeof(UnityEngine.Object).IsAssignableFrom(x))
            //    .Select(x => x.FullName)
            //    .ToList();

            options.Add("--- Basic Types ---");
            options.AddRange(typeMappings.Select(mapping => mapping.DisplayName));
            //options.Add("--- Project Serializables ---");
            //options.AddRange(projectTypes);

            _typeDropdown.choices = options;
            if (_variable.typeFullName != "" && _variable.typeFullName != "Null" && _variable.typeFullName != null) {
                TypeMapping matchingMapping = typeMappings.FirstOrDefault(mapping => mapping.ValueType == _variable.ValueType);
                if(matchingMapping != null) 
                    _typeDropdown.SetValueWithoutNotify(matchingMapping.DisplayName);
                 else
                    _typeDropdown.SetValueWithoutNotify(_variable.ValueType.FullName);
            } else
                _typeDropdown.SetValueWithoutNotify(options[0]);
        }

        public void UpdateValueContainer() {
            _valueContainer.Clear();
            if (_variable.typeFullName == "" && _variable.typeFullName == "Null" && _variable.typeFullName == null)
                return;
            
            Type t = _variable.ValueType;
            if (typeof(UnityEngine.Object).IsAssignableFrom(t)) {
                ObjectField unityObjectField = new ObjectField { objectType = t, allowSceneObjects = true };

                if (_variable.unityObjectValue != null)
                    unityObjectField.value = _variable.unityObjectValue;

                unityObjectField.RegisterValueChangedCallback(evt => {
                    _variable.Value = evt.newValue;
                    _typeDropdown.SetValueWithoutNotify(_variable.ValueType.Name);
                });
                _valueContainer.Add(unityObjectField);
                Debug.Log("Object");
            }
            else {
                VisualElement fieldForValue = GetVisualElementForVariableAndRegisterCallback(_variable);
                _valueContainer.Add(fieldForValue);
                Debug.Log("CustomField");
            }
        }

        private VisualElement GetVisualElementForVariableAndRegisterCallback(ScriptableVariable variable) {
            List<TypeMapping> typeMappings = GetTypeMappings();
            TypeMapping matchingMapping;

            if (!string.IsNullOrEmpty(_lastSelectedDisplayName)) {
                matchingMapping = typeMappings.FirstOrDefault(mapping => mapping.DisplayName == _lastSelectedDisplayName);
                _lastSelectedDisplayName = null; // Reset the last selected display name
            }
            else {
                matchingMapping = typeMappings.FirstOrDefault(mapping => mapping.ValueType == variable.ValueType);
            }

            if (matchingMapping != null) {
                return matchingMapping.FieldCreator();
            } else if (variable.ValueType != null) {
                if(variable.ValueType.IsEnum) {
                    TypeMapping enumTypeMapping = typeMappings.FirstOrDefault(mapping => mapping.ValueType == typeof(Enum));
                    if (enumTypeMapping != null) {
                        return enumTypeMapping.FieldCreator();
                    }
                    else {
                        Debug.LogError("Enum TypeMapping is not found in the typeMappings list.");
                    }
                }
            } 
            Debug.Log(variable.ValueType);
            // Fallback to PropertyField for unknown types
            PropertyField propertyField = new PropertyField(serializedObject.FindProperty("jsonValue"));
            propertyField.Bind(serializedObject);
            return propertyField;
        }

        private T CreateFieldForType<T, V>() where T : VisualElement, INotifyValueChanged<V> {
            T field = (T)Activator.CreateInstance<T>();
            if (_variable.Value != null && _variable.Value.GetType() == typeof(V)) {
                field.GetType().GetProperty("value").SetValue(field, _variable.Value);
            }
            field.RegisterCallback<ChangeEvent<V>>(evt => {
                _variable.Value = evt.newValue;
            });

            return field;
        }

        private System.Type GetCustomEditorType(System.Type type) {
            // Create an instance of the target type
            var instance = ScriptableObject.CreateInstance(type);

            // Use Editor.CreateEditor to find the associated custom editor
            var customEditor = UnityEditor.Editor.CreateEditor(instance);

            // Clean up the created instance
            DestroyImmediate(instance);

            // If a custom editor is found, return its type, otherwise return null
            return customEditor != null ? customEditor.GetType() : null;
        }

        private List<TypeMapping> _cachedTypeMappings;
        private DateTime _lastUpdateTime;

        private List<TypeMapping> GetTypeMappings() {
            var customAttributesData = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(type => type.GetCustomAttributes(typeof(ShowInScriptableVariableAttribute), false)
            .Select(attr => new { Type = type, Attribute = (ShowInScriptableVariableAttribute)attr }))
            .ToList();
            
            List<TypeMapping> basicTypeMappings = new List<TypeMapping>
            {
                new TypeMapping(typeof(UnityEngine.Object), "Unity Objects", () => CreateFieldForType<ObjectField, UnityEngine.Object>()),
                new TypeMapping(typeof(int), "Int",  () => CreateFieldForType<IntegerField, int>()),
                new TypeMapping(typeof(float), "Float", () => CreateFieldForType<FloatField, float>()),
                new TypeMapping(typeof(long), "Long", () => CreateFieldForType<LongField, long>()),
                new TypeMapping(typeof(string), "String", () => CreateFieldForType<TextField, string>()),
                new TypeMapping(typeof(Vector2), "Vector 2", () => CreateFieldForType<Vector2Field, Vector2>()),
                new TypeMapping(typeof(Vector3), "Vector 3", () => CreateFieldForType<Vector3Field, Vector3>()),
                new TypeMapping(typeof(Vector4), "Vector 4", () => CreateFieldForType<Vector4Field, Vector4>()),
                new TypeMapping(typeof(Rect), "Rect", () => CreateFieldForType<RectField, Rect>()),
                new TypeMapping(typeof(Bounds), "Bounds", () => CreateFieldForType<BoundsField, Bounds>()),
                new TypeMapping(typeof(Vector2Int), "Vector 2 Int", () => CreateFieldForType<Vector2IntField, Vector2Int>()),
                new TypeMapping(typeof(Vector3Int), "Vector 3 Int", () => CreateFieldForType<Vector3IntField, Vector3Int>()),
                new TypeMapping(typeof(RectInt), "Rect Int", () => CreateFieldForType<RectIntField, RectInt>()),
                new TypeMapping(typeof(BoundsInt), "Bounds Int", () => CreateFieldForType<BoundsIntField, BoundsInt>()),
                new TypeMapping(typeof(Color), "Color", () => CreateFieldForType<ColorField, Color>()),
                new TypeMapping(typeof(AnimationCurve), "Curve", () => CreateFieldForType<CurveField, AnimationCurve>()),
                new TypeMapping(typeof(Gradient), "Gradient", () => CreateFieldForType<GradientField, Gradient>()),
                new TypeMapping(
                    typeof(Enum),
                    "Enum",
                    () => CreateFieldForType<EnumField, Enum>(),
                    () => {
                        MenuField enumTypeDropdown = CreateEnumTypeDropdown();
                        enumTypeDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
                            Type selectedEnumType = FindType(evt.newValue.Replace("/", "."));

                            _variable.Value = Enum.GetValues(selectedEnumType).GetValue(0);
                            _variable.ValueType = selectedEnumType;
                            UpdateValueContainer();
                        });
                        _valueContainer.Add(enumTypeDropdown);
                    }
                ),
                new TypeMapping(typeof(string), "Tag", () => CreateFieldForType<TagField, string>()),
                new TypeMapping(typeof(int), "Layer", () => CreateFieldForType<LayerField, int>()),
                new TypeMapping(typeof(int), "Mask", () => CreateFieldForType<MaskField, int>()),
                // Add more type mappings here
            };

            List<TypeMapping> customTypeMappings = customAttributesData
            .Select(data => new TypeMapping(data.Type, data.Attribute.DisplayName, data.Attribute.FieldCreator, data.Attribute.OnTypeSelected))
            .ToList();

            if (_cachedTypeMappings != null &&
                _cachedTypeMappings.Count == basicTypeMappings.Count + customTypeMappings.Count) {
                if (customAttributesData.Count > 0) {
                    if (_lastUpdateTime == customAttributesData.Max(data => data.Attribute.Timestamp)) {
                        return _cachedTypeMappings;
                    }
                }
                else {
                    return _cachedTypeMappings;
                }
            }

            _cachedTypeMappings = new List<TypeMapping>();
            _cachedTypeMappings.AddRange(basicTypeMappings);
            _cachedTypeMappings.AddRange(customTypeMappings);

            if (customAttributesData.Count > 0) {
                _lastUpdateTime = customAttributesData.Max(data => data.Attribute.Timestamp);
            }

            return _cachedTypeMappings;
        }
        public class TypeMapping {
            public Type ValueType { get; set; }
            public string DisplayName { get; set; }
            public Func<VisualElement> FieldCreator { get; set; }
            public Action OnTypeSelected { get; set; }

            public TypeMapping(Type valueType, string displayName, Func<VisualElement> fieldCreator, Action onTypeSelected = null) {
                ValueType = valueType;
                DisplayName = displayName;
                FieldCreator = fieldCreator;
                OnTypeSelected = onTypeSelected;
            }
        }

        private MenuField CreateEnumTypeDropdown() {
            List<Type> enumTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsEnum)
            .ToList();

            List<string> enumTypePaths = enumTypes.Select(type =>
            {
                string assemblyName = type.Assembly.GetName().Name;
                string withoutAssembly = type.FullName.Replace(assemblyName + ".", "");
                return type.FullName.Replace(".", "/");
            }).ToList();
            MenuField enumTypeMenu = new MenuField("Select Enum Type", enumTypePaths);
            Debug.Log(enumTypePaths.Count);

            return enumTypeMenu;
        }

        public static Type FindType(string typeName) {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
