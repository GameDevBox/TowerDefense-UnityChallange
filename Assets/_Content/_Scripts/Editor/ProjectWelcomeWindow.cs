using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
public class ProjectWelcomeWindow : EditorWindow
{
    private static bool hasShown = false;
    private Vector2 scrollPosition;
    private Texture2D logoTexture;

    private const string SHOWN_KEY = "ProjectWelcome_Shown";

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        EditorApplication.delayCall += ShowWindowOnStartup;
    }

    private static void ShowWindowOnStartup()
    {
        if (!SessionState.GetBool(SHOWN_KEY, false))
        {
            ShowWindow();
            SessionState.SetBool(SHOWN_KEY, true);
        }
    }

    [MenuItem("Tools/Project Info & Welcome")]
    public static void ShowWindow()
    {
        ProjectWelcomeWindow window = GetWindow<ProjectWelcomeWindow>("Project Welcome", true);
        window.minSize = new Vector2(600, 700);
        window.maxSize = new Vector2(600, 900);
        window.Show();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Header Section
        DrawHeader();

        // Project Overview
        DrawProjectOverview();

        // Features Section
        DrawFeatures();

        // Architecture Section
        DrawArchitecture();

        // Implementation Details
        DrawImplementationDetails();

        // Bonus Features
        DrawBonusFeatures();

        // Quick Start Guide
        DrawQuickStart();

        EditorGUILayout.EndScrollView();

        // Footer
        DrawFooter();
    }

    private void DrawHeader()
    {
        // Welcome Message
        EditorGUILayout.LabelField("🎮 Tower Defense Challenge - Concept Implementation - KingCode", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Hi, I'm Arian - Concept is Ready!", EditorStyles.largeLabel);
        GUILayout.Space(10);
    }

    private void DrawProjectOverview()
    {
        EditorGUILayout.LabelField("📋 Project Overview", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This is a Concept for Tower Defense game featuring enemy wave systems, " +
            "tower placement and shooting mechanics, modular architecture, and UI systems. " +
            "Built with scalability and maintainability in mind.",
            MessageType.Info
        );
        GUILayout.Space(10);
    }

    private void DrawFeatures()
    {
        EditorGUILayout.LabelField("⭐ Core Features Implemented", EditorStyles.boldLabel);

        // Enemy Wave System
        DrawFeatureItem("🎯 Enemy Wave System", new string[] {
            "• Configurable wave patterns with ScriptableObjects",
            "• Multiple enemy sequences per wave",
            "• Difficulty scaling with looping patterns",
            "• Boss wave support with special rewards (The Code is ready but no boss in game currently)",
            "• Real-time wave progression tracking"
        });

        // Tower System
        DrawFeatureItem("🏰 Tower Defense System", new string[] {
            "• Modular tower placement with grid validation (Simple)",
            "• Multiple tower types",
            "• Smart targeting (nearest enemy priority)",
            "• Projectile with visual effects",
        });

        // Enemy AI
        DrawFeatureItem("👾 Enemy AI & Movement", new string[] {
            "• Waypoint-based navigation system",
            "• Flying vs ground enemy types",
            "• Health system with damage feedback",
            "• Death animations with delayed destruction",
        });

        // UI System
        DrawFeatureItem("📱 User Interface", new string[] {
            "• Real-time wave counter and progress",
            "• Resource management (money/score)",
            "• Health/base damage system",
            "• Game speed controls (1x, 2x, 4x, 8x)",
            "• Win/lose conditions with visual feedback"
        });

        GUILayout.Space(10);
    }

    private void DrawFeatureItem(string title, string[] items)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        foreach (string item in items)
        {
            EditorGUILayout.LabelField(item, EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawArchitecture()
    {
        EditorGUILayout.LabelField("🏗️ System Architecture", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Built with modular, scalable architecture using:\n\n" +
            "• ScriptableObjects for data-driven design (WaveData, TowerData, EnemyData)\n" +
            "• Event-driven communication system (GameEvents static class)\n" +
            "• Singleton pattern for managers (GameData, WaveManager)\n" +
            "• Component-based entity system (Enemy, Tower, Projectile)\n",
            MessageType.Info
        );

        GUILayout.Space(10);
    }

    private void DrawImplementationDetails()
    {
        EditorGUILayout.LabelField("🔧 Technical Implementation", EditorStyles.boldLabel);

        DrawTechItem("Enemy Wave System", new string[] {
            "WavePattern & WaveData ScriptableObjects",
            "Sequential enemy spawning with delays",
            "Automatic wave progression with configurable timing",
            "Difficulty scaling per wave loop"
        });

        DrawTechItem("Tower Mechanics", new string[] {
            "TowerPlacementController with grid validation",
            "TowerSpot component for placement management",
            "Projectile system with target tracking",
            "Range detection using SphereCollider triggers"
        });

        DrawTechItem("Game Management", new string[] {
            "GameData singleton for player state",
            "UIManager for real-time UI updates",
            "CameraShake for impact feedback",
            "Event system for loose coupling"
        });

        DrawTechItem("Visual & Audio", new string[] {
            "Particle systems for muzzle flash & impacts",
            "Audio integration",
            "Animation controllers for enemy states",
            "Material swapping for placement validation"
        });

        GUILayout.Space(10);
    }

    private void DrawTechItem(string title, string[] items)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(title, EditorStyles.miniBoldLabel);
        foreach (string item in items)
        {
            EditorGUILayout.LabelField("  " + item, EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(3);
    }

    private void DrawBonusFeatures()
    {
        EditorGUILayout.LabelField("🎁 Bonus Features (Beyond Requirements)", EditorStyles.boldLabel);

        string[] bonusFeatures = {
            "✅ Advanced wave configuration with multiple enemy types per wave",
            "✅ Game speed controls for better gameplay experience",
            "✅ Camera shake effects for impactful moments",
            "✅ Screenshot utility for asset management",
            "✅ Event-driven architecture for extensibility",
            "✅ Editor code documentation"
        };

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        foreach (string feature in bonusFeatures)
        {
            EditorGUILayout.LabelField(feature, EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
    }

    private void DrawQuickStart()
    {
        EditorGUILayout.LabelField("🚀 Quick Start Guide", EditorStyles.boldLabel);

        string[] steps = {
            "1. Open the main scene in Assets/_Content/_Scenes/",
            "2. Press Play to start the game immediately",
            "3. Use the UI to select towers and place them",
            "4. Watch waves spawn automatically",
            "5. Manage resources and defend your base!",
            "",
            "Hotkeys:",
            "• WASD - Move Camera",
            "• MiddleMouse - Zoom Camera",
            "• RightClick - Rotate Camera",
            "• Space - Cycle game speed",
            "• Right Click - Cancel tower placement"
        };

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        foreach (string step in steps)
        {
            EditorGUILayout.LabelField(step, EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
    }

    private void DrawFooter()
    {
        GUILayout.FlexibleSpace();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Developed by Arian", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Tower Defense Challenge - Concept Implementation", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Main Scene", GUILayout.Height(30)))
        {
            OpenMainScene();
        }
        if (GUILayout.Button("Close Welcome", GUILayout.Height(30)))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    private void OpenMainScene()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            if (scenePath.ToLower().Contains("game"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
                return;
            }
        }

        // If no main scene found, show dialog
        EditorUtility.DisplayDialog("Scene Not Found",
            "Could not find a main scene. Please look for scene files in your project.", "OK");
    }
}
#endif