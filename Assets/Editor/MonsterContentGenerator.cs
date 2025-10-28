#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Engine.Editor
{
    using Engine.Monsters;

    /// <summary>
    /// Procedurally generates moves, monsters, sprites and placeholder audio.
    /// </summary>
    public static class MonsterContentGenerator
    {
        private const int MonsterCount = 500;
        private const string MoveFolder = "Assets/Resources/Moves";
        private const string MonsterFolder = "Assets/Resources/Monsters";
        private const string ArtFolder = "Assets/Art/Monsters";
        private const string AudioFolder = "Assets/Audio";

        [MenuItem("Generate/Monsters & Moves")]
        public static void Generate()
        {
            Directory.CreateDirectory(MoveFolder);
            Directory.CreateDirectory(MonsterFolder);
            Directory.CreateDirectory(ArtFolder);
            Directory.CreateDirectory(AudioFolder);

            var rng = new System.Random(12345);

            var moves = GenerateMoves(rng);
            AssetDatabase.SaveAssets();

            var sprites = new Dictionary<int, (Sprite front, Sprite back)>();
            for (int i = 0; i < MonsterCount; i++)
            {
                sprites[i] = GenerateMonsterSprites(i, rng);
            }

            var monsters = GenerateMonsters(rng, moves, sprites);

            GenerateAudioStubs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Generation complete", $"Generated {moves.Count} moves and {monsters.Count} monsters.", "OK");
        }

        private static List<MoveData> GenerateMoves(System.Random rng)
        {
            var moveList = new List<MoveData>();
            string[] verbs = { "Flare", "Splash", "Quake", "Gale", "Shock", "Frost", "Shade", "Beam", "Mind", "Spore", "Roar", "Strike", "Pulse" };
            foreach (var verb in verbs)
            {
                foreach (ElementType type in Enum.GetValues(typeof(ElementType)))
                {
                    if (type == ElementType.None) continue;
                    string assetPath = Path.Combine(MoveFolder, $"{verb}-{type}.asset");
                    var move = AssetDatabase.LoadAssetAtPath<MoveData>(assetPath);
                    if (move == null)
                    {
                        move = ScriptableObject.CreateInstance<MoveData>();
                        AssetDatabase.CreateAsset(move, assetPath);
                    }
                    var so = new SerializedObject(move);
                    so.FindProperty("moveName").stringValue = $"{verb} {type}";
                    so.FindProperty("description").stringValue = $"A {type} move generated procedurally.";
                    so.FindProperty("elementType").enumValueIndex = (int)type;
                    so.FindProperty("power").intValue = rng.Next(30, 95);
                    so.FindProperty("accuracy").intValue = rng.Next(75, 101);
                    so.FindProperty("isMagical").boolValue = rng.NextDouble() > 0.5;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    moveList.Add(move);
                }
            }

            return moveList;
        }

        private static Dictionary<int, MonsterSpeciesData> GenerateMonsters(System.Random rng, List<MoveData> moves, Dictionary<int, (Sprite front, Sprite back)> sprites)
        {
            string[] prefixes = { "Pyro", "Aqua", "Geo", "Aero", "Volt", "Cryo", "Umbr", "Lumi", "Psy", "Toxi", "Ferr", "Sylv", "Spect", "Lyca", "Gran", "Temp" };
            string[] suffixes = { "ling", "drake", "mite", "phant", "bloom", "hound", "geist", "horn", "paw", "wing", "core", "stone", "shade", "spark", "scale", "shell" };

            var monsters = new Dictionary<int, MonsterSpeciesData>();
            MonsterSpeciesData pendingEvolution = null;
            for (int i = 0; i < MonsterCount; i++)
            {
                string assetPath = Path.Combine(MonsterFolder, $"Monster_{i:000}.asset");
                var species = AssetDatabase.LoadAssetAtPath<MonsterSpeciesData>(assetPath);
                if (species == null)
                {
                    species = ScriptableObject.CreateInstance<MonsterSpeciesData>();
                    AssetDatabase.CreateAsset(species, assetPath);
                }
                var so = new SerializedObject(species);
                string name = $"{prefixes[i % prefixes.Length]}{suffixes[i % suffixes.Length]}";
                so.FindProperty("speciesName").stringValue = name;
                var types = Enum.GetValues(typeof(ElementType));
                var primary = (ElementType)types.GetValue(rng.Next(1, types.Length));
                var secondary = rng.NextDouble() > 0.7 ? (ElementType)types.GetValue(rng.Next(1, types.Length)) : ElementType.None;
                so.FindProperty("primaryType").enumValueIndex = (int)primary;
                so.FindProperty("secondaryType").enumValueIndex = (int)secondary;
                so.FindProperty("baseHP").intValue = rng.Next(35, 110);
                so.FindProperty("baseAttack").intValue = rng.Next(20, 110);
                so.FindProperty("baseDefense").intValue = rng.Next(20, 110);
                so.FindProperty("baseSpeed").intValue = rng.Next(20, 110);
                so.FindProperty("baseMagic").intValue = rng.Next(20, 110);
                so.FindProperty("frontSprite").objectReferenceValue = sprites[i].front;
                so.FindProperty("backSprite").objectReferenceValue = sprites[i].back;

                var learnset = so.FindProperty("learnset");
                learnset.arraySize = 4;
                for (int m = 0; m < 4; m++)
                {
                    int level = 1 + m * 5 + rng.Next(0, 5);
                    var entry = learnset.GetArrayElementAtIndex(m);
                    entry.FindPropertyRelative("level").intValue = level;
                    entry.FindPropertyRelative("move").objectReferenceValue = moves[rng.Next(moves.Count)];
                }

                so.FindProperty("evolutionLevel").intValue = 0;
                so.FindProperty("evolutionTarget").objectReferenceValue = null;

                if (pendingEvolution != null)
                {
                    var evolveSO = new SerializedObject(pendingEvolution);
                    evolveSO.FindProperty("evolutionLevel").intValue = rng.Next(16, 36);
                    evolveSO.FindProperty("evolutionTarget").objectReferenceValue = species;
                    evolveSO.ApplyModifiedPropertiesWithoutUndo();
                    pendingEvolution = null;
                }

                if (rng.NextDouble() > 0.7)
                {
                    pendingEvolution = species;
                }

                so.ApplyModifiedPropertiesWithoutUndo();
                monsters[i] = species;
            }

            return monsters;
        }

        private static (Sprite front, Sprite back) GenerateMonsterSprites(int index, System.Random rng)
        {
            int size = 64;
            Color baseColor = RandomColor(rng);
            Texture2D front = CreateTexture(size, size, baseColor, rng);
            Texture2D back = CreateTexture(size, size, baseColor * 0.8f, rng);

            string frontPath = Path.Combine(ArtFolder, $"monster_front_{index:000}.png");
            string backPath = Path.Combine(ArtFolder, $"monster_back_{index:000}.png");
            File.WriteAllBytes(frontPath, front.EncodeToPNG());
            File.WriteAllBytes(backPath, back.EncodeToPNG());
            AssetDatabase.ImportAsset(frontPath);
            AssetDatabase.ImportAsset(backPath);

            var frontSprite = SpriteFromTexture(frontPath);
            var backSprite = SpriteFromTexture(backPath);
            return (frontSprite, backSprite);
        }

        private static Texture2D CreateTexture(int width, int height, Color color, System.Random rng)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, Color.Lerp(color, Color.white, (float)rng.NextDouble() * 0.3f));
                }
            }

            int radius = rng.Next(10, width / 2);
            Vector2 center = new Vector2(width / 2f, height / 2f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (Vector2.Distance(new Vector2(x, y), center) < radius)
                    {
                        texture.SetPixel(x, y, Color.Lerp(color, Color.black, 0.2f));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static Sprite SpriteFromTexture(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static Color RandomColor(System.Random rng)
        {
            return new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1f);
        }

        private static void GenerateAudioStubs()
        {
            string[] files = { "battle_theme.wav", "capture.wav", "hit.wav" };
            foreach (var file in files)
            {
                string path = Path.Combine(AudioFolder, file);
                if (!File.Exists(path))
                {
                    WriteSineWave(path, 440f, 0.5f);
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        private static void WriteSineWave(string path, float frequency, float durationSeconds)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * durationSeconds);
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream);

            int byteRate = sampleRate * 2;
            int dataSize = samples * 2;

            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                short sample = (short)(Mathf.Sin(2f * Mathf.PI * frequency * t) * short.MaxValue);
                writer.Write(sample);
            }
        }
    }
}
#endif
