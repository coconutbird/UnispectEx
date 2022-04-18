﻿using System.Collections.Generic;
using System.IO;

namespace UnispectEx.Inspector {
    internal class DefaultDumpProcessor : IDumpProcessor {
        public bool Initialize(IEnumerable<MetadataContainer> metadataContainers) {
            _metadataContainers = metadataContainers;
            
            return true;
        }

        public bool Mark() {
            foreach (var container in _metadataContainers!) {
                container.Export = true;

                foreach (var metadataFieldContainer in container.Fields)
                    metadataFieldContainer.Export = true;
            }

            return true;
        }

        public bool Serialize(IDumpSerializer serializer) {
            var stream = _serializedData;

            using var writer = new StreamWriter(stream);

            writer.WriteLine("*** GENERATED BY https://github.com/coconutbird/UnispectEx ***");

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

            file.Write(_serializedData.GetBuffer());

            return true;
        }

        private IEnumerable<MetadataContainer>? _metadataContainers;
        private readonly MemoryStream _serializedData = new();
    }
}