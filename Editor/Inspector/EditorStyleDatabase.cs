using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VahTyah.Core;

namespace VahTyah.Inspector
{
    // [CreateAssetMenu(fileName = "EditorStyles", menuName = "CustomEditor/EditorStyles", order = 1)]
    public class EditorStyleDatabase : ScriptableObject
    {
        [SerializeField, BoxGroup("Settings")] private int defaultStyleIndex = 0;
        [SerializeField, BoxGroup("Styles")] private List<InspectorStyleData> styles = new List<InspectorStyleData>();

        [Button]
        public void AddDefaultStyle()
        {
            styles.Add(GetDefaultDarkStyle());
            styles.Add(GetDefaultLightStyle());
        }

        public InspectorStyleData GetStyle()
        {
            if (styles.Count < 2)
            {
                styles.Clear();
                AddDefaultStyle();
            }

            var isDarkMode = EditorGUIUtility.isProSkin;
            return isDarkMode ? styles[0] : styles[1];
        }

        public static InspectorStyleData GetDefaultStyle()
        {
            return UnityEditor.EditorGUIUtility.isProSkin ? GetDefaultDarkStyle() : GetDefaultLightStyle();
        }

        public static InspectorStyleData GetDefaultDarkStyle()
        {
            InspectorStyleData defaultStyleData = new InspectorStyleData
            {
                groupStyles = InspectorStyleData.GroupStyles.CreateDefaultStyles(true),
                buttonStyles = InspectorStyleData.ButtonStyles.CreateDefaultStyles(true)
            };
            return defaultStyleData;
        }

        public static InspectorStyleData GetDefaultLightStyle()
        {
            InspectorStyleData defaultStyleData = new InspectorStyleData
            {
                groupStyles = InspectorStyleData.GroupStyles.CreateDefaultStyles(false),
                buttonStyles = InspectorStyleData.ButtonStyles.CreateDefaultStyles(false)
            };
            return defaultStyleData;
        }
    }

    [Serializable]
    public class InspectorStyleData
    {
        public GroupStyles groupStyles;
        public ButtonStyles buttonStyles;

        [Serializable]
        public class GroupStyles
        {
            public float headerHeight;
            public Padding headerPadding;
            public Padding contentPadding;
            public float groupSpacing;
            public LayerConfiguration backgroundConfig;
            public LayerConfiguration headerConfig;
            public GUIStyle labelStyle;

            public static GroupStyles CreateDefaultStyles(bool isDarkMode)
            {
                GroupStyles styles = new GroupStyles
                {
                    headerHeight = 28f,
                    headerPadding = new Padding(8f, 0f, 0f, 0f),
                    contentPadding = new Padding(8f, 8f, 8f, 8f),
                    groupSpacing = 6f,
                    headerConfig = isDarkMode ? CreateDarkHeaderStyle() : CreateLightHeaderStyle(),
                    backgroundConfig = isDarkMode ? CreateDarkBackgroundStyle() : CreateLightBackgroundStyle(),
                    labelStyle = CreateLabelStyle(isDarkMode)
                };
                return styles;
            }

            private static LayerConfiguration CreateDarkHeaderStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] = Layer.CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.35f, 0.35f, 0.35f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateLightHeaderStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] = Layer.CreateRoundedRect(new Color(1f, 1f, 1f, 0.3f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(1f, 1f, 1f, 0.3f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.6f, 0.6f, 0.6f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateDarkBackgroundStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.15f, 0.15f, 0.15f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.22f, 0.22f, 0.22f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.35f, 0.35f, 0.35f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateLightBackgroundStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.85f, 0.85f, 0.85f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.78f, 0.78f, 0.78f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.6f, 0.6f, 0.6f, 1f), 1f, 4f);
                return config;
            }

            private static GUIStyle CreateLabelStyle(bool isDarkMode)
            {
                GUIStyle labelStyle = new GUIStyle(); // Create from scratch
                labelStyle.fontSize = 12;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                labelStyle.normal.textColor = isDarkMode 
                    ? new Color(0.85f, 0.85f, 0.85f, 1f)
                    : new Color(0.15f, 0.15f, 0.15f, 1f);
                labelStyle.padding = new RectOffset(0, 0, 0, 0);
                return labelStyle;
            }
        }

        [Serializable]
        public class ButtonStyles
        {
            public float buttonHeight;
            public float buttonSpacing;
            public Padding buttonPadding;
            public LayerConfiguration backgroundConfig;
            public LayerConfiguration normalConfig;
            public LayerConfiguration hoverConfig;
            public LayerConfiguration activeConfig;
            public GUIStyle labelStyle;

            public static ButtonStyles CreateDefaultStyles(bool isDarkMode)
            {
                ButtonStyles styles = new ButtonStyles
                {
                    buttonHeight = 30f,
                    buttonSpacing = 5f,
                    buttonPadding = new Padding(12f, 8f, 12f, 8f),
                    backgroundConfig = isDarkMode ? CreateDarkBackgroundStyle() : CreateLightBackgroundStyle(),
                    normalConfig = isDarkMode ? CreateButtonNormalDarkStyle() : CreateButtonNormalLightStyle(),
                    hoverConfig = isDarkMode ? CreateButtonHoverDarkStyle() : CreateButtonHoverLightStyle(),
                    activeConfig = isDarkMode ? CreateButtonActiveDarkStyle() : CreateButtonActiveLightStyle(),
                    labelStyle = CreateButtonLabelStyle(isDarkMode)
                };
                return styles;
            }

            private static LayerConfiguration CreateDarkBackgroundStyle()
            {
                LayerConfiguration config = new LayerConfiguration(2);
                config.layers[0] = Layer.CreateRoundedRect(new Color(0.22f, 0.22f, 0.22f, 1f), 4f);
                config.layers[1] = Layer.CreateBorder(new Color(0.4f, 0.4f, 0.4f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateLightBackgroundStyle()
            {
                LayerConfiguration config = new LayerConfiguration(2);
                config.layers[0] = Layer.CreateRoundedRect(new Color(0.78f, 0.78f, 0.78f, 1f), 4f);
                config.layers[1] = Layer.CreateBorder(new Color(0.6f, 0.6f, 0.6f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonNormalDarkStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0f, 0f, 0f, 0.15f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.4f, 0.4f, 0.4f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonNormalLightStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(1f, 1f, 1f, 0.3f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(1f, 1f, 1f, 0.3f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.6f, 0.6f, 0.6f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonHoverDarkStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.2f, 0.2f, 0.2f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.3f, 0.3f, 0.3f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.5f, 0.5f, 0.5f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonHoverLightStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.8f, 0.8f, 0.8f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.7f, 0.7f, 0.7f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.5f, 0.5f, 0.5f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonActiveDarkStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.1f, 0.1f, 0.1f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.2f, 0.35f, 0.5f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.3f, 0.5f, 0.7f, 1f), 1f, 4f);
                return config;
            }

            private static LayerConfiguration CreateButtonActiveLightStyle()
            {
                LayerConfiguration config = new LayerConfiguration(3);
                config.layers[0] =
                    Layer.CreateRoundedRect(new Color(0.9f, 0.9f, 0.9f, 1f), 4f, new Padding(0, 1, 1, 0));
                config.layers[1] = Layer.CreateRoundedRect(new Color(0.4f, 0.6f, 0.8f, 1f), 4f);
                config.layers[2] = Layer.CreateBorder(new Color(0.3f, 0.5f, 0.7f, 1f), 1f, 4f);
                return config;
            }

            private static GUIStyle CreateButtonLabelStyle(bool isDarkMode)
            {
                GUIStyle labelStyle = new GUIStyle(); // Create from scratch
                labelStyle.fontSize = 12;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = isDarkMode
                    ? new Color(0.9f, 0.9f, 0.9f, 1f)
                    : new Color(0.1f, 0.1f, 0.1f, 1f);
                labelStyle.padding = new RectOffset(0, 0, 0, 0);
                labelStyle.margin = new RectOffset(0, 0, 0, 0);
                labelStyle.contentOffset = Vector2.zero;
                labelStyle.clipping = TextClipping.Overflow;
                return labelStyle;
            }
        }
    }
}