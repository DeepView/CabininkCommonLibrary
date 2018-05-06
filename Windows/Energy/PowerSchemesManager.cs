using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cabinink.Windows.Energy
{
   public struct SSystemPowerScheme
   {
      public string FriendlyName;
      public Guid GUID;
   }
   [Serializable]
   [ComVisible(true)]
   public class PowerSchemesManager
   {
      private List<SSystemPowerScheme> _schemes;
      private List<Guid> _guids;//电源计划方案的GUID集合。
      /// <summary>
      /// 枚举电源方案中的指定元素，通常在循环中调用此函数，以递增Index参数以检索子项，直到枚举它们为止。
      /// </summary>
      /// <param name="rootPowerKey">此参数保留为将来使用，必须设置为NULL。</param>
      /// <param name="schemeGuid">电源方案的标识符，如果此参数为NULL，则返回电源策略的枚举。</param>
      /// <param name="subGroupOfPowerSettingsGuid">电源设置的子组。如果此参数为NULL，则返回PolicyGuid项下的设置枚举。
      /// <para>NO_SUBGROUP_GUID：fea3413e-7e05-4911-9a71-700331f1c294，Settings in this subgroup are part of the default power scheme.</para>
      /// <para>GUID_DISK_SUBGROUP：0012ee47-9041-4b5d-9b77-535fba8b1442，Settings in this subgroup control power management configuration of the system's hard disk drives.</para>
      /// <para>GUID_SYSTEM_BUTTON_SUBGROUP：4f971e89-eebd-4455-a8de-9e59040e7347，Settings in this subgroup control configuration of the system power buttons.</para>
      /// <para>GUID_PROCESSOR_SETTINGS_SUBGROUP：54533251-82be-4824-96c1-47b60b740d00，Settings in this subgroup control configuration of processor power management features.</para>
      /// <para>GUID_VIDEO_SUBGROUP：7516b95f-f776-4464-8c53-06167f40cc99，Settings in this subgroup control configuration of the video power management features.</para>
      /// <para>GUID_BATTERY_SUBGROUP：e73a048d-bf27-4f12-9731-8b2076e8891f，Settings in this subgroup control battery alarm trip points and actions.</para>
      /// <para>GUID_SLEEP_SUBGROUP：238C9FA8-0AAD-41ED-83F4-97BE242C8F20，Settings in this subgroup control system sleep settings.</para>
      /// <para>GUID_PCIEXPRESS_SETTINGS_SUBGROUP：501a4d13-42af-4429-9fd1-a8218c268e20，Settings in this subgroup control PCI Express settings.</para></param>
      /// <param name="accessFlags">指定要枚举的内容的一组标志。</param>
      /// <param name="index">正在枚举的方案、子组或设置的从零开始的索引。</param>
      /// <param name="buffer">指向要接收元素的变量的指针，如果此参数为NULL，则该函数将检索所需缓冲区的大小。</param>
      /// <param name="bufferSize">指向输入的变量的指针包含缓冲区参数指向的缓冲区的大小，如果缓冲区参数为NULL，或者bufferSize不够大，则该函数将返回ERROR_MORE_DATA，并且该变量接收到所需的缓冲区大小。</param>
      /// <returns>如果调用成功，则返回写入0，如果调用失败，则为非零值，如果BufferSize参数中传递的缓冲区大小太小，或者缓冲区参数为NULL，则将返回ERROR_MORE_DATA，并由bufferSize参数指向的DWORD将用所需的缓冲区大小填充。</returns>
      [DllImport("powrprof.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern uint PowerEnumerate(IntPtr rootPowerKey, IntPtr schemeGuid, IntPtr subGroupOfPowerSettingsGuid, EPowerDataAccessor accessFlags, uint index, IntPtr buffer, ref uint bufferSize);
      /// <summary>
      /// 将接口标识符转换为可打印字符的字符串。
      /// </summary>
      /// <param name="iid">要转换的接口标识符。</param>
      /// <param name="result">接收指向结果字符串的指针的指针变量的地址。</param>
      /// <returns>此函数可以返回标准返回值 E_OUTOFMEMORY 和 S_OK。</returns>
      [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
      private static extern int StringFromIID([MarshalAs(UnmanagedType.LPStruct)]Guid iid, [MarshalAs(UnmanagedType.LPTStr)]ref string result);
      /// <summary>
      /// 将UTF-16（宽字符）字符串映射到新的字符串。
      /// </summary>
      /// <param name="codePage">用于执行转换的代码页，此参数可以设置为操作系统中安装或可用的任何代码页的值，应用程序还可以指定下表中显示的值之一：
      /// <para>CP_ACP：系统默认的 Windows ANSI 代码页。注意在不同的计算机上，即使在同一网络上，此值也可能不同。它可以在同一台计算机上进行更改，导致存储的数据变得将损坏。此值仅用于临时使用，如果可能，永久存储应使用 UTF-16 或 UTF-8。</para>
      /// <para>CP_MACCP：当前系统 Macintosh 代码页。注意在不同的计算机上，即使在同一网络上，此值也可能不同。它可以在同一台计算机上进行更改，从而导致存储的数据变得将损坏。此值仅用于临时使用，如果可能，永久存储应使用 UTF-16 或 UTF-8。注意此值主要用于旧式代码，通常不需要，因为现代 Macintosh 计算机使用 Unicode 进行编码。</para>
      /// <para>CP_OEMCP：当前系统 OEM 代码页。注意在不同的计算机上，即使在同一网络上，此值也可能不同。它可以在同一台计算机上进行更改，导致存储的数据变得将损坏。此值仅用于临时使用，如果可能，永久存储应使用 UTF-16 或 UTF-8。</para>
      /// <para>CP_SYMBOL：Windows 2000:符号代码页 (42)。</para>
      /// <para>CP_THREAD_ACP：Windows 2000:当前线程的 Windows ANSI 代码页。注意在不同的计算机上，即使在同一网络上，此值也可能不同。它可以在同一台计算机上进行更改，从而导致存储的数据变得将损坏。此值仅用于临时使用，如果可能，永久存储应使用 UTF-16 或 UTF-8。</para>
      /// <para>CP_UTF7：UTF-7。仅当由7位传输机制强制时才使用此值。使用 UTF-8 是首选。设置此值后，lpDefaultChar和lpUsedDefaultChar必须设置为NULL。</para>
      /// <para>CP_UTF8：UTF-8。设置此值后，lpDefaultChar和lpUsedDefaultChar必须设置为NULL。</para></param>
      /// <param name="flags">指示转换类型的标志。应用程序可以指定以下值的组合。当没有设置这些标志时，函数的执行速度会更快。应用程序应指定具有特定值WC_DEFAULTCHAR的WC_NO_BEST_FIT_CHARS和WC_COMPOSITECHECK，以检索所有可能的转换结果。如果未提供所有三值，则会丢失某些结果：
      /// <para>WC_COMPOSITECHECK：转换复合字符, 由基字符和是非间距字符组成, 每个字元具有不同的字符值。将这些字符转换为 precomposed 字符, 它们具有基是非间距字符组合的单个字符值。</para>
      /// <para>&gt;&gt;注意：Windows通常表示具有precomposed数据的Unicode字符串, 因此不需要使用WC_COMPOSITECHECK标志。</para>
      /// <para>WC_ERR_INVALID_CHARS：Windows Vista 和更高版本适用，如果遇到无效输入字符，则会失败（通过返回0并将最后一个错误代码设置为ERROR_NO_UNICODE_TRANSLATION）。可以使用对时出错的调用来检索最后一个错误代码。如果未设置此标志，该函数将用 U + FFFD 替换非法序列（为指定的代码页适当编码），并通过返回转换后的字符串的长度来成功。请注意，仅当将代码页指定为CP_UTF8或54936时， 此标志才适用。它不能与其他代码页值一起使用。</para>
      /// <para>WC_NO_BEST_FIT_CHARS：将不直接转换为多字节等效项的任何 Unicode 字符转换为lpDefaultChar指定的默认字符。换言之，如果从 Unicode 转换为多字节，又返回到 unicode，则不产生相同的 unicode 字符，则该函数使用默认字符。此标志可由自身使用，也可以与其他已定义的标志相结合。对于需要验证的字符串 (如文件、资源和用户名)，应用程序应始终使用 WC_NO_BEST_FIT_CHARS 标志。此标志可防止函数将字符映射到看似相似但具有非常不同语义的字符。在某些情况下，语义变化可能是极端的。例如，"∞" (无穷大) 的符号映射到某些代码页中的 8。</para>
      /// 对于下面列出的代码页, flags必须为0。否则, 函数将以 ERROR_INVALID_FLAGS 失败。
      /// <para>&gt;&gt; 50220</para>
      /// <para>&gt;&gt; 50221</para>
      /// <para>&gt;&gt; 50222</para>
      /// <para>&gt;&gt; 50225</para>
      /// <para>&gt;&gt; 50227</para>
      /// <para>&gt;&gt; 50229</para>
      /// <para>&gt;&gt; 57002 through 57011</para>
      /// <para>&gt;&gt; 65000 (UTF-7)</para>
      /// <para>&gt;&gt; 42 (Symbol)</para>
      /// 对于代码页 65001 (UTF-8) 或代码页 54936 (GB18030、Windows Vista 和更高版本), dwFlags必须设置为0或 WC_ERR_INVALID_CHARS. 否则, 函数将以 ERROR_INVALID_FLAGS 失败。</param>
      /// <param name="wideCharStr">指向要转换的 Unicode 字符串的指针。</param>
      /// <param name="wideChar">wideCharStr所指示的字符串的大小 (以字符为形式)。或者，如果字符串为 null 终止，则此参数可以设置为-1。如果wideChar设置为 0，则函数将失败。如果此参数为-1，则函数处理整个输入字符串，包括终止 null 字符。因此，生成的字符串具有终止 null 字符，该函数返回的长度包括此字符。如果此参数设置为正整数，则函数将完全处理指定的字符数。如果提供的大小不包括终止 null 字符，则生成的字符串不会以 null 结尾，返回的长度不包括此字符。</param>
      /// <param name="multiByteStr">指向接收转换字符串的缓冲区的指针。</param>
      /// <param name="multiByte">multiByteStr指示的缓冲区大小 (以字节为单位)。如果此参数设置为 0，则该函数返回lpMultiByteStr所需的缓冲区大小，并且不使用输出参数本身。</param>
      /// <param name="defaultChar">如果在指定的代码页中不能表示字符, 则指向要使用的字符的指针。如果函数要使用系统默认值, 应用程序将此参数设置为NULL 。若要获取系统默认字符, 应用程序可以调用 GetCPInfo 或 GetCPInfoEx 函数。
      /// <para>&gt;&gt;注意：对于代码页的 CP_UTF7 和 CP_UTF8 设置, 此参数必须设置为NULL。否则, 函数将以 ERROR_INVALID_PARAMETER 失败。</para></param>
      /// <param name="usedDefaultChar">指向指示函数是否在转换中使用了默认字符的标志的指针。如果源字符串中的一个或多个字符不能在指定的代码页中表示，则该标志设置为TRUE。否则，标志设置为FALSE。此参数可以设置为NULL。</param>
      /// <returns>如果成功，则返回写入到lpMultiByteStr指向的缓冲区的字节数。如果函数成功且cbMultiByte为 0，则返回值是lpMultiByteStr指示的缓冲区所需的大小 (以字节为单位)。另外，有关在输入无效序列时 WC_ERR_INVALID_CHARS 标志如何影响返回值的信息，请参见flags。
      /// <para>如果该函数未成功，则返回0。若要获取扩展的错误信息，应用程序可以调用时出错，它可以返回以下错误代码之一：</para>
      /// <para>ERROR_INSUFFICIENT_BUFFER：提供的缓冲区大小不够大，或者错误地设置为NULL。</para>
      /// <para>ERROR_INVALID_FLAGS：为标志提供的值无效。</para>
      /// <para>ERROR_INVALID_PARAMETER：任何参数值都无效。</para>
      /// <para>ERROR_NO_UNICODE_TRANSLATION：在字符串中找到无效的 Unicode。</para></returns>
      /// <remarks>不正确使用WideCharToMultiByte函数可能会危及应用程序的安全性。调用此函数可能会导致缓冲区溢出，因为wideCharStr所指示的输入缓冲区的大小等于Unicode字符串中的字符数，而multiByteStr指示的输出缓冲区的大小等于字节数。为避免缓冲区溢出，应用程序必须指定适合缓冲区接收的数据类型的缓冲区大小。
      /// <para>ANSI代码页在不同的计算机上可以不同，或者可以为一台计算机更改，导致数据损坏。对于最一致的结果，应用程序应使用Unicode（如UTF-8或UTF-16）而不是特定的代码页，除非旧式标准或数据格式阻止使用Unicode。如果无法使用Unicode，应用程序应在协议允许的情况下使用适当的编码名称对数据流进行标记。HTML和XML文件允许进行标记，但文本文件却没有。</para></remarks>
      [DllImport("kernel32.dll")]
      private static extern int WideCharToMultiByte(uint codePage, uint flags, [MarshalAs(UnmanagedType.LPWStr)] string wideCharStr, int wideChar, [MarshalAs(UnmanagedType.LPArray)] Byte[] multiByteStr, int multiByte, IntPtr defaultChar, out bool usedDefaultChar);
      /// <summary>
      /// 检索指定电源设置、子组或方案的友好名称。
      /// </summary>
      /// <param name="rootPowerKey">此参数保留为将来使用，必须设置为NULL。</param>
      /// <param name="schemeGuid">电源方案的标识符，如果此参数为NULL，则返回电源策略的枚举。</param>
      /// <param name="subGroupOfPowerSettingsGuid">电源设置的子组。如果此参数为NULL，则返回PolicyGuid项下的设置枚举。
      /// <para>NO_SUBGROUP_GUID：fea3413e-7e05-4911-9a71-700331f1c294，Settings in this subgroup are part of the default power scheme.</para>
      /// <para>GUID_DISK_SUBGROUP：0012ee47-9041-4b5d-9b77-535fba8b1442，Settings in this subgroup control power management configuration of the system's hard disk drives.</para>
      /// <para>GUID_SYSTEM_BUTTON_SUBGROUP：4f971e89-eebd-4455-a8de-9e59040e7347，Settings in this subgroup control configuration of the system power buttons.</para>
      /// <para>GUID_PROCESSOR_SETTINGS_SUBGROUP：54533251-82be-4824-96c1-47b60b740d00，Settings in this subgroup control configuration of processor power management features.</para>
      /// <para>GUID_VIDEO_SUBGROUP：7516b95f-f776-4464-8c53-06167f40cc99，Settings in this subgroup control configuration of the video power management features.</para>
      /// <para>GUID_BATTERY_SUBGROUP：e73a048d-bf27-4f12-9731-8b2076e8891f，Settings in this subgroup control battery alarm trip points and actions.</para>
      /// <para>GUID_SLEEP_SUBGROUP：238C9FA8-0AAD-41ED-83F4-97BE242C8F20，Settings in this subgroup control system sleep settings.</para>
      /// <para>GUID_PCIEXPRESS_SETTINGS_SUBGROUP：501a4d13-42af-4429-9fd1-a8218c268e20，Settings in this subgroup control PCI Express settings.</para></param>
      /// <param name="powerSettingGuid">正在使用的电源设置的标识符。</param>
      /// <param name="buffer">指向要接收元素的变量的指针，如果此参数为NULL，则该函数将检索所需缓冲区的大小。</param>
      /// <param name="bufferSize">指向输入的变量的指针包含缓冲区参数指向的缓冲区的大小，如果缓冲区参数为NULL，或者bufferSize不够大，则该函数将返回ERROR_MORE_DATA，并且该变量接收到所需的缓冲区大小。</param>
      /// <returns></returns>
      [DllImport("powrprof.dll")]
      private static extern uint PowerReadFriendlyName(IntPtr rootPowerKey, IntPtr schemeGuid, IntPtr subGroupOfPowerSettingsGuid, IntPtr powerSettingGuid, IntPtr buffer, ref uint bufferSize);
      /// <summary>
      /// 检索活动的电源计划方案并返回标识该方案的GUID。
      /// </summary>
      /// <param name="userRootPowerKey">此参数保留为将来使用，必须设置为NULL。</param>
      /// <param name="activePolicyGuid">一个指针，它接收指向GUID结构的指针，使用LocalFree函数来释放这个内存。</param>
      /// <returns>如果调用成功，返回ERROR_SUCCESS(0)，如果调用失败，则返回非零值。</returns>
      [DllImport("powrprof.dll")]
      private static extern UInt32 PowerGetActiveScheme(IntPtr userRootPowerKey, ref IntPtr activePolicyGuid);
      public PowerSchemesManager() : base() => _guids = new List<Guid>();
      /// <summary>
      /// 获取所有的电源计划的GUID。
      /// </summary>
      public List<Guid> Guids
      {
         get
         {
            IntPtr readBuffer;
            uint bufferSize = 16;
            uint index = 0;
            uint returnCode = 0;
            while (returnCode == 0)
            {
               readBuffer = Marshal.AllocHGlobal((int)bufferSize);
               try
               {
                  returnCode = PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, EPowerDataAccessor.AccessScheme, index, readBuffer, ref bufferSize);
                  if (returnCode == 259) break;
                  if (returnCode != 0) throw new COMException("获取电源计划失败，错误代码：" + returnCode);
                  Guid NewGuid = (Guid)Marshal.PtrToStructure(readBuffer, typeof(Guid));
                  _guids.Add(NewGuid);
               }
               finally { Marshal.FreeHGlobal(readBuffer); }
               index++;
            }
            return _guids;
         }
      }
      public string GetActiveSchemeFriendlyName()
      {
         IntPtr ptrActiveGuid = IntPtr.Zero;
         uint res = PowerGetActiveScheme(IntPtr.Zero, ref ptrActiveGuid);
         if (res == 0)
         {
            uint buffSize = 0;
            res = PowerReadFriendlyName(IntPtr.Zero, ptrActiveGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref buffSize);
            if (res == 0)
            {
               IntPtr ptrName = Marshal.AllocHGlobal((int)buffSize);
               res = PowerReadFriendlyName(IntPtr.Zero, ptrActiveGuid, IntPtr.Zero, IntPtr.Zero, ptrName, ref buffSize);
               if (res == 0)
               {
                  string ret = Marshal.PtrToStringUni(ptrName);
                  Marshal.FreeHGlobal(ptrName);
                  return ret;
               }
               Marshal.FreeHGlobal(ptrName);
            }
         }
         throw new COMException("无法读取活动的电源计划，错误代码：" + res);
      }
      //TODO: 这个类还没有完成。
   }
   /// <summary>
   /// 电源方案列举的方式的枚举。
   /// </summary>
   public enum EPowerDataAccessor : int
   {
      /// <summary>
      /// 枚举电源方案。
      /// </summary>
      [EnumerationDescription("枚举电源方案")]
      AccessScheme = 0x0010,
      /// <summary>
      /// 在SchemeGuid下枚举子组。
      /// </summary>
      [EnumerationDescription("在SchemeGuid下枚举子组")]
      AccessSubgroup = 0x0011,
      /// <summary>
      /// 枚举SchemeGuid\SubgroupOfPowerSettingsGuid下的单个电源设置。
      /// </summary>
      [EnumerationDescription("枚举SchemeGuid\\SubgroupOfPowerSettingsGuid下的单个电源设置")]
      AccessIndividualSetting = 0x0012
   }
}
