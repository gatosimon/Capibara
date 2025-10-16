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