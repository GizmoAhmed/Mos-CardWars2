using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Editor
{
    public class SpriteEditor : EditorWindow
    {
        // saves copy of original sprite that replaces it to show edits
        protected Texture2D editableTexture;

        // toggle grid
        protected bool GridOn = true;

        // toggle pen or eraser
        protected bool isErasing;

        // default color
        protected Color pickedColor = Color.black;

        // sprite put in the field
        protected Texture2D spriteToEdit;

        /// <summary>
        ///     OnGUI is called for every draw of the window, so whenever its clicked on and/or moved
        ///     GUILayout.Space() separates elements via pixels
        ///     GUILayout functions like Begin and End are how you seperate spac functions to move things around
        /// </summary>
        protected void OnGUI()
        {
            GUILayout.Label("Edit a Sprite", EditorStyles.boldLabel);

            // very convenient function here
            // Colorfield() puts a color picker in the editor for you
            // i thought i would have to code that
            pickedColor = EditorGUILayout.ColorField("Selected Color: ", pickedColor);

            // new sprite is the sprite you put in the thing
            var newSprite =
                EditorGUILayout.ObjectField("Sprite To Edit: ", spriteToEdit, typeof(Texture2D), false) as Texture2D;

            if (newSprite != spriteToEdit)
            {
                spriteToEdit = newSprite;
                InitEditableTexture(); // texture that you can draw over
                Repaint();
            }

            // start buttons here 
            GUILayout.Space(-25); // 25 pixels up from the next element below

            // start a horizontal layout group for buttons
            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            // Add some spacing between this button and the next (within the layout group)
            GUILayout.Space(10);

            // toggle isErasing bool when button clicked 
            if (GUILayout.Button(isErasing ? "Pen (Off)" : "Pen (On)", GUILayout.Width(100), GUILayout.Height(25)))
                isErasing = false;

            GUILayout.Space(10);

            // toggle isErasing bool when button clicked 
            if (GUILayout.Button(isErasing ? "Eraser (On)" : "Eraser (Off)", GUILayout.Width(100), GUILayout.Height(25)))
                isErasing = true;

            GUILayout.Space(10);

            if (GUILayout.Button("Clear Selected", GUILayout.Width(100), GUILayout.Height(25))) ClearSelected();

            GUILayout.Space(10);

            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(25)))
                Save();

            GUILayout.Space(10);

            if (GUILayout.Button(GridOn ? "Grid (On)" : "Grid (Off)", GUILayout.Width(100), GUILayout.Height(25)))
                GridOn = !GridOn;

            // End of horizontal layout group
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // the grey box representing the sprite area
            var drawArea = new Rect(10, 130, position.width - 20, position.height - 130);

            // Background box
            GUI.Box(drawArea, GUIContent.none);

            if (spriteToEdit != null)
            {
                HandleMouseInput(drawArea);
                DrawSprite(drawArea);

                if (GridOn) DrawGrid(drawArea);
            }
            else
            {
                EditorGUI.LabelField(drawArea, "No Sprite Selected",
                    new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } });
            }
        }

        // makes the clickable tab
        [MenuItem("Tools/Sprite Editor")]
        public static void ShowWindow()
        {
            GetWindow<SpriteEditor>("Sprite Editor");
        }

        // creates the copied texture
        protected virtual void InitEditableTexture()
        {
            if (spriteToEdit == null) return;

            // Check if texture is readable
            if (!spriteToEdit.isReadable)
            {
                Debug.LogError(
                    $"Texture '{spriteToEdit.name}' is not readable. Enable 'Read/Write' in the import settings.");
                return;
            }

            // Create a copy of the original texture
            editableTexture = new Texture2D(spriteToEdit.width, spriteToEdit.height, TextureFormat.RGBA32, false);
            editableTexture.SetPixels(spriteToEdit.GetPixels());

            // copying a texture apparently doesn't maintain all the import settings
            editableTexture.filterMode = FilterMode.Point;
            editableTexture.wrapMode = TextureWrapMode.Clamp;

            editableTexture.Apply();
        }

        protected void DrawSprite(Rect area)
        {
            // measure editable sprite dimensions
            var spriteWidth = editableTexture.width * 10;
            var spriteHeight = editableTexture.height * 10;
            var spriteRect = new Rect(area.x, area.y, spriteWidth, spriteHeight);

            // draw editable texture given spriteRect
            GUI.DrawTexture(spriteRect, editableTexture);
        }

        // puts grid over sprite
        protected void DrawGrid(Rect area)
        {
            if (editableTexture == null) return;

            // i think handle is a gui drawer
            // handle color is grey
            Handles.color = new Color(1f, 1f, 1f, 0.2f);

            // draw horizontal lines
            for (var x = 0; x <= editableTexture.width; x++)
            {
                var xPos = area.x + x * 10;
                Handles.DrawLine(new Vector3(xPos, area.y, 0),
                    new Vector3(xPos, area.y + editableTexture.height * 10, 0));
            }

            // draw vertical lines
            for (var y = 0; y <= editableTexture.height; y++)
            {
                var yPos = area.y + y * 10;
                Handles.DrawLine(new Vector3(area.x, yPos, 0), new Vector3(area.x + editableTexture.width * 10, yPos, 0));
            }
        }

        protected virtual void HandleMouseInput(Rect area)
        {
            // Get the current event
            var evt = Event.current;

            // if mouse clicked or held (drag)...
            if ((evt.type == EventType.MouseDown || evt.type == EventType.MouseDrag) && evt.button == 0)
            {
                // ...get its position...
                var localMousePos = evt.mousePosition - new Vector2(area.x, area.y);

                // flooring it snaps to the grid
                var pixelX = Mathf.FloorToInt(localMousePos.x / 10);
                var pixelY = Mathf.FloorToInt(localMousePos.y / 10);

                // ... then draw/erase at that spot
                SetPixelColor(pixelX, pixelY);

                evt.Use(); // "Prevents further event propagation" whatever that means
            }
        }

        protected virtual void SetPixelColor(int x, int y) // x and y are the locations clicked
        {
            if (editableTexture == null)
            {
                Debug.LogWarning("can't set pixel color, editable texture is null");
                return;
            }

            // Ensure coordinates are within grid
            if (x >= 0 && x < editableTexture.width && y >= 0 && y < editableTexture.height)
            {
                var newColor = isErasing ? Color.clear : pickedColor;

                editableTexture.SetPixel(x, editableTexture.height - y - 1, newColor);

                editableTexture.Apply();
                Repaint();
            }
        }

        // ChatGPT wrote this whole thing for me
        protected virtual void Save()
        {
            if (editableTexture == null) return;

            // Open a save panel where the user can choose a location
            var path = EditorUtility.SaveFilePanel(
                "Save Sprite", // Window title
                "Assets/", // Default folder
                "New Sprite", // Default file name
                "png"); // File extension

            if (string.IsNullOrEmpty(path)) return; // If the user cancels, do nothing

            // Convert the texture to PNG
            var pngData = editableTexture.EncodeToPNG();
            if (pngData == null)
            {
                Debug.LogError("Failed to encode texture to PNG.");
                return;
            }

            // Convert absolute path to relative path (if inside Assets folder)
            var relativePath = path.StartsWith(Application.dataPath)
                ? "Assets" + path.Substring(Application.dataPath.Length)
                : null;

            // Save the PNG file
            File.WriteAllBytes(path, pngData);
            Debug.Log($"Saved texture to: {path}");

            // Refresh the asset database if saved inside the project
            if (!string.IsNullOrEmpty(relativePath))
            {
                AssetDatabase.ImportAsset(relativePath);
                var importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                if (importer != null)
                {
                    // apply all the good import settings
                    importer.textureType = TextureImporterType.Sprite; // Set as sprite
                    importer.isReadable = true; // Make readable for future edits
                    importer.filterMode = FilterMode.Point; // No blurring
                    importer.textureCompression = TextureImporterCompression.Uncompressed; // No compression
                    importer.maxTextureSize = 16384; // highest max size
                    importer.spritePixelsPerUnit = 48; // Pixels per unit = 48

                    // Apply changes
                    AssetDatabase.WriteImportSettingsIfDirty(relativePath);
                    AssetDatabase.Refresh();
                    Debug.Log("Texture settings applied!");
                }
            }
        }

        protected virtual void ClearSelected()
        {
            // Get all pixels
            var allPixels = editableTexture.GetPixels();

            // go through all pixels, if it matches the picked color, get rid of it
            for (var i = 0; i < allPixels.Length; i++)
                if (allPixels[i] == pickedColor)
                    allPixels[i] = Color.clear;

            editableTexture.SetPixels(allPixels); // Apply modified pixels
            editableTexture.Apply(); // Apply changes to the texture
            Repaint(); // redraw everything
        }
    }
}