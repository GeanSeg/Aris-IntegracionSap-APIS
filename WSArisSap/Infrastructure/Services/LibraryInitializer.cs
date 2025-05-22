using Application.Interfaces;
using LanguageExt.ClassInstances;
using System.Runtime.InteropServices;

namespace Infrastructure.Services
{
    public sealed class LibraryInitialize(ILoggingService loggingService): ILibraryInitializer
    {
       public bool InitializeLibrary()
        {
            bool rpta = true;

            try
            {
                loggingService.LogInfo("InitializeLibrary : Cargando librerias necesarias");

                string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
                string[] dlls = { "icuuc50.dll", "icudt50.dll", "icuin50.dll" };

                foreach (var dll in dlls)
                {
                    string fullPath = Path.Combine(basePath, dll);

                    loggingService.LogInfo($"InitializeLibrary : Se ha cargo la libreria {dll} correctamente");

                    if (!File.Exists(fullPath))
                    {
                        loggingService.LogError($" InitializeLibrary : No se cargó la DLL: {fullPath}");
                        rpta = false;
                        break;
                    }

                    NativeLibrary.Load(fullPath);
                }

                loggingService.LogInfo($"InitializeLibrary : Se han cargado las librerias correctamente");

            }
            catch (Exception ex)
            {
                loggingService.LogError($"InitializeLibrary : Error al cargar las librerías ICU: {ex.Message}");
                rpta = false;
            }
            return rpta;
        }
    }
}
