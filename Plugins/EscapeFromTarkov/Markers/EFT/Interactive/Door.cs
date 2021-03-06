using System.Collections.Immutable;

using EscapeFromTarkov.Extensions;

using UnispectEx.Core.Inspector;

namespace EscapeFromTarkov.Markers.EFT.Interactive; 

internal class Door : IMarker {
    public bool Mark(ImmutableList<MetadataContainer> containers) {
        var doorContainer = containers.FindContainerByFullName("EFT.Interactive.Door");

        if (doorContainer is null)
            return false;

        doorContainer.CleanPropertyFieldNames();
        doorContainer.ExportNonObfuscatedSymbols();

        return true;
    }
}