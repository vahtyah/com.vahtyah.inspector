# com.vahtyah.inspector — API Reference

> Version 1.1.0. Auto-generated from the compiled public API by `VahTyah → Generate API Docs`. Do not edit by hand — edit the source or `Documentation~/Examples.md` instead.

This package ships as obfuscated DLLs. This file documents the full public surface so it can be used without decompiling.

## Usage Examples

Attribute-driven Inspector enhancements (grouping, buttons, conditional display, auto-wiring
references). The field/method attributes live in the runtime assembly, so they compile into your
build; the drawing happens in a custom Inspector that is gated behind the `VAHTYAH_CUSTOM_INSPECTOR`
scripting define — add it in *Project Settings → Player → Scripting Define Symbols* for the
grouping/ShowIf/ReadOnly/Required attributes to take visual effect.

### Grouping, buttons, required, conditional fields

```csharp
using UnityEngine;
using VahTyah.Inspector;

public class Turret : MonoBehaviour
{
    [BoxGroup("Refs")] [Required] public Transform muzzle;      // warn icon if unassigned
    [BoxGroup("Refs")] [ReadOnly]  public int    instanceId;    // shown, not editable

    [BoxGroup("Fire")] public bool  automatic;
    [BoxGroup("Fire")] [ShowIf(nameof(automatic))] public float fireRate = 10f; // only when automatic
    [BoxGroup("Fire")] public FireMode mode;
    [BoxGroup("Fire")] [ShowIf(nameof(mode), FireMode.Burst)] public int burstCount = 3;

    [OnValueChanged(nameof(OnColorChanged))] public Color color = Color.white;
    private void OnColorChanged() => GetComponent<Renderer>().material.color = color;

    [Button("Fire Once")]                 // renders a button in the Inspector
    private void FireOnce() { /* ... */ }
}

public enum FireMode { Single, Burst }
```

`[ShowIf]` reads a bool member (`[ShowIf("member")]`) or compares any member to a value
(`[ShowIf("member", value)]`, supports enum/int/string/bool). The member may be a field, property,
or no-arg method on the target, including private and inherited ones. `[Required(message, isError)]`
and `[Button(label, order)]` / `[BoxGroup(id, label, order)]` take optional args (see API Reference).

### Auto-wired references (`[AutoRef]`, `[AssetRef]`)

```csharp
using UnityEngine;
using VahTyah.Inspector;

public class Enemy : MonoBehaviour
{
    [AutoRef]                              // GetComponent on Self (default)
    public Rigidbody body;

    [AutoRef(RefSource.Children, includeInactive: true)]
    public Collider[] hitboxes;            // GetComponentsInChildren

    [AutoRef(RefSource.Parent)]  public Canvas rootCanvas;

    [AssetRef(assetName: "DefaultEnemyConfig")]  // finds the asset by name in the project
    public ScriptableObject config;
}
```

`RefSource` is `Self | Children | Parent | Scene`.

### `[SerializeReference]` polymorphism (`[SubclassSelector]`)

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using VahTyah.Inspector;

public interface IAbility { }
[Serializable] public class Dash : IAbility { public float distance = 5f; }
[Serializable] public class Heal : IAbility { public int amount = 20; }

public class Player : MonoBehaviour
{
    [SerializeReference, SubclassSelector]                 // dropdown to pick Dash/Heal
    public IAbility primary;

    [SerializeReference, SubclassSelector(includeNull: false)]
    public List<IAbility> abilities = new();              // per-element dropdown
}
```

`[SubclassSelector]` is a real Unity `PropertyDrawer`, so `EditorGUILayout.PropertyField` honors it
even outside the custom Inspector.

### Persist tweaks made in Play mode (`PlayModeSaveSystem`)

```csharp
using UnityEditor;
using VahTyah.Inspector;

// From an editor tool: keep a component's values changed during Play mode after exiting.
PlayModeSaveSystem.SaveComponent(myComponent);
bool kept = PlayModeSaveSystem.IsSaved(myComponent);
PlayModeSaveSystem.ClearSavedData(myComponent);
```

## API Reference

### namespace `VahTyah.Inspector`

#### class `AssetRefAttribute`

*: Attribute*  
Automatically find and assign asset references from the project  

```csharp
public AssetRefAttribute();
public AssetRefAttribute(string assetName = null, string assetPath = null);
public string AssetName { get; }
public string AssetPath { get; }
```

- `AssetName` — Asset name (without extension) to match, or null to match by field type only.
- `AssetPath` — Folder path to search within (e.g. "Assets/Data"), or null to search the entire project.

#### class `AutoRefAttribute`

*: Attribute*  
Automatically get/find component references  

```csharp
public AutoRefAttribute(RefSource source = RefSource.Self, bool includeInactive = false);
public bool IncludeInactive { get; }
public RefSource Source { get; }
```

- `Source` — Where to search for the component reference.
- `IncludeInactive` — Whether inactive GameObjects are included in the search.

#### class `BoxGroupAttribute`

*: GroupAttribute*  
Groups fields sharing the same id into a boxed section in the Inspector.  

```csharp
public BoxGroupAttribute(string id, string label = "", int order = 0);
```

#### class `ButtonAttribute`

*: Attribute*  
Attribute to display a button in the inspector that calls a method  

```csharp
public ButtonAttribute(string label = "", int order = 0);
public string Label { get; }
public int Order { get; }
```

- `Label` — Button label; falls back to the method name when empty.
- `Order` — Display order among buttons (lower values appear first).

#### class `CustomInspector`

*: Editor*  
Base custom Editor that renders fields with grouping, ShowIf, required/readonly/auto-ref/asset-ref controls, OnValueChanged callbacks, and Button methods.  

```csharp
public CustomInspector();
protected virtual void OnEnable();
public virtual void OnInspectorGUI();
```

#### abstract class `DeferredEditor`

*: Editor*  
Base Editor gom idiom "hoãn thao tác đổi asset/mảng" (_deferred): consumer gọi Defer trong lúc vẽ (từ menu, nút…) để hoãn thao tác ra SAU khi mọi layout scope đóng — tránh ExitGUI/đổi số control giữa các scope — rồi gọi RunDeferred ở cuối OnInspectorGUI.  

```csharp
protected DeferredEditor();
protected bool HasDeferred { get; }
protected void Defer(Action action);
protected void RunDeferred(bool repaintEventOnly);
```

- `HasDeferred` — True khi đang có một thao tác chờ chạy qua RunDeferred.
- `Defer` — Hoãn một thao tác. Ghi đè cái trước (giống idiom _deferred = () => … ở consumer).
- `RunDeferred` — Chạy thao tác đã hoãn (nếu có) rồi Repaint(). repaintEventOnly = true → chỉ chạy ở EventType.Repaint (không đổi số control giữa Layout/Repaint của cùng frame — Juice cần); false → chạy vô điều kiện (Module).

#### static class `DoctorIcon`

Render icon "doctor" cảnh báo trên header band: console.erroricon.sml / console.warnicon.sml canh giữa (idiom RequiredDrawer). Phần COMPUTE (item nào lỗi, semantic gì) giữ ở consumer — helper chỉ lo vẽ icon + tooltip.  

```csharp
public static void Draw(Rect rect, bool isError, string tooltip, GUIStyle style);
```

- `Draw` — Vẽ icon warn/error trong rect kèm tooltip. style do consumer truyền (Juice: label MiddleCenter; Module: GUI.skin.label mặc định) để giữ đúng pixel của mỗi bên.

#### abstract class `GroupAttribute`

*: Attribute*  
Base attribute for grouping Inspector fields under a shared id and label.  

```csharp
protected GroupAttribute(string id, string label = "", int order = 0);
public string ID { get; }
public string Label { get; }
public int Order { get; }
```

- `ID` — Identifier that fields with the same value are grouped under.
- `Label` — Header text shown for the group; defaults to the id when not set.
- `Order` — Display order of the group (lower values appear first).

#### static class `GroupBoxGUI`

Helper vẽ box-group dùng chung cho các custom editor tiêu thụ package (Juice, Module, BoxGroupDrawer): nền box bo góc + header band (LayerDrawingSystem) và foldout ▾/▸. Chỉ giữ phần THỰC SỰ dùng chung ≥2 consumer. Các thứ đặc thù 1 consumer (play/stop glyph, progress bar, icon-button style của Juice; accent overlay của mỗi consumer) KHÔNG nằm ở đây — để consumer tự giữ. Literal khác nhau giữa consumer (pad, accent, rightPad…) được truyền vào chứ không hard-code. Ngoài #if VAHTYAH_CUSTOM_INSPECTOR.  

```csharp
public static bool DrawFoldout(Rect clickRegion, float labelPad, string title, bool open, GUIStyle titleStyle);
public static void DrawGroupBackdrop(Rect box, Rect header, InspectorStyleData.GroupStyles g);
public static bool DrawHeader(Rect box, Rect header, InspectorStyleData.GroupStyles g, float labelPad, string title, bool open, GUIStyle titleStyle);
```

- `DrawGroupBackdrop` — Nền box + header band. DrawLayers tự no-op ngoài Repaint nên gọi vô điều kiện.
- `DrawFoldout` — Nút vô hình phủ clickRegion + arrow ▾/▸ + label. Trả về open mới (caller lo persist, ví dụ SessionState). Dùng cho group có widget bên phải header (clickRegion chỉ phủ nửa trái). labelPad là lề trái của label — do consumer truyền (Juice: headerPadding.left + 4f; Module: headerPadding.left).
- `DrawHeader` — Backdrop + foldout phủ TRỌN header — cho group không có widget bên phải.

#### struct `HeaderBar`

Con trỏ layout cho cụm nút căn phải của một header band — thay cho mọi phép x -= WIDTH; x -= GAP viết tay trong các custom editor. Con trỏ chạy từ header.xMax - rightPad về trái; mỗi Button cấp một rect cao 1 dòng, canh giữa dọc trong header. Không giữ state ngoài con trỏ cục bộ → an toàn, consumer vẫn tự quyết nút nào / glyph gì. rightPad và gapBefore là hằng của consumer (Juice: RightPad 6, GAP 4) — truyền vào, KHÔNG lấy từ GroupStyles (GroupStyles không có các trường này).  

```csharp
public HeaderBar(Rect header, float rightPad);
public readonly float Cy;
public readonly float Line;
public float X { get; }
public Rect Button(float width, float gapBefore = 0);
public Rect Remaining(Rect header, float leftPad, float gapBefore = 0);
```

- `Cy` — Toạ độ y đã căn giữa dọc cho control cao 1 dòng trong header.
- `Line` — Chiều cao 1 dòng (EditorGUIUtility.singleLineHeight).
- `X` — Vị trí con trỏ hiện tại (mép trái cụm nút phải) — để tính bề rộng vùng label/toggle còn lại.
- `Button` — Cấp một nút: chạy con trỏ sang trái (gapBefore + width) rồi trả Rect cao 1 dòng, canh giữa dọc. Nút đầu tiên thường gapBefore = 0; các nút sau gapBefore = GAP của consumer.
- `Remaining` — Vùng còn lại bên trái cho label/title: từ header.x + leftPad tới con trỏ hiện tại (trừ gapBefore). Dùng full chiều cao header (label căn theo header, không theo dòng).

#### static class `InspectorStyle`

Điểm truy cập tĩnh tới InspectorStyleData hiện hành mà các drawer đọc để vẽ. Nguồn style là InspectorTheme (lưu local trong UserSettings/), không còn asset nào.  

```csharp
public static void EnsureStyleDatabaseExists();
public static InspectorStyleData GetStyle();
public static void Refresh();
```

- `EnsureStyleDatabaseExists` — Nạp style hiện hành nếu chưa có.
- `GetStyle` — Trả về InspectorStyleData đang dùng để vẽ inspector.
- `Refresh` — Đọc lại style hiện hành (vd. sau khi đổi theme); drawer sẽ cập nhật ở repaint kế.

#### class `InspectorStyleData`

Toàn bộ style dùng để vẽ inspector tuỳ biến: style cho group và cho button.  

```csharp
public InspectorStyleData();
public InspectorStyleData.ButtonStyles buttonStyles;
public InspectorStyleData.GroupStyles groupStyles;
public static InspectorStyleData CreateDefault(bool isDarkMode);
public static InspectorStyleData CreateForSkin();
```

- `CreateDefault` — Bộ style dựng sẵn cho theme tối hoặc sáng.
- `CreateForSkin` — Bộ style dựng sẵn khớp skin Editor hiện tại (pro-skin = dark).

#### class `InspectorStyleData.ButtonStyles`

Kích thước, padding, layer config cho các trạng thái (normal/hover/active) của button.  

```csharp
public ButtonStyles();
public LayerConfiguration activeConfig;
public LayerConfiguration backgroundConfig;
public float buttonHeight;
public Padding buttonPadding;
public float buttonSpacing;
public LayerConfiguration hoverConfig;
public GUIStyle labelStyle;
public LayerConfiguration normalConfig;
public static InspectorStyleData.ButtonStyles CreateDefaultStyles(bool isDarkMode);
```

- `CreateDefaultStyles` — Tạo bộ button style mặc định cho theme tối hoặc sáng.

#### class `InspectorStyleData.GroupStyles`

Kích thước, padding và layer config để vẽ group box và header của nó.  

```csharp
public GroupStyles();
public LayerConfiguration backgroundConfig;
public Padding contentPadding;
public float groupSpacing;
public LayerConfiguration headerConfig;
public float headerHeight;
public Padding headerPadding;
public GUIStyle labelStyle;
public static InspectorStyleData.GroupStyles CreateDefaultStyles(bool isDarkMode);
```

- `CreateDefaultStyles` — Tạo bộ group style mặc định cho theme tối hoặc sáng.

#### class `MonoBehaviorInspector`

*: CustomInspector*  
Applies CustomInspector to every MonoBehaviour so its attributes render.  

```csharp
public MonoBehaviorInspector();
```

#### class `OnValueChangedAttribute`

*: Attribute*  
Invokes a method when the field value changes in the Inspector  

```csharp
public OnValueChangedAttribute(string methodName);
public string MethodName { get; }
```

- `MethodName` — Name of the method invoked when the field value changes.

#### class `PlayModeSaveSystem`

*: ScriptableSingleton<PlayModeSaveSystem>*  
Captures serialized values of selected components during Play mode and re-applies them when returning to Edit mode, so tweaks made while playing are not lost.  

```csharp
public PlayModeSaveSystem();
public static void ClearAllSavedData();
public static void ClearSavedData(Component component);
public static bool IsSaved(Component component);
public static void SaveComponent(Component component);
```

- `SaveComponent` — Toggles tracking for the component: registers it for save if not tracked, otherwise stops tracking it.
- `IsSaved` — Returns whether the component is currently tracked for Play mode save.
- `ClearSavedData` — Removes any saved data tracked for the given component.
- `ClearAllSavedData` — Removes all tracked save data for every component.

#### class `PlayModeSaveSystem.ComponentData`

Serialized snapshot of one component's property values and reference identity used to restore it.  

```csharp
public ComponentData();
public int componentInstanceId;
public string componentTypeName;
public string globalId;
public string globalIdUnpacked;
public List<string> objectRefAssetPaths;
public List<string> objectRefAssetTypes;
public List<string> objectRefPaths;
public List<string> propertyPaths;
public List<int> propertyTypes;
public List<string> stringValues;
```

#### class `ReadOnlyAttribute`

*: Attribute*  
Displays a field in the Inspector as non-editable (read-only).  

```csharp
public ReadOnlyAttribute();
```

#### enum `RefSource`

Search scope used by AutoRefAttribute to locate a component reference.  

```csharp
enum RefSource : int
{
    Self = 0,
    Children = 1,
    Parent = 2,
    Scene = 3,
}
```

#### class `RequiredAttribute`

*: Attribute*  
Flags a reference field as required, showing a help box when it is left unassigned.  

```csharp
public RequiredAttribute(string message = null, bool isError = false);
public bool IsError { get; }
public string Message { get; }
```

- `Message` — Custom message shown when the field is unassigned; a default is used when null.
- `IsError` — Whether the notice is shown as an error rather than a warning.

#### class `ScriptableObjectInspector`

*: CustomInspector*  
Applies CustomInspector to every ScriptableObject so its attributes render.  

```csharp
public ScriptableObjectInspector();
```

#### sealed class `ShowIfAttribute`

*: Attribute*  
Chỉ hiện field khi điều kiện đúng. - [ShowIf("boolMember")]        : hiện khi field/property/method (no-arg) bool == true. - [ShowIf("member", value)]     : hiện khi member == value (hỗ trợ enum, int, string, bool...). Member nằm trên chính target (kể cả private, kể cả base class). Chỉ có tác dụng khi VAHTYAH_CUSTOM_INSPECTOR bật (giống BoxGroup/ReadOnly/Required).  

```csharp
public ShowIfAttribute(string condition);
public ShowIfAttribute(string condition, object value);
public string Condition { get; }
public bool HasValue { get; }
public object Value { get; }
```

- `Condition` — Tên field/property/method (no-arg) trên target dùng làm điều kiện.
- `Value` — Giá trị cần so khớp với member; chỉ dùng khi HasValue == true.
- `HasValue` — True khi attribute so khớp theo Value, false khi chỉ kiểm tra bool member.

#### sealed class `SubclassSelectorAttribute`

*: PropertyAttribute*  
Đặt lên field [SerializeReference] (hoặc List có phần tử [SerializeReference]) để hiện dropdown chọn concrete class implement interface/base đó ngay trong Inspector.  

```csharp
public SubclassSelectorAttribute(bool includeNull = true);
public readonly bool IncludeNull;
```

- `IncludeNull` — Có thêm lựa chọn null vào dropdown hay không.

#### sealed class `SubclassSelectorDrawer`

*: PropertyDrawer*  
Vẽ dropdown chọn subclass cho field [SerializeReference] + [SubclassSelector]. Là Unity PropertyDrawer nên EditorGUILayout.PropertyField (kể cả trong CustomInspector) tự honor; áp cho từng phần tử của List.  

```csharp
public SubclassSelectorDrawer();
public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label);
public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label);
```

