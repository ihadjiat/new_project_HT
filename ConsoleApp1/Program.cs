using System.Runtime.InteropServices;
using InternalLibrary;

Console.WriteLine($"Managed Add(10, 5) = {Calculator.Add(10, 5)}");
Console.WriteLine($"Managed Subtract(10, 5) = {Calculator.Subtract(10, 5)}");

try
{
    Console.WriteLine($"Native add(7, 2) = {NativeExports.Add(7, 2)}");
    Console.WriteLine($"Native subtract(7, 2) = {NativeExports.Subtract(7, 2)}");
}
catch (DllNotFoundException)
{
    Console.WriteLine("Native CInterfaceNative DLL not found. Publish CInterfaceNative to test C ABI calls.");
}

internal static class NativeExports
{
    [DllImport("CInterfaceNative", EntryPoint = "add")]
    internal static extern int Add(int left, int right);

    [DllImport("CInterfaceNative", EntryPoint = "subtract")]
    internal static extern int Subtract(int left, int right);
}
