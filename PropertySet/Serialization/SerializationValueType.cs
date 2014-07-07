namespace Klaudwerk.PropertySet.Serialization
{
    /// <summary>
    /// Enum that define "quick" serialization data types
    /// </summary>
    public enum SerializationValueType
    {
        /// <summary>
        /// The type is string
        /// </summary>
        String =1,
        /// <summary>
        /// The type is integer
        /// </summary>
        Int,
        /// <summary>
        /// The type ils Long
        /// </summary>
        Long,
        /// <summary>
        /// The type is Bool
        /// </summary>
        Bool,
        /// <summary>
        /// The type is Double
        /// </summary>
        Double,
        /// <summary>
        /// The type is Date Time
        /// </summary>
        DateTime,
        /// <summary>
        /// The type is a Byte array
        /// </summary>
        ByteArray,
        /// <summary>
        /// The type is Object
        /// </summary>
        Object
    }
}