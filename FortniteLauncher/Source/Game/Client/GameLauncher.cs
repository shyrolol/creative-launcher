using System;
using System.Threading.Tasks;

class Fortnite
{
    public static async Task Launch(string GamePath)
    {
        try
        {
            await RequiredFilesDownloader.Download();

            string basePath = $"{GamePath}\\FortniteGame\\Binaries\\Win64";

            string encodedEmail = Uri.EscapeDataString(GlobalSettings.Options.Email);
            string encodedPassword = Uri.EscapeDataString(GlobalSettings.Options.Password);

            string authArgs =
                $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us " +
                $"-AUTH_TYPE=epic -AUTH_LOGIN={GlobalSettings.Options.Email} -AUTH_PASSWORD={GlobalSettings.Options.Password} " +
                $"-fltoken=3db3ba5dcbd2e16703f3978d " +
                $"-caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9...";

            var shipping = await FNProc.Launch($"{basePath}\\FortniteClient-Win64-Shipping.exe", false, authArgs);

            var launcher = await FNProc.Launch($"{basePath}\\FortniteLauncher.exe", true);
            var be = await FNProc.Launch($"{basePath}\\FortniteClient-Win64-Shipping_BE.exe", true);
            var eac = await FNProc.Launch($"{basePath}\\FortniteClient-Win64-Shipping_EAC.exe", true);

            if (Definitions.bEnableEAC)
            {
                await FNProc.Launch(
                    $"{GamePath}\\Eon_EAC.exe",
                    false,
                    $"-AUTH_LOGIN=\"{GlobalSettings.Options.Email}\" -AUTH_PASSWORD=\"{GlobalSettings.Options.Password}\""
                );
            }

            LaunchStatusService.OnGameOpened();
        }
        catch (Exception Error)
        {
            DialogService.ShowSimpleDialog($"{Error.Message}", "Whoops! Something went wrong.");
        }
    }
}
