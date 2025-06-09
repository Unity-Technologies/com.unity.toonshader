namespace UnityEngine.TestTools.Graphics
{
    /// <summary>
    /// An enum that represents the different graphics vendors.
    /// </summary>
    /// <remarks>
    /// This enum is used to represent the different graphics vendors. It is used in the <see cref="GraphicsDeviceInfo"/> class to check the vendor of the current graphics device. The values of the enum are the vendor IDs of the different graphics vendors as defined in the <see cref="SystemInfo.graphicsDeviceVendorID"/> property.
    /// </remarks>
    public enum GraphicsVendor
    {
        Nvidia          = 0x10de,
        AMD             = 0x1002,
        Intel           = 0x8086,
        ARM             = 0x13b5,
        Qualcomm        = 0x5143,
        Apple           = 0x106b
    }

    /// <summary>
    /// A class that provides information about the current graphics device.
    /// </summary>
    /// <remarks>
    /// This class is a wrapper around the <see cref="SystemInfo"/> class and provides a way to query the current graphics device information. It provides properties to get the name, type, ID, vendor, vendor ID, version, memory size, and shader level of the current graphics device. It also provides methods to check if the current graphics device is of a specific vendor, has at least a certain memory size, has at least a certain shader level, or has at least a certain version. The class also provides a method to print the current graphics device information.
    /// </remarks>
    public static class GraphicsDeviceInfo
    {
        /// <summary>
        /// The name of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is a string that represents the name of the graphics device. See <see cref="SystemInfo.graphicsDeviceName"/> for more information.
        /// </remarks>
        public static string Name => SystemInfo.graphicsDeviceName;

        /// <summary>
        /// The type of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is an enum that represents the type of the graphics device. See <see cref="SystemInfo.graphicsDeviceType"/> for more information.
        /// </remarks>
        public static Rendering.GraphicsDeviceType Type => SystemInfo.graphicsDeviceType;

        /// <summary>
        /// The ID of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is a unique identifier for the graphics device. See <see cref="SystemInfo.graphicsDeviceID"/> for more information.
        /// </remarks>
        public static int DeviceID => SystemInfo.graphicsDeviceID;

        /// <summary>
        /// The vendor of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is a string that represents the vendor of the graphics device. See <see cref="SystemInfo.graphicsDeviceVendor"/> for more information.
        /// </remarks>
        public static string Vendor => SystemInfo.graphicsDeviceVendor;

        /// <summary>
        /// The vendor ID of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is a unique identifier for the vendor of the graphics device. See <see cref="SystemInfo.graphicsDeviceVendorID"/> for more information.
        /// </remarks>
        public static int VendorID => SystemInfo.graphicsDeviceVendorID;

        /// <summary>
        /// The version of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is a string that represents the version of the graphics device. See <see cref="SystemInfo.graphicsDeviceVersion"/> for more information.
        /// </remarks>
        public static string Version => SystemInfo.graphicsDeviceVersion;

        /// <summary>
        /// The memory size of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is an integer that represents the memory size of the graphics device. See <see cref="SystemInfo.graphicsMemorySize"/> for more information.
        /// </remarks>
        public static int MemorySize => SystemInfo.graphicsMemorySize;

        /// <summary>
        /// The shader level of the graphics device.
        /// </summary>
        /// <remarks>
        /// This is an integer that represents the shader level of the graphics device. See <see cref="SystemInfo.graphicsShaderLevel"/> for more information.
        /// </remarks>
        public static int ShaderLevel => SystemInfo.graphicsShaderLevel;

        /// <summary>
        /// Checks if the current graphics device is of the specified vendor.
        /// </summary>
        /// <param name="vendor">The vendor to check for</param>
        /// <returns>True if the current graphics device is of the specified vendor, false otherwise</returns>
        /// <remarks>
        /// This method checks if the current graphics device is of the specified vendor. It does this by comparing the vendor ID of the current graphics device with the vendor ID of the specified vendor.
        /// </remarks>
        public static bool IsVendor(GraphicsVendor vendor) => VendorID == (int)vendor;

        /// <summary>
        /// Checks if the current graphics device memory size is at least the specified size.
        /// </summary>
        /// <param name="size">The size to check for</param>
        /// <returns>True if the current graphics device memory size is at least the specified size, false otherwise</returns>
        public static bool IsMemorySizeAtLeast(int size) => MemorySize >= size;

        /// <summary>
        /// Checks if the current graphics device shader level is at least the specified level.
        /// </summary>
        /// <param name="level">The level to check for</param>
        /// <returns>True if the current graphics device shader level is at least the specified level, false otherwise</returns>
        public static bool IsShaderLevelAtLeast(int level) => ShaderLevel >= level;

        /// <summary>
        /// Checks if the current graphics device version is at least the specified version.
        /// </summary>
        /// <param name="version">The version to check for</param>
        /// <returns>True if the current graphics device version is at least the specified version, false otherwise</returns>
        public static bool IsVersionAtLeast(string version) => Version.CompareTo(version) >= 0;

        /// <summary>
        /// Returns a string representation of the current graphics device information.
        /// </summary>
    public static string PrintDeviceInfo()
        => $"\n==== Graphics Device Information ====\nName: {Name}\nType: {Type}\nVendor: {Vendor}\nVendorID: {VendorID}\nDeviceID: {DeviceID}\nVersion: {Version}\nMemorySize: {MemorySize}\nShaderLevel: {ShaderLevel}\n=====================================\n";
    }
}
