﻿using System.Collections.Generic;
using System.IO;

using UnispectEx.Core.Inspector;

namespace UnispectEx.Processors; 

public class DefaultDumpProcessor : IDumpProcessor {
    public bool Initialize(IEnumerable<MetadataContainer> metadataContainers) {
        _metadataContainers = metadataContainers;
            
        return true;
    }

    public bool Serialize(IDumpSerializer serializer) {
        var stream = _serializedData;

        using var writer = new StreamWriter(stream);

        writer.WriteLine("// *** GENERATED BY https://github.com/coconutbird/UnispectEx ***");

        foreach (var container in _metadataContainers!) {
            if (!container.Export)
                continue;

            if (!serializer.Serialize(writer, container))
                return false;
        }

        return true;
    }

    public bool Write(string outputDirectory) {
        using var file = File.Open(Path.Join(outputDirectory, "dump.txt"), FileMode.Create);

        file.Write(_serializedData.ToArray());

        return true;
    }

    private IEnumerable<MetadataContainer>? _metadataContainers;
    private readonly MemoryStream _serializedData = new();
}