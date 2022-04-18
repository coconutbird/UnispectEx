﻿using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using UnispectEx.Inspector;
using UnispectEx.Mono;

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
            foreach (var container in _metadataContainers!) {
                if (!serializer.Serialize(writer, container))
                    return false;
            }

            return true;
        }

        public bool Write(IEnumerable<MetadataContainer> metadataContainers, string outputDirectory) {
            using var file = File.Open(Path.Join(outputDirectory, "dump.txt"), FileMode.Create);

            file.Write(_serializedData.GetBuffer());

            return true;
        }

        private IEnumerable<MetadataContainer>? _metadataContainers;
        private readonly MemoryStream _serializedData = new();
    }
}