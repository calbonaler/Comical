using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Forms;

namespace Comical;

sealed class Settings
{
	static Settings()
	{
		_serializerOptions = new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			Converters = { new JsonStringEnumConverter(), new RectangleJsonConverter() },
		};
		_serializerOptions.MakeReadOnly(true);
	}
	[JsonConstructor]
	Settings() { }

	static readonly Lock _lock = new();
	static readonly JsonSerializerOptions _serializerOptions;

	public FormWindowState EditorWindowState { get; set; } = FormWindowState.Normal;
	public Rectangle EditorWindowBounds { get; set; } = new(0, 0, 800, 600);
	public ObservableCollection<string> RecentAuthors { get; init; } = [];

	public static Settings Default
	{
		get
		{
			lock (_lock)
			{
				if (field is not null) return field;
				if (File.Exists(SettingsFilePath))
				{
					using var stream = File.OpenRead(SettingsFilePath);
					field = JsonSerializer.Deserialize<Settings>(stream, _serializerOptions);
					return field!;
				}
				else
				{
					field = new();
					return field;
				}
			}
		}
	}
	public void Save()
	{
		Directory.CreateDirectory(SettingsDirectoryPath);
		using var stream = File.Create(SettingsFilePath);
		JsonSerializer.Serialize(stream, this, _serializerOptions);
	}

	public static void LoadDockPanelConfiguration(Action<Stream> action)
	{
		if (!File.Exists(DockPanelConfigurationFilePath)) return;
		using var stream = File.OpenRead(DockPanelConfigurationFilePath);
		action(stream);
	}
	public static void SaveDockPanelConfiguration(Action<Stream, Encoding> action)
	{
		Directory.CreateDirectory(SettingsDirectoryPath);
		using var stream = File.Create(DockPanelConfigurationFilePath);
		action(stream, new UTF8Encoding(false));
	}

	static string SettingsDirectoryPath
	{
		get
		{
			var thisAssembly = Assembly.GetExecutingAssembly();
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				thisAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()!.Company,
				thisAssembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product);
		}
	}
	static string SettingsFilePath => Path.Combine(SettingsDirectoryPath, "Settings.json");
	static string DockPanelConfigurationFilePath => Path.Combine(SettingsDirectoryPath, "DockPanelConfiguration.xml");

	sealed class RectangleJsonConverter : JsonConverter<Rectangle>
	{
		public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var result = new Rectangle();
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;
				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					var propertyName = reader.GetString();
					reader.Read();
					switch (propertyName)
					{
						case nameof(result.X): result.X = reader.GetInt32(); break;
						case nameof(result.Y): result.Y = reader.GetInt32(); break;
						case nameof(result.Width): result.Width = reader.GetInt32(); break;
						case nameof(result.Height): result.Height = reader.GetInt32(); break;
					}
				}
			}
			return result;
		}
		public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(value.X), value.X);
			writer.WriteNumber(nameof(value.Y), value.Y);
			writer.WriteNumber(nameof(value.Width), value.Width);
			writer.WriteNumber(nameof(value.Height), value.Height);
			writer.WriteEndObject();
		}
	}
}
