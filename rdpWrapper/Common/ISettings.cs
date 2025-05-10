namespace sergiye.Common {
  public interface ISettings
  {
      /// <summary>
      /// Returns information whether the given collection of settings contains a value assigned to the given key.
      /// </summary>
      /// <param name="name">Key to which the setting value is assigned.</param>
      bool Contains(string name);

      /// <summary>
      /// Assigns a setting option to a given key.
      /// </summary>
      /// <param name="name">Key to which the setting value is assigned.</param>
      /// <param name="value">Text setting value.</param>
      void SetValue(string name, string value);

      /// <summary>
      /// Gets a setting option assigned to the given key.
      /// </summary>
      /// <param name="name">Key to which the setting value is assigned.</param>
      /// <param name="value">Default value.</param>
      string GetValue(string name, string value);

      /// <summary>
      /// Removes a setting with the specified key from the settings collection.
      /// </summary>
      /// <param name="name">Key to which the setting value is assigned.</param>
      void Remove(string name);
  }
}
