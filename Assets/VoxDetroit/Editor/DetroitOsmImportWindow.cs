using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using VoxDetroit.Detroit;

namespace VoxDetroit.Editor
{
    public sealed class DetroitOsmImportWindow : EditorWindow
    {
        private const string Endpoint =
            "https://overpass-api.de/api/interpreter";

        private const string OutputPath =
            "Assets/VoxDetroit/Data/downtown-core-prototype.json";

        private const double South = 42.3306;
        private const double West = -83.0475;
        private const double North = 42.3323;
        private const double East = -83.0444;

        private bool _downloading;
        private string _status =
            "Ready to download the Downtown prototype.";

        [MenuItem("Vox Detroit/Detroit OSM Importer")]
        public static void Open()
        {
            GetWindow<DetroitOsmImportWindow>(
                "Detroit OSM Importer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Downtown Core Prototype",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Downloads roads and building footprints from " +
                "OpenStreetMap through Overpass for the fixed M1 " +
                "prototype bounds. Imported data remains subject " +
                "to ODbL attribution requirements.",
                MessageType.Info);

            EditorGUILayout.LabelField(
                "Bounds",
                $"{South}, {West} → {North}, {East}");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Status",
                _status,
                EditorStyles.wordWrappedLabel);

            using (new EditorGUI.DisabledScope(_downloading))
            {
                if (GUILayout.Button("Download + Convert Prototype"))
                {
                    DownloadAndConvert();
                }
            }
        }

        private async void DownloadAndConvert()
        {
            _downloading = true;
            _status = "Downloading OpenStreetMap data...";
            Repaint();

            try
            {
                string query = BuildOverpassQuery();

                using (var client = new HttpClient())
                using (var formContent =
                       new FormUrlEncodedContent(
                           new[]
                           {
                               new KeyValuePair<string, string>(
                                   "data",
                                   query)
                           }))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "VoxDetroit-Unity-Importer/0.1");

                    HttpResponseMessage response =
                        await client.PostAsync(Endpoint, formContent);

                    response.EnsureSuccessStatusCode();
                    string xml =
                        await response.Content.ReadAsStringAsync();

                    ConvertAndSave(xml);
                }
            }
            catch (Exception exception)
            {
                _status = "Import failed: " + exception.Message;
                Debug.LogException(exception);
            }
            finally
            {
                _downloading = false;
                Repaint();
            }
        }

        private void ConvertAndSave(string xml)
        {
            XDocument source = XDocument.Parse(xml);

            var roads = new List<RoadFeature>();
            var buildings = new List<BuildingFeature>();

            foreach (XElement way in source.Descendants("way"))
            {
                Dictionary<string, string> tags =
                    way.Elements("tag")
                        .Where(
                            element =>
                                element.Attribute("k") != null &&
                                element.Attribute("v") != null)
                        .ToDictionary(
                            element => element.Attribute("k").Value,
                            element => element.Attribute("v").Value);

                GeoPoint[] geometry =
                    way.Elements("nd")
                        .Where(
                            nd =>
                                nd.Attribute("lat") != null &&
                                nd.Attribute("lon") != null)
                        .Select(
                            nd =>
                                new GeoPoint(
                                    ParseDouble(
                                        nd.Attribute("lat").Value),
                                    ParseDouble(
                                        nd.Attribute("lon").Value)))
                        .ToArray();

                if (geometry.Length < 2)
                {
                    continue;
                }

                string id = "way/" + way.Attribute("id")?.Value;

                if (tags.TryGetValue("highway", out string highway))
                {
                    roads.Add(
                        new RoadFeature
                        {
                            id = id,
                            name = GetTag(tags, "name"),
                            roadClass = highway,
                            widthMeters =
                                ResolveRoadWidth(tags, highway),
                            centerline = geometry
                        });
                }

                if (tags.TryGetValue("building", out string building))
                {
                    buildings.Add(
                        new BuildingFeature
                        {
                            id = id,
                            name = GetTag(tags, "name"),
                            buildingType = building,
                            heightMeters =
                                ResolveBuildingHeight(tags),
                            levels =
                                ResolveBuildingLevels(tags),
                            footprint = geometry
                        });
                }
            }

            var document = new DetroitImportDocument
            {
                schemaVersion = "1.0",
                sourceName = "OpenStreetMap",
                sourceAttribution = "© OpenStreetMap contributors",
                sourceLicense = "Open Database License (ODbL) 1.0",
                sourceUrl = "https://www.openstreetmap.org/copyright",
                retrievedUtc = DateTime.UtcNow.ToString("O"),
                areaName = "Downtown Detroit Core Prototype",
                bounds = new GeoBounds
                {
                    south = South,
                    west = West,
                    north = North,
                    east = East
                },
                roads = roads.ToArray(),
                buildings = buildings.ToArray()
            };

            string json = JsonUtility.ToJson(document, true);
            string directory = Path.GetDirectoryName(OutputPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(OutputPath, json);
            AssetDatabase.Refresh();

            _status =
                $"Saved {roads.Count} roads and " +
                $"{buildings.Count} buildings to {OutputPath}";

            Debug.Log(_status);
        }

        private static string BuildOverpassQuery()
        {
            string bbox = string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                South,
                West,
                North,
                East);

            return
                "[out:xml][timeout:30];" +
                "(" +
                $"way[\"highway\"]({bbox});" +
                $"way[\"building\"]({bbox});" +
                ");" +
                "out geom tags;";
        }

        private static float ResolveRoadWidth(
            Dictionary<string, string> tags,
            string highway)
        {
            if (tags.TryGetValue("width", out string widthText) &&
                TryParseMeters(widthText, out float explicitWidth))
            {
                return explicitWidth;
            }

            switch (highway)
            {
                case "motorway":
                case "motorway_link":
                    return 18f;
                case "trunk":
                case "primary":
                    return 12f;
                case "secondary":
                    return 10f;
                case "tertiary":
                    return 8f;
                case "residential":
                    return 7f;
                case "service":
                    return 5f;
                case "footway":
                case "path":
                case "pedestrian":
                    return 2.5f;
                default:
                    return 6f;
            }
        }

        private static float ResolveBuildingHeight(
            Dictionary<string, string> tags)
        {
            if (tags.TryGetValue("height", out string heightText) &&
                TryParseMeters(heightText, out float height))
            {
                return height;
            }

            int levels = ResolveBuildingLevels(tags);
            return levels > 0 ? levels * 3.2f : 0f;
        }

        private static int ResolveBuildingLevels(
            Dictionary<string, string> tags)
        {
            if (tags.TryGetValue(
                    "building:levels",
                    out string levelsText) &&
                int.TryParse(
                    levelsText,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int levels))
            {
                return levels;
            }

            return 0;
        }

        private static bool TryParseMeters(
            string value,
            out float meters)
        {
            string trimmed = value.Trim().ToLowerInvariant();

            if (trimmed.EndsWith("ft"))
            {
                string feetText =
                    trimmed.Substring(0, trimmed.Length - 2).Trim();

                if (float.TryParse(
                        feetText,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out float feet))
                {
                    meters = feet * 0.3048f;
                    return true;
                }
            }

            trimmed = trimmed
                .Replace("meters", "")
                .Replace("meter", "")
                .Replace("m", "")
                .Trim();

            return float.TryParse(
                trimmed,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out meters);
        }

        private static string GetTag(
            Dictionary<string, string> tags,
            string key)
        {
            return tags.TryGetValue(key, out string value)
                ? value
                : string.Empty;
        }

        private static double ParseDouble(string value)
        {
            return double.Parse(
                value,
                CultureInfo.InvariantCulture);
        }
    }
}
