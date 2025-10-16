using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.IO;

public static class TfsHelper
{
    public static void AgregarAFuente(string localPath)
    {
        string serverUrl = "http://desarrollo:8080/tfs/sistemas%20municipales/Tributos/Equipo%20Tributos";
        // Conexión al TFS/Azure DevOps
        var tfs = new TfsTeamProjectCollection(new Uri(serverUrl));
        tfs.EnsureAuthenticated();

        var versionControl = tfs.GetService<VersionControlServer>();

        // Obtener el workspace asociado a la ruta local
        Workspace workspace = versionControl.TryGetWorkspace(localPath);
        if (workspace == null)
        {
            throw new InvalidOperationException($"No se encontró un Workspace para la ruta {localPath}");
        }

        // Recursivamente agrega archivos y carpetas
        AgregarRecursivo(workspace, localPath);
    }

    private static void AgregarRecursivo(Workspace workspace, string path)
    {
        if (Directory.Exists(path))
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                AgregarRecursivo(workspace, dir);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                AgregarRecursivo(workspace, file);
            }
        }
        else if (File.Exists(path))
        {
            // Verificar si ya está pendiente o bajo control de versiones
            var pending = workspace.GetPendingChanges(path);
            var item = workspace.VersionControlServer.GetItem(path, VersionSpec.Latest, DeletedState.NonDeleted, GetItemsOptions.None);

            if (pending.Length == 0 && item == null) // ni pendiente ni bajo control
            {
                workspace.PendAdd(path, false);
                Console.WriteLine($"✅ Marcado para agregar: {path}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Ya existe o pendiente: {path}");
            }
        }
    }
}






//using System;
//using System.IO;
//using Microsoft.TeamFoundation.Client;
//using Microsoft.TeamFoundation.VersionControl.Client;

//public static class TfsHelper
//{
//    /// <summary>
//    /// Marca como PendAdd un archivo o carpeta dentro del workspace asociado al .sln.
//    /// </summary>
//    /// <param name="solutionPath">Ruta completa del archivo .sln</param>
//    /// <param name="targetPath">Ruta de la carpeta o archivo a agregar</param>
//    public static void AgregarAFuenteDesdeSln(string solutionPath, string targetPath)
//    {
//        // Carpeta del .sln
//        string carpetaSln = Path.GetDirectoryName(solutionPath);

//        // Detectar la URL de la colección desde el workspace
//        string serverUrl = DetectarColeccion(carpetaSln);

//        // Conexión a la colección
//        var tpc = new TfsTeamProjectCollection(new Uri(serverUrl));
//        tpc.EnsureAuthenticated();

//        var versionControl = tpc.GetService<VersionControlServer>();

//        // Obtener workspace para la ruta local
//        Workspace workspace = versionControl.TryGetWorkspace(carpetaSln);
//        if (workspace == null)
//            throw new InvalidOperationException($"No se encontró un Workspace para la ruta {carpetaSln}");

//        Console.WriteLine($"Usando workspace: {workspace.Name}");
//        Console.WriteLine($"Server URL: {workspace.VersionControlServer.TeamProjectCollection.Uri}");

//        // Agregar archivos/carpeta
//        AgregarRecursivo(workspace, targetPath);
//    }

//    /// <summary>
//    /// Obtiene la URL de la colección a partir del workspace que contiene el path dado.
//    /// </summary>
//    private static string DetectarColeccion(string localPath)
//    {
//        //// Busca info del workspace local
//        //WorkspaceInfo info = Workstation.Current.GetLocalWorkspaceInfo(localPath);
//        //if (info == null)
//        //    throw new InvalidOperationException($"No se encontró un Workspace para la ruta {localPath}");

//        //return info.ServerUri.AbsoluteUri;

//        // Crea un cliente con cualquier conexión (localhost basta)
//        var dummyTpc = new TfsTeamProjectCollection(new Uri("http://localhost:8080/tfs"));
//        var versionControl = dummyTpc.GetService<VersionControlServer>();

//        return versionControl.GetWorkspace(localPath).DisplayName;
//    }

//    private static void AgregarRecursivo(Workspace workspace, string path)
//    {
//        if (Directory.Exists(path))
//        {
//            foreach (var dir in Directory.GetDirectories(path))
//                AgregarRecursivo(workspace, dir);

//            foreach (var file in Directory.GetFiles(path))
//                AgregarRecursivo(workspace, file);
//        }
//        else if (File.Exists(path))
//        {
//            // Chequear si ya está pendiente o versionado
//            var pending = workspace.GetPendingChanges(path);
//            bool yaVersionado = workspace.VersionControlServer.ServerItemExists(
//                workspace.GetServerItemForLocalItem(path), ItemType.File);

//            if (pending.Length == 0 && !yaVersionado)
//            {
//                workspace.PendAdd(path, false);
//                Console.WriteLine($"✅ Marcado para agregar: {path}");
//            }
//            else
//            {
//                Console.WriteLine($"ℹ️ Ya existe o pendiente: {path}");
//            }
//        }
//    }
//}


////using System;
////using System.IO;
////using Microsoft.TeamFoundation.Client;
////using Microsoft.TeamFoundation.VersionControl.Client;

////public static class TfsHelper
////{
////    /// <summary>
////    /// Marca como PendAdd un archivo o carpeta dentro del workspace asociado al .sln.
////    /// </summary>
////    /// <param name="solutionPath">Ruta completa del archivo .sln</param>
////    /// <param name="targetPath">Ruta de la carpeta o archivo a agregar</param>
////    public static void AgregarAFuenteDesdeSln(string solutionPath, string targetPath)
////    {
////        // Carpeta del .sln
////        string carpetaSln = Path.GetDirectoryName(solutionPath);

////        // Buscar el workspace correspondiente
////        var versionControl = new TfsTeamProjectCollectionFactory().GetTeamProjectCollection(new Uri(DetectarColeccion(carpetaSln)))
////            .GetService<VersionControlServer>();

////        Workspace workspace = versionControl.TryGetWorkspace(carpetaSln);
////        if (workspace == null)
////            throw new InvalidOperationException($"No se encontró un Workspace para la ruta {carpetaSln}");

////        Console.WriteLine($"Usando workspace: {workspace.Name}");
////        Console.WriteLine($"Server URL: {workspace.VersionControlServer.TeamProjectCollection.Uri}");

////        // Agregar archivos/carpeta
////        AgregarRecursivo(workspace, targetPath);
////    }

////    /// <summary>
////    /// Obtiene la URL de la colección a partir del workspace que contiene el path dado.
////    /// </summary>
////    private static string DetectarColeccion(string localPath)
////    {
////        // TFS resuelve el workspace directamente desde la máquina local
////        VersionControlServer vctrl = Workstation.Current.GetLocalWorkspaceInfo(localPath).GetWorkspace(TfsTeamProjectCollectionFactory.GetTeamProjectCollection).VersionControlServer;
////        return vctrl.TeamProjectCollection.Uri.ToString();
////    }

////    private static void AgregarRecursivo(Workspace workspace, string path)
////    {
////        if (Directory.Exists(path))
////        {
////            foreach (var dir in Directory.GetDirectories(path))
////                AgregarRecursivo(workspace, dir);

////            foreach (var file in Directory.GetFiles(path))
////                AgregarRecursivo(workspace, file);
////        }
////        else if (File.Exists(path))
////        {
////            var pending = workspace.GetPendingChanges(path);
////            var item = workspace.VersionControlServer.GetItem(path, VersionSpec.Latest, DeletedState.NonDeleted, GetItemsOptions.None);

////            if (pending.Length == 0 && item == null)
////            {
////                workspace.PendAdd(path, false);
////                Console.WriteLine($"✅ Marcado para agregar: {path}");
////            }
////            else
////            {
////                Console.WriteLine($"ℹ️ Ya existe o pendiente: {path}");
////            }
////        }
////    }
////}



//////using System;
//////using System.IO;
//////using Microsoft.TeamFoundation.Client;
//////using Microsoft.TeamFoundation.VersionControl.Client;

//////public static class TfsHelper
//////{
//////    public static void AgregarAFuente(string localPath)
//////    {
//////        string serverUrl = "http://desarrollo:8080/tfs/sistemas%20municipales/Tributos/Equipo%20Tributos";
//////        // Conexión al TFS/Azure DevOps
//////        var tfs = new TfsTeamProjectCollection(new Uri(serverUrl));
//////        tfs.EnsureAuthenticated();

//////        var versionControl = tfs.GetService<VersionControlServer>();

//////        // Obtener el workspace asociado a la ruta local
//////        Workspace workspace = versionControl.TryGetWorkspace(localPath);
//////        if (workspace == null)
//////        {
//////            throw new InvalidOperationException($"No se encontró un Workspace para la ruta {localPath}");
//////        }

//////        // Recursivamente agrega archivos y carpetas
//////        AgregarRecursivo(workspace, localPath);
//////    }

//////    private static void AgregarRecursivo(Workspace workspace, string path)
//////    {
//////        if (Directory.Exists(path))
//////        {
//////            foreach (var dir in Directory.GetDirectories(path))
//////            {
//////                AgregarRecursivo(workspace, dir);
//////            }

//////            foreach (var file in Directory.GetFiles(path))
//////            {
//////                AgregarRecursivo(workspace, file);
//////            }
//////        }
//////        else if (File.Exists(path))
//////        {
//////            // Verificar si ya está pendiente o bajo control de versiones
//////            var pending = workspace.GetPendingChanges(path);
//////            var item = workspace.VersionControlServer.TryGetItem(path, VersionSpec.Latest, DeletedState.NonDeleted, GetItemsOptions.None);

//////            if (pending.Length == 0 && item == null) // ni pendiente ni bajo control
//////            {
//////                workspace.PendAdd(path, recursive: false);
//////                Console.WriteLine($"✅ Marcado para agregar: {path}");
//////            }
//////            else
//////            {
//////                Console.WriteLine($"ℹ️ Ya existe o pendiente: {path}");
//////            }
//////        }
//////    }
//////}


////////using System;
////////using System.Diagnostics;
////////using System.IO;
////////using System.Linq;

////////namespace Capibara
////////{
////////    public class TfsHelper
////////    {
////////        private readonly string _workspacePath;

////////        public string teamFoundation = string.Empty;

////////        public TfsHelper(string workspacePath)
////////        {
////////            _workspacePath = workspacePath;


////////            try
////////            {
////////                // Carpeta destino (ejemplo: C:\MisDescargas)
////////                string carpetaDestino = AppDomain.CurrentDomain.BaseDirectory + @"\TeamFoundation\";
////////                //string carpetaDestino = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\";

////////                // Asegurar que exista la carpeta
////////                if (!Directory.Exists(carpetaDestino))
////////                    Directory.CreateDirectory(carpetaDestino);

////////                // Nombre final del archivo
////////                teamFoundation = Path.Combine(carpetaDestino, "TF.exe");

////////                if (!File.Exists(teamFoundation))
////////                {
////////                    // Guardar el recurso en disco
////////                    File.WriteAllBytes(teamFoundation, Capibara.Properties.Resources.TF);
////////                }
////////            }
////////            catch (Exception ex)
////////            {
////////            }
////////        }

////////        /// <summary>
////////        /// Agrega un archivo o carpeta a TFS si no está ya bajo control.
////////        /// - Si es carpeta nueva → la agrega completa (/recursive).
////////        /// - Si la carpeta ya está en TFS → revisa sus archivos internos y agrega los faltantes.
////////        /// - Si es archivo → lo agrega solo si no está en TFS.
////////        /// </summary>
////////        public void AddIfNotInTfs(string ruta)
////////        {
////////            if (File.Exists(ruta))
////////            {
////////                // Es archivo → procesar individual
////////                AddFileIfNotInTfs(ruta);
////////            }
////////            else if (Directory.Exists(ruta))
////////            {
////////                // Primero reviso si la carpeta ya está en TFS
////////                string salida = EjecutarTf($"info \"{ruta}\"");

////////                if (salida.IndexOf("No items match", StringComparison.OrdinalIgnoreCase) >= 0 ||
////////                    salida.IndexOf("No se encontró ningún elemento", StringComparison.OrdinalIgnoreCase) >= 0)
////////                {
////////                    // Carpeta nueva → agregarla completa
////////                    EjecutarTf($"add \"{ruta}\" /recursive");
////////                    Console.WriteLine("Carpeta agregada completa a TFS: " + ruta);
////////                }
////////                else
////////                {
////////                    // Carpeta ya está en TFS → revisar archivos internos
////////                    foreach (string archivo in Directory.GetFiles(ruta, "*.*", SearchOption.AllDirectories))
////////                    {
////////                        AddFileIfNotInTfs(archivo);
////////                    }
////////                }
////////            }
////////        }

////////        private void AddFileIfNotInTfs(string archivo)
////////        {
////////            string salida = EjecutarTf($"info \"{archivo}\"");

////////            if (salida.IndexOf("No items match", StringComparison.OrdinalIgnoreCase) >= 0 ||
////////                salida.IndexOf("No se encontró ningún elemento", StringComparison.OrdinalIgnoreCase) >= 0)
////////            {
////////                EjecutarTf($"add \"{archivo}\"");
////////                Console.WriteLine("Archivo agregado a TFS: " + archivo);
////////            }
////////            else
////////            {
////////                Console.WriteLine("Archivo ya en TFS: " + archivo);
////////            }
////////        }

////////        private string EjecutarTf(string argumentos)
////////        {
////////            var psi = new ProcessStartInfo
////////            {
////////                FileName = teamFoundation,
////////                Arguments = argumentos,
////////                WorkingDirectory = Path.GetDirectoryName(_workspacePath),
////////                UseShellExecute = false,
////////                RedirectStandardOutput = true,
////////                RedirectStandardError = true,
////////                CreateNoWindow = true
////////            };

////////            using (var proceso = Process.Start(psi))
////////            {
////////                string salida = proceso.StandardOutput.ReadToEnd();
////////                string error = proceso.StandardError.ReadToEnd();
////////                proceso.WaitForExit();

////////                if (!string.IsNullOrEmpty(error))
////////                {
////////                    Console.WriteLine("⚠️ TF Error: " + error);
////////                }

////////                return salida;
////////            }
////////        }
////////    }
////////}