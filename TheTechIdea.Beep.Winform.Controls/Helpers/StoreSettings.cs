using ExCSS;
using System;
using System.Configuration;

public static class StoreSettings
{
    /// <summary>
    /// Stores a value in application settings. Creates the setting if it does not exist.
    /// </summary>
    /// <param name="controlGuid">The unique Guid ID of the control.</param>
    /// <param name="propertyName">The property name to store the value for.</param>
    /// <param name="value">The value to store.</param>
    public static void Set(string controlGuid, string propertyName, object value)
    {
        if (string.IsNullOrEmpty(controlGuid) || string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("Control GUID and property name cannot be null or empty.");

        string settingKey = $"{controlGuid}_{propertyName}";

        // Check if the setting exists
        if (!TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Context.Contains(settingKey))
        {
            // Add the setting dynamically if it doesn't exist
            AddSetting(settingKey, value);
        }

        // Update the setting value
        TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default[settingKey] = value;
        TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Save();
    }

    /// <summary>
    /// Retrieves a value from application settings. Returns a default value if the setting does not exist.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="controlGuid">The unique Guid ID of the control.</param>
    /// <param name="propertyName">The property name to retrieve the value for.</param>
    /// <param name="defaultValue">The default value to return if the setting does not exist.</param>
    /// <returns>The stored value or the default value.</returns>
    public static T Get<T>(string controlGuid, string propertyName, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(controlGuid) || string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("Control GUID and property name cannot be null or empty.");

        string settingKey = $"{controlGuid}_{propertyName}";

        // Return the value if the setting exists; otherwise, return the default value
        if (TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Context.Contains(settingKey))
        {
            return (T)Convert.ChangeType(TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default[settingKey], typeof(T));
        }

        // If setting does not exist, create it with the default value
        AddSetting(settingKey, defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Adds a setting to the application settings dynamically.
    /// </summary>
    /// <param name="key">The unique key for the setting.</param>
    /// <param name="defaultValue">The default value for the setting.</param>
    private static void AddSetting(string key, object defaultValue)
    {
        var settingsProperty = new SettingsProperty(key)
        {
            PropertyType = defaultValue?.GetType() ?? typeof(string),
            IsReadOnly = false,
            DefaultValue = defaultValue,
            Provider = TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Providers["LocalFileSettingsProvider"],
            SerializeAs = SettingsSerializeAs.String
        };
        settingsProperty.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());

        TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Properties.Add(settingsProperty);
        TheTechIdea.Beep.Winform.Controls.Properties.Settings.Default.Reload(); // Reload to apply the changes
    }
}
