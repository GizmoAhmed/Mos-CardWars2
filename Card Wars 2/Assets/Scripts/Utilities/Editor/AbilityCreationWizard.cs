#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using AbilityEvents;

namespace Utilities.Editor
{
    public class AbilityCreationWizard : EditorWindow
    {
        // ===========================
        // Wizard Fields
        // ===========================

        private string _abilityName = "NewAbility";
        private AbilityType _abilityType = AbilityType.Passive;
        private CardType _cardType = CardType.Creature;

        private Vector2 _scrollPos;

        private string _previewClassName = "";
        private string _previewPath = "";
        private string _previewAssetName = "";

        // EditorPrefs keys for post-compile SO creation
        private const string PENDING_SO_CLASS = "AbilityWizard_PendingSOClass";
        private const string PENDING_SO_PATH = "AbilityWizard_PendingSOPath";

        // ===========================
        // Enums for Wizard
        // ===========================

        private enum AbilityType
        {
            Passive,
            Active,
            Cast
        }

        private enum CardType
        {
            Creature,
            Building,
            Spell,
            Charm,
            Rune
        }

        // ===========================
        // Open Window
        // ===========================

        [MenuItem("Tools/Ability Creator")]
        public static void OpenWizard()
        {
            AbilityCreationWizard window = GetWindow<AbilityCreationWizard>("Ability Creator");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        // ===========================
        // Post-Compile SO Creation
        // ===========================

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // Nothing pending, do nothing
            if (!EditorPrefs.HasKey(PENDING_SO_CLASS)) return;

            string className = EditorPrefs.GetString(PENDING_SO_CLASS);
            string assetPath = EditorPrefs.GetString(PENDING_SO_PATH);

            // Clear keys so this doesn't run again
            EditorPrefs.DeleteKey(PENDING_SO_CLASS);
            EditorPrefs.DeleteKey(PENDING_SO_PATH);

            // Delay one frame to let Unity finish loading
            EditorApplication.delayCall += () => CreateSOAsset(className, assetPath);
        }

        private static void CreateSOAsset(string className, string assetPath)
        {
            if (File.Exists(assetPath))
            {
                Debug.LogWarning($"Asset already exists: {assetPath}");
                return;
            }

            // Search all assemblies for the type
            System.Type abilityType = null;

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                abilityType = assembly.GetType(className);
                if (abilityType != null) break;
            }

            if (abilityType == null)
            {
                Debug.LogError($"Could not find type: {className}. " +
                               $"Check the script compiled without errors!");
                return;
            }

            // Create and save the SO
            ScriptableObject so = ScriptableObject.CreateInstance(abilityType);
            AssetDatabase.CreateAsset(so, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select and highlight the new asset
            Selection.activeObject = so;
            EditorGUIUtility.PingObject(so);

            Debug.Log($"<color=green>✓ Created SO: {assetPath}</color>");
        }

        // ===========================
        // Draw UI
        // ===========================

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            DrawAbilitySettings();
            DrawPreview();
            DrawCreateButton();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("⚔️ Ability Creator", headerStyle);
            EditorGUILayout.Space(10);
            DrawDivider();
        }

        private void DrawAbilitySettings()
        {
            EditorGUILayout.LabelField("Ability Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Card type
            _cardType = (CardType)EditorGUILayout.EnumPopup("Card Type", _cardType);

            // Derive ability type from card type automatically
            _abilityType = GetAbilityTypeFromCardType(_cardType);

            // Show derived ability type as read-only (informational)
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("Ability Type (Auto)", _abilityType);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);

            // Ability name
            EditorGUILayout.LabelField("Ability Name (without type suffix):");
            _abilityName = EditorGUILayout.TextField(_abilityName);

            EditorGUILayout.Space(5);

            if (_abilityType == AbilityType.Passive)
            {
                DrawDivider();
                EditorGUILayout.LabelField("Passive Settings", EditorStyles.boldLabel);
            }

            DrawDivider();

            UpdatePreview();
        }

        private AbilityType GetAbilityTypeFromCardType(CardType cardType)
        {
            return cardType switch
            {
                CardType.Creature => AbilityType.Active,
                CardType.Spell => AbilityType.Cast,
                CardType.Building => AbilityType.Passive,
                CardType.Charm => AbilityType.Passive,
                CardType.Rune => AbilityType.Passive,
                _ => AbilityType.Passive
            };
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Class Name", _previewClassName);
            EditorGUILayout.TextField("Folder Path", _previewPath);
            EditorGUILayout.TextField("Script Path", $"{_previewPath}/{_previewClassName}.cs");
            EditorGUILayout.TextField("Asset Path", $"{_previewPath}/{_previewAssetName}.asset");
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);
        }

        private void DrawCreateButton()
        {
            DrawDivider();
            EditorGUILayout.Space(5);

            bool canCreate = !string.IsNullOrEmpty(_abilityName) &&
                             _abilityName != "NewAbility" &&
                             !File.Exists($"{_previewPath}/{_previewClassName}.cs");

            // Warn if script already exists
            if (File.Exists($"{_previewPath}/{_previewClassName}.cs"))
            {
                EditorGUILayout.HelpBox(
                    $"{_previewClassName} already exists!",
                    MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(!canCreate);

            if (GUILayout.Button("✨ Create Ability", GUILayout.Height(40)))
            {
                CreateAbility();
            }

            EditorGUI.EndDisabledGroup();

            if (!canCreate && !File.Exists($"{_previewPath}/{_previewClassName}.cs"))
            {
                EditorGUILayout.HelpBox(
                    "Enter a unique ability name to continue",
                    MessageType.Info);
            }

            EditorGUILayout.Space(5);
        }

        // ===========================
        // Preview Generation
        // ===========================

        private void UpdatePreview()
        {
            _previewClassName = $"{_abilityName}_{_cardType}";

            // ✅ SO asset gets "SO" suffix
            _previewAssetName = $"{_previewClassName}SO";

            _previewPath = $"Assets/Scripts/CardScripts/Abilities/" +
                           $"{_cardType}Abilities/" +
                           $"{_previewClassName}";
        }

        // ===========================
        // Creation Logic
        // ===========================

        private void CreateAbility()
        {
            // 1. Create folder
            CreateFolder();

            // 2. Create script
            CreateScript();

            // 3. Store pending SO info BEFORE refreshing
            // (refresh triggers recompile which triggers OnScriptsReloaded)
            EditorPrefs.SetString(PENDING_SO_CLASS, _previewClassName);
            EditorPrefs.SetString(PENDING_SO_PATH, $"{_previewPath}/{_previewAssetName}.asset");
            
            Debug.Log($"<color=yellow>Recompiling... " +
                      $"SO will be created after compile.</color>");

            // 4. Trigger recompile
            AssetDatabase.Refresh();

            // 5. Ping the folder
            EditorUtility.FocusProjectWindow();
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(_previewPath);
            EditorGUIUtility.PingObject(folder);
        }

        private void CreateFolder()
        {
            if (!Directory.Exists(_previewPath))
            {
                Directory.CreateDirectory(_previewPath);
                Debug.Log($"<color=cyan>Created folder: {_previewPath}</color>");
            }
        }

        private void CreateScript()
        {
            string scriptPath = $"{_previewPath}/{_previewClassName}.cs";

            if (File.Exists(scriptPath))
            {
                Debug.LogWarning($"Script already exists: {scriptPath}");
                return;
            }

            string scriptContent = GenerateScriptContent();
            File.WriteAllText(scriptPath, scriptContent);

            Debug.Log($"<color=cyan>Created script: {scriptPath}</color>");
        }

        // ===========================
        // Script Template Generation
        // ===========================

        private string GenerateScriptContent()
        {
            string baseClass = GetBaseClass();
            string menuPath = GetMenuPath();
            string executeBody = GetExecuteBody();

            return $@"using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(
    fileName = ""{_previewClassName}"", 
    menuName = ""{menuPath}"")]
public class {_previewClassName} : {baseClass}
{{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {{
        {executeBody}
    }}
}}";
        }

        private string GetBaseClass()
        {
            return _abilityType switch
            {
                AbilityType.Passive => "PassiveAbilitySO",
                AbilityType.Active => "ActiveAbilitySO",
                AbilityType.Cast => "CastAbilitySO",
                _ => "PassiveAbilitySO"
            };
        }

        private string GetMenuPath()
        {
            return $"Abilities/{_cardType}/{_previewClassName}";
        }

        private string GetExecuteBody()
        {
            return $@"Debug.Log($""Executing on {{this}} on {{thisCard.name}}"");";
        }

        // ===========================
        // Helpers
        // ===========================

        private void DrawDivider()
        {
            EditorGUILayout.Space(3);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space(3);
        }
    }
}
#endif