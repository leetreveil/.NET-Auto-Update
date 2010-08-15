using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NAppUpdate.Framework.Conditions
{
    public class OSCondition : IUpdateCondition
    {
        public OSCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        #region IUpdateCondition Members

        public IDictionary<string, string> Attributes { get; private set; }

        public bool IsMet(NAppUpdate.Framework.Tasks.IUpdateTask task)
        {
            // OS bitness check, if requested
            if (Attributes.ContainsKey("bit"))
            {
                bool Is64Bit = Is64BitOperatingSystem();
                if ("32".Equals(Attributes["bit"]) && Is64Bit)
                    return false;
                else if ("64".Equals(Attributes["bit"]) && !Is64Bit)
                    return false;
            }
            return true;

            // TODO: Work with enums on code and Attributes to get a proper and full OS version comparison
            // use http://stackoverflow.com/questions/545666/how-to-translate-ms-windows-os-version-numbers-into-product-names-in-net
            // and http://msdn.microsoft.com/en-us/library/ms724429(VS.85).aspx
            throw new NotImplementedException();
        }

        #endregion

        // Check OS bitness (32 / 64 bit)
        // As seen on http://1code.codeplex.com/SourceControl/changeset/view/39074#842775
        #region Is64BitOperatingSystem (IsWow64Process)

        /// <summary>
        /// The function determines whether the current operating system is a 
        /// 64-bit operating system.
        /// </summary>
        /// <returns>
        /// The function returns true if the operating system is 64-bit; 
        /// otherwise, it returns false.
        /// </returns>
        public static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
            {
                return true;
            }
            else  // 32-bit programs run on both 32-bit and 64-bit Windows
            {
                // Detect whether the current process is a 32-bit process 
                // running on a 64-bit system.
                bool flag;
                return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    IsWow64Process(GetCurrentProcess(), out flag)) && flag);
            }
        }

        /// <summary>
        /// The function determins whether a method exists in the export 
        /// table of a certain module.
        /// </summary>
        /// <param name="moduleName">The name of the module</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns>
        /// The function returns true if the method specified by methodName 
        /// exists in the export table of the module specified by moduleName.
        /// </returns>
        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)]string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        #endregion
    }
}
