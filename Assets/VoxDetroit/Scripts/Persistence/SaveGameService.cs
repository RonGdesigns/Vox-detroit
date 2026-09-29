using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoxDetroit.Persistence
{
    public sealed class SaveGameService
    {
        private const string Extension = ".voxsave.json";
        private readonly string _directory;

        public SaveGameService(string directory = null)
        {
            _directory = string.IsNullOrWhiteSpace(directory)
                ? Path.Combine(
                    Application.persistentDataPath,
                    "VoxDetroit",
                    "Saves")
                : directory;
        }

        public string Save(
            string slotName,
            VoxDetroitSaveData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ValidateSchemaForWrite(data);

            Directory.CreateDirectory(_directory);

            string safeSlot = SanitizeSlotName(slotName);
            string path = GetPath(safeSlot);
            string tempPath = path + ".tmp";
            string backupPath = path + ".bak";

            string now = DateTime.UtcNow.ToString("O");

            if (string.IsNullOrWhiteSpace(data.saveId))
            {
                data.saveId = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(data.createdUtc))
            {
                data.createdUtc = now;
            }

            data.updatedUtc = now;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(tempPath, json);

            if (File.Exists(path))
            {
                File.Copy(path, backupPath, true);
                File.Delete(path);
            }

            File.Move(tempPath, path);
            return path;
        }

        public VoxDetroitSaveData Load(string slotName)
        {
            string path = GetPath(
                SanitizeSlotName(slotName));

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Save slot does not exist.",
                    path);
            }

            string json = File.ReadAllText(path);
            VoxDetroitSaveData data =
                JsonUtility.FromJson<VoxDetroitSaveData>(json);

            ValidateLoaded(data);
            return SaveDataNormalizer.Normalize(data);
        }

        public bool TryLoad(
            string slotName,
            out VoxDetroitSaveData data,
            out string error)
        {
            try
            {
                data = Load(slotName);
                error = null;
                return true;
            }
            catch (Exception exception)
            {
                data = null;
                error = exception.Message;
                return false;
            }
        }

        public IReadOnlyList<string> ListSlots()
        {
            if (!Directory.Exists(_directory))
            {
                return Array.Empty<string>();
            }

            string[] files =
                Directory.GetFiles(
                    _directory,
                    "*" + Extension,
                    SearchOption.TopDirectoryOnly);

            var slots = new List<string>(files.Length);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);

                if (name.EndsWith(
                        Extension,
                        StringComparison.OrdinalIgnoreCase))
                {
                    slots.Add(
                        name.Substring(
                            0,
                            name.Length - Extension.Length));
                }
            }

            slots.Sort(StringComparer.OrdinalIgnoreCase);
            return slots;
        }

        public bool Delete(string slotName)
        {
            string path = GetPath(
                SanitizeSlotName(slotName));

            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);

            string backup = path + ".bak";
            if (File.Exists(backup))
            {
                File.Delete(backup);
            }

            return true;
        }

        private string GetPath(string safeSlot)
        {
            return Path.Combine(
                _directory,
                safeSlot + Extension);
        }

        private static string SanitizeSlotName(string slotName)
        {
            if (string.IsNullOrWhiteSpace(slotName))
            {
                throw new ArgumentException(
                    "Save slot name is required.",
                    nameof(slotName));
            }

            char[] invalid = Path.GetInvalidFileNameChars();
            var chars = slotName.Trim().ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (Array.IndexOf(invalid, chars[i]) >= 0 ||
                    char.IsWhiteSpace(chars[i]))
                {
                    chars[i] = '_';
                }
            }

            string result = new string(chars);

            if (result.Length > 64)
            {
                result = result.Substring(0, 64);
            }

            return result;
        }

        private static void ValidateSchemaForWrite(
            VoxDetroitSaveData data)
        {
            if (data.schemaVersion !=
                SaveSchema.CurrentVersion)
            {
                throw new InvalidOperationException(
                    $"Cannot write save schema {data.schemaVersion}; " +
                    $"current schema is {SaveSchema.CurrentVersion}.");
            }
        }

        private static void ValidateLoaded(
            VoxDetroitSaveData data)
        {
            if (data == null)
            {
                throw new InvalidDataException(
                    "Save file could not be parsed.");
            }

            if (data.schemaVersion >
                SaveSchema.CurrentVersion)
            {
                throw new InvalidDataException(
                    "This save was created by a newer version " +
                    "of Vox Detroit.");
            }

            if (data.schemaVersion <
                SaveSchema.CurrentVersion)
            {
                throw new InvalidDataException(
                    $"Save schema {data.schemaVersion} requires " +
                    "a migration before it can be loaded.");
            }
        }
    }
}
