using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

class FNProc
{
    public static Task<Process> Launch(string exePath, bool FreezeProc = false, string Arg = "")
    {
        if (!File.Exists(exePath))
            throw new FileNotFoundException("Executable not found", exePath);

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"{Arg} -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -skippatchcheck -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_TYPE=epic",
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            }
        };

        process.Start();

        if (FreezeProc)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr handle = OpenThread(THREAD_SUSPEND_RESUME, false, thread.Id);
                if (handle != IntPtr.Zero)
                {
                    SuspendThread(handle);
                    CloseHandle(handle);
                }
            }
        }

        return Task.FromResult(process);
    }



    private const int THREAD_SUSPEND_RESUME = 0x0002;

    [DllImport("kernel32.dll")]
    private static extern int SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);
}
