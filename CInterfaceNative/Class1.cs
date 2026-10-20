using System.Runtime.InteropServices;
using InternalLibrary;

namespace CInterfaceNative;

public static class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "add")]
    public static int Add(int left, int right) => Calculator.Add(left, right);

    [UnmanagedCallersOnly(EntryPoint = "subtract")]
    public static int Subtract(int left, int right) => Calculator.Subtract(left, right);
}
