using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Editor
{
    public class SpriteEditorV2 : SpriteEditor
    {
        // actions are put on a stack
        // stacks are first in last out, which is good for undoing a list of actions from recently to less recentyl
        private readonly Stack<object> undoActionStack = new();
    
        private int GridSize = 36; // width and height of the grid in pixels
    
    
        private bool isDrawing; // keeps track of strokes

        private int pixelsPerUnit = 48; // the amount of pixels that fit into one unit of Unity world space
    
        // dictionary for pixels, each entry is a pixel at a location and of a color
        private readonly Dictionary<Vector2Int, Color> strokeSnapshot = new();

        private Color switchedToColor = Color.white; // all the pixels of pixkedColor, turn into this color

        /// <summary>
        /// Called every redraw, redraw meaning things like clicking, opening, closing, or moving the window
        /// </summary>
        private new void OnGUI()
        {
            // detect ctrl-Z for undo, doesn't work for some reason
            var e = Event.current;

            if (e.type == EventType.KeyDown && e.modifiers == EventModifiers.Command && e.keyCode == KeyCode.Z)
            {
                Undo();
                e.Use();
            }

            GUILayout.Label("Edit a Sprite", EditorStyles.boldLabel);

            pickedColor = EditorGUILayout.ColorField("Selected Color: ", pickedColor);

            switchedToColor = EditorGUILayout.ColorField("Switch to this Color: ", switchedToColor);

            pixelsPerUnit = EditorGUILayout.IntField("Pixels Per Unit", pixelsPerUnit);

            // detects changes in variables per gui refresh, very useful
            EditorGUI.BeginChangeCheck();
            var newGridSize = EditorGUILayout.IntField("Grid Size: ", GridSize);

            if (EditorGUI.EndChangeCheck())
            {
                GridSize = Mathf.Max(1, newGridSize); // at least 1
                spriteToEdit = null;
                InitEditableTexture();
                Repaint();
            }

            var inputSprite =
                EditorGUILayout.ObjectField("Sprite To Edit: ", spriteToEdit, typeof(Texture2D), false) as Texture2D;

            // input sprite into the sprite you are currently editing...
            if (inputSprite != spriteToEdit)
            {
                spriteToEdit = inputSprite;
                InitEditableTexture();
                Repaint();
            }

            // if no inputted sprite, assume blank canvas to draw on
            if (editableTexture == null)
            {
                InitEditableTexture();
                Repaint();
            }

            GUILayout.Space(-25);

            // Buttons
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Undo", GUILayout.Width(100), GUILayout.Height(25))) Undo();

            GUILayout.Space(10);

            if (GUILayout.Button(isErasing ? "Eraser [Pen]" : "Pen [Eraser]", GUILayout.Width(100), GUILayout.Height(25)))
                isErasing = !isErasing;

            GUILayout.Space(10);

            if (GUILayout.Button("Clear Selected", GUILayout.Width(100), GUILayout.Height(25))) ClearSelected();

            GUILayout.Space(10);

            if (GUILayout.Button("Switch Colors", GUILayout.Width(100), GUILayout.Height(25))) SwitchColors();

            GUILayout.Space(10);

            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(25))) Save();

            GUILayout.Space(10);

            if (GUILayout.Button(GridOn ? "Grid (On)" : "Grid (Off)", GUILayout.Width(100), GUILayout.Height(25)))
                GridOn = !GridOn;

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // move draw area via that second parameter
            var drawArea = new Rect(10, 175, position.width - 20, position.height - 130);

            GUI.Box(drawArea, GUIContent.none);

            HandleMouseInput(drawArea);

            DrawSprite(drawArea);

            if (GridOn) DrawGrid(drawArea);
        }

        /// <summary>
        /// set a pixel at a location
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        protected override void SetPixelColor(int x, int y)
        {
            if (editableTexture == null)
            {
                Debug.LogWarning("Can't set pixel color, editable texture is null");
                return;
            }

            // Ensure coordinates are within grid
            if (x >= 0 && x < editableTexture.width && y >= 0 && y < editableTexture.height)
            {
                var newColor = isErasing ? Color.clear : pickedColor;

                // Store previous color only if it's not in the stroke list already
                var pixelPos = new Vector2Int(x, y);
                if (!strokeSnapshot.ContainsKey(pixelPos)) // Fix
                {
                    var previousColor = editableTexture.GetPixel(x, editableTexture.height - y - 1);
                    strokeSnapshot[pixelPos] = previousColor;
                }

                // Apply new color
                editableTexture.SetPixel(x, editableTexture.height - y - 1, newColor);
                editableTexture.Apply();
                Repaint();
            }
        }

        protected override void HandleMouseInput(Rect area)
        {
            var evt = Event.current;
            var localMousePos = evt.mousePosition - new Vector2(area.x, area.y);
            var pixelX = Mathf.FloorToInt(localMousePos.x / 10);
            var pixelY = Mathf.FloorToInt(localMousePos.y / 10);

            if (evt.type == EventType.MouseDown && evt.button == 0) // mouse down...
            {
                isDrawing = true;
                strokeSnapshot.Clear();
                SetPixelColor(pixelX, pixelY);
                evt.Use();
            }
            else if (evt.type == EventType.MouseDrag && isDrawing) // ...dragging...
            {
                SetPixelColor(pixelX, pixelY);
                evt.Use();
            }
            else if (evt.type == EventType.MouseUp && isDrawing) // ... mouse up
            {
                isDrawing = false;
                if (strokeSnapshot.Count > 0)
                    undoActionStack.Push(
                        new Dictionary<Vector2Int, Color>(strokeSnapshot)); // Fix: Push a copy of the stroke
                evt.Use();
            }
        }

        /// <summary>
        /// add to undo stack 
        /// </summary>
        private void AddToActionStack()
        {
            if (editableTexture == null) return;

            undoActionStack.Push(editableTexture.GetPixels()); 
        }

        /// <summary>
        /// switch all colors of selected to switch
        /// exactly the same as clear selecetd, except of setting clear sets to a color
        /// </summary>
        private void SwitchColors()
        {
            // Get all pixels
            var allPixels = editableTexture.GetPixels();

            AddToActionStack();

            // go through all pixels, if it matches the picked color, make it the new color
            for (var i = 0; i < allPixels.Length; i++)
                if (allPixels[i] == pickedColor)
                    allPixels[i] = switchedToColor;

            editableTexture.SetPixels(allPixels); // Apply modified pixels
            editableTexture.Apply(); // Apply changes to the texture
            Repaint(); // redraw everything
        }

        protected override void ClearSelected()
        {
            // Get all pixels
            var allPixels = editableTexture.GetPixels();

            AddToActionStack();

            // go through all pixels, if it matches the picked color, get rid of it
            for (var i = 0; i < allPixels.Length; i++)
                if (allPixels[i] == pickedColor)
                    allPixels[i] = Color.clear;

            editableTexture.SetPixels(allPixels); // Apply modified pixels
            editableTexture.Apply(); // Apply changes to the texture
            Repaint(); // redraw everything
        }

        // creates the copied texture
        protected override void InitEditableTexture()
        {
            // if there is an inputted sprite, see if it is readable
            if (spriteToEdit != null && !spriteToEdit.isReadable)
                Debug.LogError(
                    $"Texture '{spriteToEdit.name}' is not readable. Enable 'Read/Write' in the import settings.");

            // readable inputted sprite...
            if (spriteToEdit != null)
            {
                // Create a copy of the original texture
                editableTexture = new Texture2D(spriteToEdit.width, spriteToEdit.height, TextureFormat.RGBA32, false);
                editableTexture.SetPixels(spriteToEdit.GetPixels());
            }

            // no sprite inputted? make a fake, blank sprite
            else
            {
                // Create a transparent texture that just fills the grid size
                editableTexture = new Texture2D(GridSize, GridSize, TextureFormat.RGBA32, false);
                var pixels = new Color[GridSize * GridSize];

                for (var i = 0; i < pixels.Length; i++)
                    pixels[i] = new Color(0, 0, 0, 0);

                editableTexture.SetPixels(pixels);
            }

            // copying a texture apparently doesn't maintain all the import settings, i have to set those here
            editableTexture.filterMode = FilterMode.Point;
            editableTexture.wrapMode = TextureWrapMode.Clamp;

            editableTexture.Apply();
        }

        // only diff between this and the base one is the ppu, which isn't a constant but a variable now
        protected override void Save()
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

                    var ppu = Mathf.Max(1, pixelsPerUnit); // at least one ppu required
                    importer.spritePixelsPerUnit =
                        ppu; // Pixels per unit (ie 48 pixels fit in one unit of unity world space)

                    // Apply changes
                    AssetDatabase.WriteImportSettingsIfDirty(relativePath);
                    AssetDatabase.Refresh();
                    Debug.Log("Texture settings applied!");
                }
            }
        }

        /// <summary>
        /// undoes the last action
        /// </summary>
        private void Undo()
        {
            if (undoActionStack.Count == 0) return;

            var lastAction = undoActionStack.Pop();

            if (lastAction is Dictionary<Vector2Int, Color> stroke) // if stroke...
                foreach (var pixel in stroke)
                    editableTexture.SetPixel(pixel.Key.x, editableTexture.height - pixel.Key.y - 1, pixel.Value);
        
            else if (lastAction is Color[] fullTextureState) // if full texture action, ie clear selection and color switch
                editableTexture.SetPixels(fullTextureState);

            editableTexture.Apply();
            Repaint();
        }


        [MenuItem("Tools/Sprite Editor V2")]
        public new static void ShowWindow()
        {
            GetWindow<SpriteEditorV2>("Sprite Editor V2");
        }
    }
}